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
    };

interface IStringMap {
    [key: string]: string;
}

interface IHighlightErr extends Error {
    astNode: ILambdaScriptAstNode;
}

function getHighlightedCode(lscAst: ILambdaScriptAstNode, lambdaScriptCode: string) {
    const codeByLines = _str.lines(lambdaScriptCode),
        coloredCode = traverse(lscAst).reduce(function(acc: string, astNode: ILambdaScriptAstNode) {
            const color = colorMap[astNode.type];

            if (color) {
                const colorFn = chalk[color].bind(chalk);

                if (!astNode.loc) {
                    const err = <IHighlightErr> new Error(
                        'A node in the AST was found without the loc property whose type exists in the color mapping.'
                    );
                    err.astNode = astNode;
                    throw err;
                }

                if (astNode.loc.first_line !== astNode.loc.last_line) {
                    // We do not support syntax highlighting across lines for now.
                    return acc;
                }

                // I don't know why but jison does not 0 index the line number.
                const lineIndex = astNode.loc.first_line - 1,

                    accClone = _.clone(acc),
                    lineToColor = accClone[lineIndex],

                    existingColorOffset = lineToColor.length - chalk.stripColor(lineToColor).length,

                    colStart = astNode.loc.first_column + existingColorOffset,
                    colEnd = astNode.loc.last_column + existingColorOffset;

                accClone[lineIndex] =
                    lineToColor.slice(0, colStart) +
                    colorFn(lineToColor.slice(colStart, colEnd)) +
                    lineToColor.slice(colEnd);

                logger.debug({
                    astNode: astNode,
                    existingColorOffset: existingColorOffset,
                    original: acc[lineIndex],
                    colored: accClone[lineIndex],
                    lineIndex: lineIndex,
                    colStart: colStart,
                    colEnd: colEnd
                }, 'Coloring line portion');

                return accClone;
            }

            return acc;
        }, codeByLines);

    return coloredCode.join(os.EOL);
}

export = getHighlightedCode;
