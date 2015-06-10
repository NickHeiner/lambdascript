# LambdaScript

This is my experiment in making a new programming language in F#, in preparation for my talk about how to make your own programming language.

### Running
You must have `npm` installed. You can get `npm` via [io.js](https://iojs.org/en/index.html).

### Missing Features
* Intelligible errors
* Type checking

### Known issues
* The tokenizer is too aggressive and will split a string literal with a space in it into multiple tokens. `"foo bar"` becomes `["\"foo"; "bar\""]`.
* This got seriously hacky as I was getting down towards the end of the time I wanted to spend.
* The language itself is a little funky.
