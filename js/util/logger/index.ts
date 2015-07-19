'use strict';

import createStdoutStream = require('./create-stdout-stream');
import config = require('../config');

const bunyan = require('bunyan'),
    chalk = require('chalk');

chalk.enabled = true;

module.exports = createLogger(process.stdout);

/**
 * Centralized logging configuration.
 *
 * @param {stream} out - a stream to write log records to.
 * @returns {object} log - an instance of a bunyan logger.
 */
function createLogger(out) {
    const streams = [{
        name: 'stdout',
        stream: createStdoutStream(out, 'short'),
        level: config.get('loglevel')
    }];

    return bunyan.createLogger({
        name: 'lsc',
        serializers: bunyan.stdSerializers,
        src: false,
        streams: streams
    });
}
