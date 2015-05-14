module JsAstToJsTest

open NUnit.Framework
open JsAstToJs

[<Test>]
let ``jsAstToJs - literal`` () =
    let json = """{"type": "ExpressionStatement", "expression": {"type": "Literal", "value": "string literal"}}"""
    let actual = jsAstToJs json
    Assert.AreEqual("'string literal';", actual)