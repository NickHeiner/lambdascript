/// <reference path="../../typings/tsd.d.ts" />

'use strict';

import logger = require('../../util/logger');

const test = require('tape'),
    path = require('path'),
    tmp = require('tmp'),
    q = require('q'),
    child_process = require('child_process'),

    // we want to nodejs-import rather than require this because we want to test the compiled js
    // that would be published, not the js compiled as part of this same test bundle with potentially
    // different settings.
    lsc: any = require('../..');

test('lambdascript compiler in js', function(t: any) {
    function runTest(testFileName: string) {
        const testFilePath = path.resolve(__dirname, '..', 'fixtures', testFileName);

        logger.info({testFilePath: testFilePath}, 'Running test file');

        return q.nfcall(tmp.file.bind(tmp))
            .then(function(jsOutputFilePath: string) {
                logger.info({jsOutputFilePath: jsOutputFilePath}, 'Compiling to js output');
                return lsc(testFilePath, jsOutputFilePath)
                    .then(function() {
                        logger.info('Spawning node on generated js');
                        return q.nfcall(child_process.spawn.bind(child_process), 'node', jsOutputFilePath);
                    });
            }).then(function(stdout: string, stderr: string) {
                logger.info({stderr: stderr, stdout: stdout}, 'Nodejs spawn complete');
                if (stderr) {
                   throw new Error(stderr);
                }

                return stdout;
            });
    }

    function stringEqual(t: any, str1: string, str2: string, message: string): void {
        t.equal(str1, str2, message);
    }

    t.test('print boolean', function(t: any) {
        t.plan(1);
        runTest('print-string.lambda').then(function(stdout: string) {
            stringEqual(t, stdout, 'hello-world', 'string is printed correctly to standard out');
        }).then(t.error);
    });
});
