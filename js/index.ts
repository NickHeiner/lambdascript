/// <reference path="./typings/tsd.d.ts" />

'use strict';

import astToJsAst = require('./lib/ast-to-js-ast');
import withPrelude = require('./lib/with-prelude');

const lambda = require('./lambda'),
    logger = require('./util/logger'),
    escodegen = require('escodegen'),
    qFs = require('q-io/fs');

function lsc(inputLambdaScriptFile: string, outputJsFile: string) {
    return qFs.read(inputLambdaScriptFile).then(function(lambdaScriptCode: string) {
        logger.debug({lambdaScriptCode: lambdaScriptCode}, 'Read LambdaScript code from file system');

        const jisonOutput = lambda.parser.parse(lambdaScriptCode);
        logger.debug({jisonOutput: jisonOutput}, 'Parsed LambdaScript code');

        const jsAst = astToJsAst(jisonOutput);
        logger.debug({jsAst: jsAst}, 'Converted LambdaScript AST to JS AST');

        const js = escodegen.generate(jsAst);
        logger.debug({js: js}, 'Generated raw js');

        const jsWithPrelude = withPrelude(js);
        logger.debug({jsWithPrelude: jsWithPrelude}, 'Generated raw js');

        return qFs.write(outputJsFile, jsWithPrelude);
    });
}

module.exports = lsc;
