/// <reference path="./typings/tsd.d.ts" />

'use strict';

const q = require('q'),
    lambda = require('./lambda'),
    qFs = require('q-io/fs');

function lsc(inputLambdaScriptFile: string, outputJsFile: string) {
    return qFs.read(inputLambdaScriptFile).then(function(lambdaScriptCode: string) {
        const js = lambda.parser.parse(lambdaScriptCode);
        return qFs.write(outputJsFile, js);
    });
}

// Why is this necessary?
module.exports = lsc;
