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
let ``bottomUpParse - sample`` () = 
    let actual = bottomUpParse [
                                Lambda;
                                Identifier "x";
                                FuncDot;
                                Literal "always return this string"
                               ]
    let expected = Some sampleTree
    Assert.AreEqual(expected, actual)

[<Test>]
let ``bottomUpParse - invalid input`` () = 
    let actual = bottomUpParse [
                                Lambda
                                Identifier "f"
                                Identifier "illegalDuplicatedIdentifier"
                                FuncDot
                                Literal "1"
                               ]
    Assert.AreEqual(None, actual)

[<Test>]
let ``bottomUpParse - literal`` () =
    (* Parsing:
            "racecar"
    *)
    let actual = 
        bottomUpParse [
            Literal "racecar"
        ]

    let expected = 
        Expression [Leaf (Literal "racecar")]
        |> Some

    Assert.AreEqual(expected, actual)

[<Test>]
let ``bottomUpParse - function invocation`` () =
    (* Parsing:
            isPalindrome "racecar"
    *)
    let actual = 
        bottomUpParse [
            Identifier "isPalindrome"
            Literal "racecar"
        ]

    let expected = 
        Expression [
            FuncInvocation [
                Expression [
                    Leaf (Identifier "isPalindrome")
                ]
                Expression [
                    Leaf (Literal "racecar")
                ]
            ]
        ]
        |> Some

    Assert.AreEqual(expected, actual)

[<Test>]
let ``bottomUpParse - nested function invocation`` () =
    (* Parsing:
            print <isPalindrome "racecar"> 
    *)
    let actual = 
        bottomUpParse [
            Identifier "print"
            OpenAngleBracket
            Identifier "isPalindrome"
            Literal "racecar"
            CloseAngleBracket
        ]

    let expected = 
        Expression [
            FuncInvocation [
                Leaf (Identifier "print")
                FuncInvocation [
                    Leaf (Identifier "isPalindrome")
                    Leaf (Literal "racecar")
                ]
            ]
        ]
        |> Some

    Assert.AreEqual(expected, actual)