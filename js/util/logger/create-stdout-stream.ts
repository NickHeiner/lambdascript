'use strict';

var through2 = require('through2'),
    chalk = require('chalk'),
    bunyanFormat = require('bunyan-format'),
    stripAnsi = require('strip-ansi'),
    _ = require('lodash');

module.exports = createStdoutStream;

function createStdoutStream(out, logFormat) {
    const tapSafeLogOutput = through2(function(chunk, enc, callback) {
            // All log records always end in a newline, so we want to strip
            // it off pre-prefixing and add it back afterwards.
            const lines = stripAnsi(chunk.toString()).trim().split('\n'),
                prefixedLines = _.map(lines, function(line) {
                        return '# ' + chalk.grey('LOG: ' + line);
                    }).join('\n') + '\n';

            callback(null, prefixedLines);
        }),
        outputStream = process.env.TAP === '1' ? tapSafeLogOutput : out,
        formattedStream = bunyanFormat({outputMode: logFormat}, outputStream);

    tapSafeLogOutput.pipe(out);

    return formattedStream;
}
