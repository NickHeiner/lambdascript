module CstToAstTest

open NUnit.Framework
open Lex
open Grammar
open CstToAst

[<Test>]
let ``cstToAst - string lookup`` () =
    let expected = 
        StringReLookup {
            string = Leaf (Literal "foo")
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
   