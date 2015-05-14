module JsAstToJsTest

open NUnit.Framework
open JsAstToJs

[<Test>]
let ``jsAstToJs - literal`` () =
    let actual = 
        """{"type": "ExpressionStatement", "expression": {"type": "Literal", "value": "string literal"}}"""
        |> Some
        |> jsAstToJs
        |> Option.get

    Assert.AreEqual("'string literal';", actual)

[<Test>]
let ``jsAstToJs - function call`` () =
    let actual = 
        """
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
        |> Some
        |> jsAstToJs
        |> Option.get

    Assert.AreEqual("f(x);", actual)
