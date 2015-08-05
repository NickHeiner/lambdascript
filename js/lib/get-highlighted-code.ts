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


// One potential way to fail fast here would be to detect when we are coloring over an existing highlight.
// I have yet to encounter a case where that was expected behavior.

function getLineColorAction(lines: string[], loc: ILoc, colorFn: (str: string) => string): string[] {
    if (loc.first_line !== loc.last_line) {
        // We do not support syntax highlighting across lines for now.
        return lines;
    }

    // I don't know why but jison does not 0 index the line number.
    const lineIndex = loc.first_line - 1,

        linesClone = _.clone(lines),
        lineToColor = linesClone[lineIndex],

        existingColorOffset = lineToColor.length - chalk.stripColor(lineToColor).length,

        colStart = loc.first_column + existingColorOffset,
        colEnd = loc.last_column + existingColorOffset;

    linesClone[lineIndex] =
        lineToColor.slice(0, colStart) +
        colorFn(lineToColor.slice(colStart, colEnd)) +
        lineToColor.slice(colEnd);

    logger.debug({
        existingColorOffset: existingColorOffset,
        original: lines[lineIndex],
        colored: linesClone[lineIndex],
        lineIndex: lineIndex,
        colStart: colStart,
        colEnd: colEnd
    }, 'Coloring line portion');

    return linesClone;
}

function getHighlightedCode(lscAst: ILambdaScriptAstNode, lambdaScriptCode: string) {
    const codeByLines = _str.lines(lambdaScriptCode),
        coloredCode = traverse(lscAst).reduce(function(acc: string[], astNode: ILambdaScriptAstNode) {
            const color = colorMap[astNode.type];

            if (color) {
                const colorFn = chalk[color].bind(chalk),
                    loc = astNode.type === 'StringRegexLookup' ? (<IStringRegexLookup> astNode).regexLoc : astNode.loc;

                logger.debug({astNode: astNode}, 'Coloring node');

                return getLineColorAction(acc, loc, colorFn);
            }

            return acc;
        }, codeByLines);

    return coloredCode.join(os.EOL);
}

export = getHighlightedCode;
