### λScript – JS Version

In `../lambdaconf-talk`, I implemented the compiler in F#. Here, I'll do it in js.

#### Known Issues and Oddities
`tsd` keeps adding the following line to `typings/tsd.d.ts`:
 
 ```ts
 /// <reference path="../node_modules/tsc/bin/typescript.d.ts" />
 ```
 
 This causes `tsc` to fail. Removing the line causes it to succeed. I am not sure why that line is there, or
 how I can make it not show up.

[`tsc` complains for new types added in ES6](https://github.com/borisyankov/DefinitelyTyped/issues/4249), 
so I applied to fix referenced in that issue and am using an older version of the nodejs `d.ts` file.
