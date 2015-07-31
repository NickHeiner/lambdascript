function withPrelude(js: string): string {
    return `'use strict';

var print = console.log.bind(console);

function __stringRegexLookup(str, regex) {
    var match = str.match(regex);
    if (match) {
        return match[1];
    }

    return '';
}

${js}
`;
}

export = withPrelude;
