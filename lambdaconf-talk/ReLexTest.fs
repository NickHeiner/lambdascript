module ReLexTest

open Lex
open ReLex
open NUnit.Framework

[<Test>]
let ``reLex - identity`` () =
    let expected = [OpenAngleBracket; Identifier "x"; CloseAngleBracket]
    let actual = reLex expected

    Assert.AreEqual(expected, actual)

[<Test>]
let ``reLex - func declaration`` () =
    let expected = [Lambda; FuncName "f"; ArgName "x"; FuncDot; Literal "literal value"]
    let actual = reLex [Lambda; Identifier "f"; Identifier "x"; FuncDot; Literal "literal value"]

    Assert.AreEqual(expected, actual)