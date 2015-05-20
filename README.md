# LambdaScript

This is my experiment in making a new programming language in F#, in preparation for my LambdaConf talk about how to make your own programming language.

### Running
You must have `npm` installed. You can get `npm` via [io.js](https://iojs.org/en/index.html).

### Missing Features
* Intelligible errors
* Type checking

### Known issues
* The tokenizer is too aggressive and will split a string literal with a space in it into multiple tokens. `"foo bar"` becomes `["\"foo"; "bar\""]`.
