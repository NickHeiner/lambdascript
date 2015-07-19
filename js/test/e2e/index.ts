'use strict';

const test = require('tape'),
    path = require('path'),
    tmp = require('tmp'),
    q = require('q'),
    lsc = require('../..');

test('lambdascript compiler in js', function(t) {
   function runTest(testFileName: string) {
       const testFilePath = path.resolve(__dirname, '..', 'fixtures', testFileName);
   }
});
