module LambdaToJsTest

open NUnit.Framework
open LambdaToJs

(* Instead of js string comparison, it would be nice to compare ASTs so we don't worry about formatting. 
   Additionally, it would be nice to actually test the js that was output.
*)

[<Test>]
let ``lambdaToJs - function invocation`` () =
    let expected = "var print = console.log.bind(console);\nprint(isPalindrome('racecar'));"
    let actual = lambdaToJs ["""print < isPalindrome "racecar" > """] |> Option.get

    Assert.AreEqual(expected, actual)

[<Test>]
let ``lambdaToJs - string lookup`` () =
    let expected = "var print = console.log.bind(console);\nstr.match('ab(.)d')[1];"
    let actual = lambdaToJs ["str[/ab(.)d/]"] |> Option.get

    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``lambdaToJs - nested bool`` () =
    let expected = "var print = console.log.bind(console);\nfoo || bar && baz;"
    let actual = lambdaToJs ["foo or < bar and baz >"] |> Option.get

    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``lambdaToJs - nested bool with or`` () =
    let expected = "var print = console.log.bind(console);\n(foo || bar) && baz;"
    let actual = lambdaToJs ["< foo or bar > and baz"] |> Option.get

    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``lambdaToJs - function declaration`` () =
    let expected = "var print = console.log.bind(console);\nfunction f(x) {\n    return 'retVal';\n}"
    let actual = lambdaToJs ["""λ f x . "retVal" """] |> Option.get

    Assert.AreEqual(expected, actual)

(* TODO there is a bug where `print "hello world"` does not compile correctly. 
   I suspect that the tokenizier is broken.
*)

[<Test>]
let ``lambdaToJs - print statement`` () =
    let expected = "var print = console.log.bind(console);\nprint('hello');"
    let actual = lambdaToJs ["""print "hello" """] |> Option.get

    Assert.AreEqual(expected, actual)

[<Test>]
let ``lambdaToJs - expression list`` () =
    (* It's a little funky that there are parens around the function declaration. 
        I think that's because I'm declaring it as an expression statement. *)
    let expected = "var print = console.log.bind(console);
(function () {
    (function f(x) {
        return x;
    });
    return f('hello');
}());"
    let actual = lambdaToJs ["""< λ f x . x >; f "hello" """] |> Option.get

    Assert.AreEqual(expected, actual)