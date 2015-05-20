module LambdaToJsTest

open NUnit.Framework
open LambdaToJs

(* Instead of js string comparison, it would be nice to compare ASTs so we don't worry about formatting. 
   Additionally, it would be nice to actually test the js that was output.
*)

[<Test>]
let ``lambdaToJs - function invocation`` () =
    let expected = "print(isPalindrome('racecar'));"
    let actual = lambdaToJs ["""print < isPalindrome "racecar" > """] |> Option.get

    Assert.AreEqual(expected, actual)

[<Test>]
let ``lambdaToJs - string lookup`` () =
    let expected = "str.match('ab(.)d')[1];"
    let actual = lambdaToJs ["str[/ab(.)d/]"] |> Option.get

    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``lambdaToJs - nested bool`` () =
    let expected = "foo || bar && baz;"
    let actual = lambdaToJs ["foo or < bar and baz >"] |> Option.get

    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``lambdaToJs - nested bool with or`` () =
    let expected = "(foo || bar) && baz;"
    let actual = lambdaToJs ["< foo or bar > and baz"] |> Option.get

    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``lambdaToJs - function declaration`` () =
    let expected = "function f(x) {\n    return 'retVal';\n}"
    let actual = lambdaToJs ["""λ f x . "retVal" """] |> Option.get

    Assert.AreEqual(expected, actual)

(* TODO there is a bug where `print "hello world"` does not compile correctly. 
   I suspect that the tokenizier is broken.
*)

[<Test>]
let ``lambdaToJs - print statement`` () =
    let expected = "var print = console.log.bind(console);\nprint('hello world');"
    let actual = lambdaToJs ["""print "hello" """] |> Option.get

    Assert.AreEqual(expected, actual)