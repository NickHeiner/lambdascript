'use strict';

const test = require('tape'),
    path = require('path'),
    tmp = require('tmp'),
    q = require('q'),
    child_process = require('child_process'),
    lsc = require('../..');

test('lambdascript compiler in js', function(t) {
   function runTest(testFileName: string) {
       const testFilePath = path.resolve(__dirname, '..', 'fixtures', testFileName);

       return q.nfcall(tmp.file.bind(tmp))
           .then(function(jsOutputFilePath) {
               return lsc.compile(testFilePath, jsOutputFilePath)
                   .then(function() {
                       return q.nfcall(child_process.spawn.bind(child_process), 'node', jsOutputFilePath);
                   });
           }).then(function(stdout, stderr) {
               if (stderr) {
                   throw new Error(stderr);
               }

               return stdout;
           });
   }

    t.test('print boolean', function(t) {
        t.plan(1);
        runTest('print-boolean.lambda').then(function(stdout) {
            t.equal(stdout, 'true', 'boolean is printed correctly to standard out');
        });
    });
});
