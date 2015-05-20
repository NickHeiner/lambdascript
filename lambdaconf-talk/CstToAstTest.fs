module CstToAstTest

open NUnit.Framework
open Lex
open Grammar
open CstToAst

[<Test>]
let ``cstToAst - literal`` () =
    let expected = Lit "foo" |> Some

    let actual = 
        Expression [
            Leaf (Literal "foo")
        ]
        |> Some
        |> cstToAst

    Assert.AreEqual(expected, actual)

[<Test>]
let ``cstToAst - identifier`` () =
    let expected = Ident "param" |> Some

    let actual = 
        Expression [
            Leaf (Identifier "param")
        ]
        |> Some
        |> cstToAst

    Assert.AreEqual(expected, actual)

[<Test>]
let ``cstToAst - string lookup`` () =
    let expected = 
        StringReLookup {
            lookupSource = Lit "foo"
            regex = "^(f)?"
        } 
        |> Some

    let actual = 
        Expression [
            StringLookup [
                Expression [
                    Leaf (Literal "foo")
                ]
                Leaf OpenSquareBracket
                Leaf (RegexLiteral "^(f)?")
                Leaf CloseSquareBracket
            ]    
        ]
        |> Some
        |> cstToAst

    Assert.AreEqual(expected, actual)
   
[<Test>]
let ``cstToAst - boolean equals`` () =
    let expected = 
        Bool {
            leftHandSide = Lit ""
            operator = Equal
            rightHandSide = Ident "str"
        } |> Some

    let actual =
        Expression [
            Boolean [
                Expression [
                    Leaf (Literal "")
                ]
                Leaf Equality
                Expression [
                    Leaf (Identifier "str")
                ]
            ]
        ]
        |> Some
        |> cstToAst

    Assert.AreEqual(expected, actual)
   
[<Test>]
let ``cstToAst - boolean and`` () =
    let expected = 
        Bool {
            leftHandSide = Lit ""
            operator = Intersection
            rightHandSide = Ident "str"
        } |> Some

    let actual =
        Expression [
            Boolean [
                Expression [
                    Leaf (Literal "")
                ]
                Leaf And
                Expression [
                    Leaf (Identifier "str")
                ]
            ]
        ]
        |> Some
        |> cstToAst

    Assert.AreEqual(expected, actual)
   
[<Test>]
let ``cstToAst - boolean or`` () =
    let expected = 
        Bool {
            leftHandSide = Lit ""
            operator = Union
            rightHandSide = Ident "str"
        } |> Some

    let actual =
        Expression [
            Boolean [
                Expression [
                    Leaf (Literal "")
                ]
                Leaf Or
                Expression [
                    Leaf (Identifier "str")
                ]
            ]
        ]
        |> Some
        |> cstToAst

    Assert.AreEqual(expected, actual)
   
[<Test>]
let ``cstToAst - function invocation`` () =
    let expected = 
        FunctionInvoke {
            func = Ident "f"
            arg = Ident "x"
        } |> Some

    let actual =
        Expression [
            FuncInvocation [
                Expression [Leaf (Identifier "f")]
                Expression [Leaf (Identifier "x")]
            ]
        ]
        |> Some
        |> cstToAst

    Assert.AreEqual(expected, actual)

[<Test>]
let ``cstToAst - function declaration`` () =
    let expected = 
        FunctionDecl {
            funcName = "funcName"
            argName = "argName"
            body = Lit "always return this value"
        }
        |> Some

    let actual = 
        Expression [
            FuncDeclaration [
                Leaf Lambda
                Leaf (FuncName "funcName")
                Leaf (ArgName "argName")
                Leaf FuncDot
                Expression [
                    Leaf (Literal "always return this value")
                ]
            ]
        ]
        |> Some
        |> cstToAst

    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``cstToAst - expression list`` () =
    let expected = 
        ExpressionList (Ident "x", Ident "y")
        |> Some

    let actual = 
        Expression [
            Expression [
                Leaf (Identifier "x")
            ]
            Leaf ExpressionSep
            Expression [
                Leaf (Identifier "y")
            ]
        ]
        |> Some
        |> cstToAst

    Assert.AreEqual(expected, actual)