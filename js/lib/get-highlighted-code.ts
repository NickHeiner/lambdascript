import '../types';

const chalk = require('chalk'),
    _str = require('underscore.string'),
    _ = require('lodash'),
    traverse = require('traverse'),
    os = require('os'),

    colorMap: IStringMap = {
        Identifier: 'cyan'
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

                const accClone = _.clone(acc),
                    lineToColor = accClone[astNode.loc.first_line];

                accClone[astNode.loc.first_line] = colorFn(lineToColor);

                return accClone;
            }

            return acc;
        }, codeByLines);

    return coloredCode.join(os.EOL);
}

export = getHighlightedCode;
