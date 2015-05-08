module BottomUpParseTest

open Lex
open Grammar
open BottomUpParse
open NUnit.Framework

let sampleTree = 
    Expression [
        FuncDeclaration [
            Leaf Lambda; 
            Leaf (Identifier "x"); 
            Leaf FuncDot; 
            Expression [
                Leaf (Literal "1")
            ]
        ]
    ]

[<Test>]
let ``bottomUpParse - sample``() = 
    let actual = bottomUpParse [
                                Lambda;
                                Identifier "x";
                                FuncDot;
                                Literal "1"
                               ]
    let expected = Some sampleTree
    Assert.AreEqual(expected, actual)

[<Test>]
let ``bottomUpParse - invalid input``() = 
    let actual = bottomUpParse [
                                Lambda;
                                Identifier "f";
                                Identifier "illegalDuplicatedIdentifier";
                                FuncDot;
                                Literal "1"
                               ]
    Assert.AreEqual(None, actual)