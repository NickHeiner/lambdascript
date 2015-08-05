import '../types';

const chalk = require('chalk'),
    _str = require('underscore.string'),
    traverse = require('traverse'),
    os = require('os'),

    colorMap: IStringMap = {
        Identifier: 'cyan'
    };

interface IStringMap {
    [key: string]: string;
}

function getHighlightedCode(lscAst: ILambdaScriptAstNode, lambdaScriptCode: string) {
    const codeByLines = _str.lines(lambdaScriptCode),
        coloredCode = traverse(lscAst).reduce(function(acc: string, astNode: ILambdaScriptAstNode) {
            const color = colorMap[astNode.type];

            if (color) {
                const colorFn = chalk[color];
                console.log(colorFn('color'));
            }

            return acc;
        }, codeByLines);

    return coloredCode.join(os.EOL);
}

export = getHighlightedCode;
