module AstToJsAstTest

open NUnit.Framework
open CstToAst
open AstToJsAst
open Newtonsoft.Json

let areJsonEquivalent expected actual =
    Assert.AreEqual(JsonConvert.DeserializeObject expected, JsonConvert.DeserializeObject actual)

[<Test>]
let ``astToJsAst - literal`` () =
    let expected = """
        {
            "type": "Program",
            "body": [
                {
                    "type": "ExpressionStatement",
                    "expression": {
                        "type": "Literal",
                        "value": "hi"
                    }
                }
            ]
        }
    """
    
    let actual = 
        Lit "hi"
        |> Some
        |> astToJsAst
        |> Option.get

    areJsonEquivalent expected actual

[<Test>]
let ``astToJsAst - identifier`` () =
    let expected = """
        {
            "type": "Program",
            "body": [
                {
                    "type": "ExpressionStatement",
                    "expression": {
                        "type": "Identifier",
                        "value": "identifier"
                    }
                }
            ]
        }
    """
    
    let actual = 
        Ident "identifier"
        |> Some
        |> astToJsAst
        |> Option.get

    areJsonEquivalent expected actual

[<Test>]
let ``astToJsAst - function invocation with identifier`` () =
    let expected = """
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

    let actual = 
        FunctionInvoke {
            func = Ident "f"
            arg = Ident "x"
        }
        |> Some
        |> astToJsAst
        |> Option.get

    areJsonEquivalent expected actual

[<Test>]
let ``astToJsAst - function invocation with argument as expression`` () =
    let expected = """
        {
        "type": "Program",
        "body": [
            {
                "type": "ExpressionStatement",
                "expression": {
                    "type": "CallExpression",
                    "callee": {
                        "type": "Identifier",
                        "name": "outer"
                    },
                    "arguments": [
                        {
                            "type": "CallExpression",
                            "callee": {
                                "type": "Identifier",
                                "name": "innter"
                            },
                            "arguments": [
                                {
                                    "type": "Identifier",
                                    "name": "argName"
                                }
                            ]
                        }
                    ]
                }
            }
        ]
    }
    """

    let actual = 
        FunctionInvoke {
            func = Ident "outer"
            arg = FunctionInvoke {
                func = Ident "inner"
                arg = Ident "argName"
            }
        }
        |> Some
        |> astToJsAst
        |> Option.get

    areJsonEquivalent expected actual