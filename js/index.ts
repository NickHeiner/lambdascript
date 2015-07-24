/// <reference path="./typings/tsd.d.ts" />

'use strict';

import astToJsAst = require('./lib/ast-to-js-ast');

const q = require('q'),
    lambda = require('./lambda'),
    logger = require('./util/logger'),
    escodegen = require('escodegen'),
    qFs = require('q-io/fs');

function lsc(inputLambdaScriptFile: string, outputJsFile: string) {
    return qFs.read(inputLambdaScriptFile).then(function(lambdaScriptCode: string) {
        const jisonOutput = lambda.parser.parse(lambdaScriptCode);
        logger.debug({jisonOutput: jisonOutput}, 'Parsed LambdaScript code');

        const jsAst = astToJsAst(jisonOutput);
        logger.debug({jsAst: jsAst}, 'Converted LambdaScript AST to JS AST');

        const js = escodegen.generate(jsAst);
        logger.debug({js: js}, 'Generated js');

        return qFs.write(outputJsFile, js);
    });
}

// Why is this necessary?
module.exports = lsc;
