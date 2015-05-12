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
   