### λScript – JS Version

In `../lambdaconf-talk`, I implemented the compiler in F#. Here, I'll do it in js.

#### Known Issues and Oddities
`tsd` keeps adding the following line to `typings/tsd.d.ts`:
 
```ts
/// <reference path="../node_modules/tsc/bin/typescript.d.ts" />
```
 
This causes `tsc` to fail. Removing the line causes it to succeed. I am not sure why that line is there, or
how I can make it not show up.
 
It appears that in `tsconfig.json`, the `files` array is relative to the file itself, but the `compilerOptions.out` 
field is not.

Why is a `.js` file created for each `.ts` file, even though I have the `out` flag set? Are these js files being
used instead of the ts files? Am I doing the imports wrong?

Why does adding `import logger = require('../../util/logger/index')` make the output be empty?

[`tsc` complains for new types added in ES6](https://github.com/borisyankov/DefinitelyTyped/issues/4249), 
so I applied to fix referenced in that issue and am using an older version of the nodejs `d.ts` file.

##### Difficulties using jison
* When your `.jison` files are invalid, the errors are not always helpful
* No static analysis is available for `.jison` files
    * You can have malformed js in your `.jison` file but it will still compile
* When your language can't be parsed, it's not obvious if it's during the tokenizing or grammar phase
* Docs are limited
* The debugger output is hard to understand
* It would be nice to get actual line and column numbers when parse errors occur.

##### Other notes
* Ideological rigidity (like wanting everything to be an expression) can make things harder.
* Although TS has taken some getting used to, it is generally helpful in solving errors at compile-time.
