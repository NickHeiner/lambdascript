'use strict';

import createStdoutStream = require('./create-stdout-stream');
import config = require('../config/index');

const bunyan = require('bunyan'),
    chalk = require('chalk');

chalk.enabled = true;

export = createLogger(process.stdout);

/**
 * Centralized logging configuration.
 *
 * @param {stream} out - a stream to write log records to.
 * @returns {object} log - an instance of a bunyan logger.
 */
function createLogger(out: NodeJS.WritableStream) {
    const streams = [{
        name: 'stdout',
        stream: createStdoutStream(out, 'short'),
        level: config('loglevel')
    }];

    return bunyan.createLogger({
        name: 'lsc',
        serializers: bunyan.stdSerializers,
        src: false,
        streams: streams
    });
}
