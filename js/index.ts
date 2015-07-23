/// <reference path="./typings/tsd.d.ts" />

'use strict';

import astToJsAst = require('./lib/ast-to-js-ast');
import jisonOutputToLambdaScriptAst = require('./lib/jison-output-to-lambda-script-ast');

const q = require('q'),
    lambda = require('./lambda'),
    escodegen = require('escodegen'),
    qFs = require('q-io/fs');

function lsc(inputLambdaScriptFile: string, outputJsFile: string) {
    return qFs.read(inputLambdaScriptFile).then(function(lambdaScriptCode: string) {
        const jisonOutput = lambda.parser.parse(lambdaScriptCode),
            lambdaScriptAst = jisonOutputToLambdaScriptAst(jisonOutput),
            jsAst = astToJsAst(lambdaScriptAst),
            js = escodegen.generate(jsAst);

        return qFs.write(outputJsFile, js);
    });
}

// Why is this necessary?
module.exports = lsc;
