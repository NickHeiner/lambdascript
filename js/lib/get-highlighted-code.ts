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
        StringRegexLookup: 'green'
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
                        loc = astNode.type === 'StringRegexLookup' ?
                            (<IStringRegexLookup> astNode).regexLoc : astNode.loc;

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
