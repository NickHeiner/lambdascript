import '../types';

const chalk = require('chalk'),
    _str = require('underscore.string'),
    _ = require('lodash'),
    traverse = require('traverse'),
    logger = require('../util/logger'),
    os = require('os'),

    colorMap: IStringMap = {
        Identifier: 'magenta',
        Literal: 'yellow',
        StringRegexLookup: 'green',
        Boolean: 'blue'
    },

    alternateLocMap: IStringMap = {
        StringRegexLookup: 'regexLoc',
        Boolean: 'booleanLoc'
    };

interface IStringMap {
    [key: string]: string;
}

interface ILineColorAction {
    lineIndex: number;
    colStart: number;
    colEnd: number;
    colorFn: (str: string) => string;
}

function getHighlightedCode(lscAst: ILambdaScriptAstNode, lambdaScriptCode: string) {
    const codeByLines = _str.lines(lambdaScriptCode),
        colorActions = traverse(lscAst)
            .reduce(function(colorActions: ILineColorAction[], astNode: ILambdaScriptAstNode) {
                const color = colorMap[astNode.type];

                if (color) {
                    const colorFn = chalk[color].bind(chalk),
                        locKey = alternateLocMap[astNode.type] || 'loc',

                        // I'm just using _.result to avoid TS7017
                        loc: ILoc = _.result(astNode, locKey);

                    /**
                     * Originally, I just colored as soon as I found a node. However, this does not work,
                     * because other coloring may have occurred on the line already, which will offset our
                     * column indices. And it is not sufficient just to drop all color from the line and
                     * see what the length difference is, because some of that color could be *after* our
                     * column indices and thus irrelevant. To solve this, we will gather all the colorings
                     * we want to do, and then apply them in sorted order. This makes the offset caclulation trivial.
                     */
                    return colorActions.concat({
                        // I don't know why but jison does not 0-index the line number.
                        lineIndex: loc.first_line - 1,
                        colStart: loc.first_column,
                        colEnd: loc.last_column,
                        colorFn: colorFn
                    });
                }

                return colorActions;
            }, []),

        coloredCode = _(colorActions)
            .sortByAll(['lineIndex', 'colStart'])
            .reduce(function(lines: string[], colorAction: ILineColorAction) {

                const linesClone = _.clone(lines),
                    lineToColor = linesClone[colorAction.lineIndex],

                    existingColorOffset = lineToColor.length - chalk.stripColor(lineToColor).length,

                    colStartWithOffset = colorAction.colStart + existingColorOffset,
                    colEndWithOffset = colorAction.colEnd + existingColorOffset;

                // One potential way to fail fast here would be to detect when we are coloring
                // over an existing highlight. I have yet to encounter a case where that was expected behavior.

                linesClone[colorAction.lineIndex] =
                    lineToColor.slice(0, colStartWithOffset) +
                    colorAction.colorFn(lineToColor.slice(colStartWithOffset, colEndWithOffset)) +
                    lineToColor.slice(colEndWithOffset);

                logger.debug({
                    original: lines[colorAction.lineIndex],
                    colored: linesClone[colorAction.lineIndex]
                }, 'Coloring line');

                return linesClone;
            }, codeByLines);

    return coloredCode.join(os.EOL);
}

export = getHighlightedCode;
