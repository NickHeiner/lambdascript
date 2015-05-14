module LambdaToJsTest

open NUnit.Framework
open LambdaToJs

[<Test>]
let ``lambdaToJs - method call`` () =
    let expected = "print(isPalindrome('racecar'));"
    let actual = lambdaToJs ["""print < isPalindrome "racecar" > """] |> Option.get

    Assert.AreEqual(expected, actual)

[<Test>]
let ``lambdaToJs - string lookup`` () =
    let expected = "str.match('ab(.)d')[1];"
    let actual = lambdaToJs ["str[/ab(.)d/]"] |> Option.get

    Assert.AreEqual(expected, actual)
