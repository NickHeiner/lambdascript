/// <reference path="./typings/tsd.d.ts" />

'use strict';

import './types';
import astToJsAst = require('./lib/ast-to-js-ast');
import withPrelude = require('./lib/with-prelude');
import getHighlightedCode = require('./lib/get-highlighted-code');

const lambda = require('./lambda'),
    logger = require('./util/logger'),
    escodegen = require('escodegen'),
    qFs = require('q-io/fs');

function getLscAst(lambdaScriptCode: string): ILambdaScriptAstNode {
    logger.debug({lambdaScriptCode: lambdaScriptCode}, 'Parsing LambdaScript code');

    const lscAst = lambda.parser.parse(lambdaScriptCode);
    logger.debug({lscAst: lscAst}, 'Parsed LambdaScript code');

    return lscAst;
}

function lsc(inputLambdaScriptFile: string, outputJsFile: string) {
    return qFs.read(inputLambdaScriptFile).then(function(lambdaScriptCode: string) {
        const lscAst = getLscAst(lambdaScriptCode),
            jsAst = astToJsAst(lscAst);
        logger.debug({jsAst: jsAst}, 'Converted LambdaScript AST to JS AST');

        const js = escodegen.generate(jsAst);
        logger.debug({js: js}, 'Generated raw js');

        const jsWithPrelude = withPrelude(js);
        logger.debug({jsWithPrelude: jsWithPrelude}, 'Added prelude to raw js');

        return qFs.write(outputJsFile, jsWithPrelude);
    });
}

function lscHighlight(inputLambdaScriptFile: string): Q.IPromise<string> {
    return qFs.read(inputLambdaScriptFile).then(function(lambdaScriptCode: string) {
        const lscAst = getLscAst(lambdaScriptCode);
        return getHighlightedCode(lscAst, lambdaScriptCode);
    });
}

module.exports = lsc;
module.exports.highlight = lscHighlight;
