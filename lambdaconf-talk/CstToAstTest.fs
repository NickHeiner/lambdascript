module CstToAstTest

open NUnit.Framework
open Lex
open Grammar
open CstToAst

[<Test>]
let ``cstToAst - literal`` () =
    let expected = Lit "foo"

    let actual = 
        Expression [
            Leaf (Literal "foo")
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
   