module LexTest

open Lex
open NUnit.Framework

[<Test>]
let ``lex - no input``() = 
    Assert.AreEqual(lex [], [])

[<Test>]
let ``lex - some input``() = 
    let actual = lex ["λ"; "x"; "."; "1"]
    let expected = [
        Lambda;
        Identifier "x";
        FuncDot;
        Identifier "1"
    ]
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``lex - different identifier``() = 
    let actual = lex ["λ"; "f"; "."; "1"]
    let expected = [
        Lambda;
        Identifier "f";
        FuncDot;
        Identifier "1"
    ]
    
    Assert.AreEqual(expected, actual)
