module JsAstToJsTest

open NUnit.Framework
open JsAstToJs

[<Test>]
let ``jsAstToJs - simple`` () =
    let actual = jsAstToJs ""
    Assert.AreEqual(2, actual)