﻿module LexTest

open Lex
open NUnit.Framework

[<Test>]
let ``lex - no input`` () = 
    Assert.AreEqual(lex [], [])

[<Test>]
let ``lex - some input`` () = 
    let actual = lex ["λ"; "x"; "."; "1"]
    let expected = [
        Lambda;
        Identifier "x";
        FuncDot;
        Identifier "1"
    ]
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``lex - different identifier`` () = 
    let actual = lex ["λ"; "f"; "."; "1"]
    let expected = [
        Lambda;
        Identifier "f";
        FuncDot;
        Identifier "1"
    ]
    
    Assert.AreEqual(expected, actual)

[<Test>]
let ``lex - Lambda`` () =
    Assert.AreEqual([Lambda], lex ["λ"])
    
[<Test>]
let ``lex - Identifier`` () =
    Assert.AreEqual([Identifier "isPalindrome"], lex ["isPalindrome"])

[<Test>]
let ``lex - FuncDot`` () =
    Assert.AreEqual([FuncDot], lex ["."])

[<Test>]
let ``lex - Literal`` () =
    Assert.AreEqual([Literal "racecar"], lex ["\"racecar\""])

[<Test>]
let ``lex - OpenAngleBracket`` () =
    Assert.AreEqual([OpenAngleBracket], lex ["<"])

[<Test>]
let ``lex - CloseAngleBracket`` () =
    Assert.AreEqual([CloseAngleBracket], lex [">"])

[<Test>]
let ``lex - OpenSquareBracket`` () =
    Assert.AreEqual([OpenSquareBracket], lex ["["])

[<Test>]
let ``lex - CloseSquareBracket`` () =
    Assert.AreEqual([CloseSquareBracket], lex ["]"])