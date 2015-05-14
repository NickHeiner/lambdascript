module JsAstToJsTest

open NUnit.Framework
open JsAstToJs

[<Test>]
let ``jsAstToJs - literal`` () =
    let json = """{"type": "ExpressionStatement", "expression": {"type": "Literal", "value": "string literal"}}"""
    let actual = jsAstToJs json
    Assert.AreEqual("'string literal';", actual)

[<Test>]
let ``jsAstToJs - function call`` () =
    let json = """
        {
        "type": "Program",
        "body": [
            {
                "type": "ExpressionStatement",
                "expression": {
                    "type": "CallExpression",
                    "callee": {
                        "type": "Identifier",
                        "name": "f"
                    },
                    "arguments": [
                        {
                            "type": "Identifier",
                            "name": "x"
                        }
                    ]
                }
            }
        ]
    }
    """

    let actual = jsAstToJs json
    Assert.AreEqual("f(x);", actual)
