'use strict';

const nconf = require('nconf'),
    _ = require('lodash'),
    path = require('path'),
    flat = require('flat'),
    minimist = require('minimist'),
    traverse = require('traverse'),
    ENV_DELIMITER = '__',
    ENV_DELIMITER_REGEX = new RegExp('.+' + ENV_DELIMITER + '.+');

export = createConfig(process.argv);

/**
 * @description Create configuration
 *
 * @param {string[]} argv command-line arguments
 * @returns {object} configuration object
 */
function createConfig(rawArgv: string[]) {
    const config = new nconf.Provider(),
        parsedArgv = flat.unflatten(minimist(rawArgv), {delimiter: ':'}),
        envWhitelist = _(process.env)
            .keys()
            .filter(function(key: string) {
                return ENV_DELIMITER_REGEX.test(key) && !/^npm_config/.test(key);
            })
            .valueOf()
            .concat(['loglevel']);

    config.add('argv', {type: 'literal', store: parsedArgv});

    config.env({
        separator: ENV_DELIMITER,
        whitelist: envWhitelist
    });

    return function get(key: string, defaultValue?: any): any {
        var result = coerceValues(config.get(key));

        if (_.isUndefined(result)) {
            result = defaultValue;
        }

        return result;
    };
}

function coerceValues(configData: any) {
    return traverse(configData).forEach(function(value: any) {
        if (this.isLeaf && !_.isUndefined(value)) {
            // Allow env vars to define boolean values
            if (value === 'true') {
                this.update(true);
            }
            else if (value === 'false') {
                this.update(false);
            }
            else if (value === 'undefined') {
                this.update(void 0);
            }
        }
    });
}
