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

[<Test>]
let ``astToJsAst - string lookup`` () =
    let expected = """
        {
            "type": "Program",
            "body": [
                {
                    "type": "ExpressionStatement",
                    "expression": {
                        "type": "MemberExpression",
                        "computed": true,
                        "object": {
                            "type": "CallExpression",
                            "callee": {
                                "type": "MemberExpression",
                                "computed": false,
                                "object": {
                                    "type": "Identifier",
                                    "name": "str"
                                },
                                "property": {
                                    "type": "Identifier",
                                    "name": "match"
                                }
                            },
                            "arguments": [
                                {
                                    "type": "Literal",
                                    "value": "/(fe?)/",
                                    "regex": {
                                        "pattern": "(fe?)",
                                        "flags": ""
                                    }
                                }
                            ]
                        },
                        "property": {
                            "type": "Literal",
                            "value": 1
                        }
                    }
                }
            ]
        }
    """

    let actual = 
        StringReLookup {
            lookupSource = Ident "str"
            regex = "(fe?)"
        }
        |> Some
        |> astToJsAst
        |> Option.get

    areJsonEquivalent expected actual

[<Test>]
let ``astToJsAst - boolean or`` () =
    let expected = """
        {
            "type": "Program",
            "body": [
                {
                    "type": "ExpressionStatement",
                    "expression": {
                        "type": "LogicalExpression",
                        "operator": "||",
                        "left": {
                            "type": "Identifier",
                            "name": "foo"
                        },
                        "right": {
                            "type": "Identifier",
                            "name": "bar"
                        }
                    }
                }
            ]
        }
    """
    
    let actual = 
        Bool {
            leftHandSide = Ident "foo"
            operator = Union
            rightHandSide = Ident "bar"
        }
        |> Some
        |> astToJsAst
        |> Option.get

    areJsonEquivalent expected actual

[<Test>]
let ``astToJsAst - boolean and`` () =
    let expected = """
        {
            "type": "Program",
            "body": [
                {
                    "type": "ExpressionStatement",
                    "expression": {
                        "type": "LogicalExpression",
                        "operator": "&&",
                        "left": {
                            "type": "Identifier",
                            "name": "foo"
                        },
                        "right": {
                            "type": "Identifier",
                            "name": "bar"
                        }
                    }
                }
            ]
        }
    """
    
    let actual = 
        Bool {
            leftHandSide = Ident "foo"
            operator = Intersection
            rightHandSide = Ident "bar"
        }
        |> Some
        |> astToJsAst
        |> Option.get

    areJsonEquivalent expected actual

[<Test>]
let ``astToJsAst - function declaration`` () =
    let expected = """
        {
            "type": "Program",
            "body": [
                {
                    "type": "VariableDeclaration",
                    "declarations": [
                        {
                            "type": "VariableDeclarator",
                            "id": {
                                "type": "Identifier",
                                "name": "f"
                            },
                            "init": {
                                "type": "FunctionExpression",
                                "id": {
                                    "type": "Identifier",
                                    "name": "f"
                                },
                                "params": [
                                    {
                                        "type": "Identifier",
                                        "name": "x"
                                    }
                                ],
                                "defaults": [],
                                "body": {
                                    "type": "BlockStatement",
                                    "body": [
                                        {
                                            "type": "ReturnStatement",
                                            "argument": {
                                                "type": "Literal",
                                                "value": "constant value"
                                            }
                                        }
                                    ]
                                },
                                "generator": false,
                                "expression": false
                            }
                        }
                    ],
                    "kind": "var"
                }
            ]
        }
    """
    
    let actual = 
        FunctionDecl {
            funcName = "f"
            argName = "x"
            body = Lit "constant value"
        }
        |> Some
        |> astToJsAst
        |> Option.get

    areJsonEquivalent expected actual

[<Test>]
let ``astToJsAst - expression list`` () =
    let expected = """
        {
            "type": "Program",
            "body": [
                {
                    "type": "ExpressionStatement",
                    "expression": {
                        "type": "CallExpression",
                        "callee": {
                            "type": "FunctionExpression",
                            "id": null,
                            "params": [],
                            "defaults": [],
                            "body": {
                                "type": "BlockStatement",
                                "body": [
                                    {
                                        "type": "ExpressionStatement",
                                        "expression": {
                                            "type": "Identifier",
                                            "name": "x"
                                        }
                                    },
                                    {
                                        "type": "ReturnStatement",
                                        "argument": {
                                            "type": "Identifier",
                                            "name": "y"
                                        }
                                    }
                                ]
                            },
                            "generator": false,
                            "expression": false
                        },
                        "arguments": []
                    }
                }
            ]
        }
    """
    
    let actual = 
        ExpressionList (Ident "x", Ident "y")
        |> Some
        |> astToJsAst
        |> Option.get

    areJsonEquivalent expected actual

let _ = """
{"type":"Program","body":[{"type":"ExpressionStatement","expression":{"type":"CallExpression","callee":{"type":"FunctionExpression","id":null,"params":[],"defaults":[],"body":{"type":"BlockStatement","body":[{"type":"Identifier","name":"x"},{"type":"ReturnStatement","argument":{"type":"Identifier","name":"y"}}]},"generator":false,"expression":false},"arguments":[]}}]}
"""
