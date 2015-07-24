function withPrelude(js: string): string {
    return `'use strict';

var print = console.log.bind(console);

${js}
`;
}

export = withPrelude;
