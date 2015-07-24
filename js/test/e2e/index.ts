/// <reference path="../../typings/tsd.d.ts" />

'use strict';

 import logger = require('../../util/logger/index');

const test = require('tape'),
    path = require('path'),
    tmp = require('tmp'),
    q = require('q'),
    child_process = require('child_process'),

    // we want to nodejs-import rather than require this because we want to test the compiled js
    // that would be published, not the js compiled as part of this same test bundle with potentially
    // different settings.
    lsc = require('../..');

test('lambdascript compiler in js', function(t: any) {
    function runTest(testFileName: string) {
        const testFilePath = path.resolve(__dirname, '..', 'fixtures', testFileName);

        logger.info({testFilePath: testFilePath}, 'Running test file');

        return q.nfcall(tmp.file.bind(tmp), {postfix: '.js'})
            .spread(function(jsOutputFilePath: string) {
                logger.info({jsOutputFilePath: jsOutputFilePath, lsc: lsc}, 'Compiling to js output');
                return lsc(testFilePath, jsOutputFilePath)
                    .then(function() {
                        const command = `node ${jsOutputFilePath}`;
                        logger.info({command: command}, 'Spawning node on generated js');
                        return q.nfcall(child_process.exec.bind(child_process), command);
                    });
            }).spread(function(stdout: Buffer, stderr: Buffer) {
                if (stderr.length) {
                   throw new Error(stderr.toString());
                }

                logger.info({stdout: stdout.toString()}, 'Nodejs spawn complete');
                return stdout.toString();
            });
    }

    function stringEqual(t: any, str1: string, str2: string, message: string): void {
        t.equal(str1, str2, message);
    }

    t.test('print boolean', function(t: any) {
        t.plan(1);
        runTest('print-string.lambda').then(function(stdout: string) {
            stringEqual(t, stdout, 'hello-world\n', 'string is printed correctly to standard out');
        }).catch(function(err: any) {
            t.error(err);
            throw err;
        });
    });
});
