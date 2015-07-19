/// <reference path="../../typings/tsd.d.ts" />

'use strict';

const test = require('tape'),
    path = require('path'),
    tmp = require('tmp'),
    q = require('q'),
    child_process = require('child_process'),
    lsc = require('../..');

test('lambdascript compiler in js', function(t: any) {
    function runTest(testFileName: string) {
        const testFilePath = path.resolve(__dirname, '..', 'fixtures', testFileName);

        return q.nfcall(tmp.file.bind(tmp))
            .then(function(jsOutputFilePath: string) {
               return lsc(testFilePath, jsOutputFilePath)
                   .then(function() {
                       return q.nfcall(child_process.spawn.bind(child_process), 'node', jsOutputFilePath);
                   });
            }).then(function(stdout: string, stderr: string) {
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
        runTest('print-boolean.lambda').then(function(stdout: string) {
            stringEqual(t, stdout, 'true', 'boolean is printed correctly to standard out');
        });
    });
});
