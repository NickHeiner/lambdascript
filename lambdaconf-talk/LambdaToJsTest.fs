module LambdaToJsTest

open NUnit.Framework
open LambdaToJs
open ReadFile

(* Instead of js string comparison, it would be nice to compare ASTs so we don't worry about formatting. 
   Additionally, it would be nice to actually test the js that was output.

   These tests are ugly as shit.
   Any sin is acceptable given sufficient self-awareness.
*)

[<Test>]
let ``lambdaToJs - function invocation`` () =
    let expected = """
'use strict';

function stringLookup(str, regex) {
    var match = str.match(regex);
    if (match) {
        return match[1];
    }

    return '';
}

var print = console.log.bind(console);""" + "print(isPalindrome('racecar'));"
    let actual = lambdaToJs ["""print < isPalindrome "racecar" > """] |> Option.get

    Assert.AreEqual(expected, actual)

[<Test>]
let ``lambdaToJs - string lookup`` () =
    let expected = """
'use strict';

function stringLookup(str, regex) {
    var match = str.match(regex);
    if (match) {
        return match[1];
    }

    return '';
}

var print = console.log.bind(console);""" + "stringLookup(str, 'ab(.)d');"
    let actual = lambdaToJs ["str[/ab(.)d/]"] |> Option.get

    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``lambdaToJs - nested bool`` () =
    let expected = """
'use strict';

function stringLookup(str, regex) {
    var match = str.match(regex);
    if (match) {
        return match[1];
    }

    return '';
}

var print = console.log.bind(console);""" + "foo || bar && baz;"
    let actual = lambdaToJs ["foo or < bar and baz >"] |> Option.get

    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``lambdaToJs - nested bool with or`` () =
    let expected = """
'use strict';

function stringLookup(str, regex) {
    var match = str.match(regex);
    if (match) {
        return match[1];
    }

    return '';
}

var print = console.log.bind(console);""" + "(foo || bar) && baz;"
    let actual = lambdaToJs ["< foo or bar > and baz"] |> Option.get

    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``lambdaToJs - function declaration`` () =
    let expected = """
'use strict';

function stringLookup(str, regex) {
    var match = str.match(regex);
    if (match) {
        return match[1];
    }

    return '';
}

var print = console.log.bind(console);""" + "var f = function f(x) {\n    return 'retVal';\n};"
    let actual = lambdaToJs ["""λ f x . "retVal" """] |> Option.get

    Assert.AreEqual(expected, actual)

(* TODO there is a bug where `print "hello world"` does not compile correctly. 
   I suspect that the tokenizier is broken.
*)

[<Test>]
let ``lambdaToJs - print statement`` () =
    let expected = """
'use strict';

function stringLookup(str, regex) {
    var match = str.match(regex);
    if (match) {
        return match[1];
    }

    return '';
}

var print = console.log.bind(console);print('hello');"""
    let actual = lambdaToJs ["""print "hello" """] |> Option.get

    Assert.AreEqual(expected, actual)

[<Test>]
let ``lambdaToJs - expression list`` () =
    (* I am not sure why there are two ;; *)
    let expected = """
'use strict';

function stringLookup(str, regex) {
    var match = str.match(regex);
    if (match) {
        return match[1];
    }

    return '';
}

var print = console.log.bind(console);(function () {"""
                    + "\n    var f = function f(x) {\n        return x;\n    };;\n    return f('hello');\n}());"
    let actual = lambdaToJs ["""< λ f x . x >; f "hello" """] |> Option.get

    Assert.AreEqual(expected, actual)

[<Test>]
let ``lambdaToJs - sample`` () =
    let expected = "\r\n'use strict';\r\n\r\nfunction stringLookup(str, regex) {\r\n    var match = str.match(regex);\r\n    if (match) {\r\n        return match[1];\r\n    }\r\n\r\n    return '';\r\n}\r\n\r\nvar print = console.log.bind(console);(function () {\n    var isPalindrome = function isPalindrome(str) {\n        return str === '' || stringLookup(str, '^(.)') === stringLookup(str, '.*(.)$') && isPalindrome(stringLookup(str, '^.(.*).'));\n    };;\n    return function () {\n        print(isPalindrome('racecar'));\n        return print(isPalindrome('not-a-palindrome'));\n    }();\n}());"
    let actual = "..\..\sample.lambda"
                    |> GetFileContents
                    |> lambdaToJs
                    |> Option.get

    Assert.AreEqual(expected, actual)