module BottomUpParseTest

open Lex
open Grammar
open BottomUpParse
open NUnit.Framework

[<Test>]
let ``bottomUpParse - function declaration`` () = 
    let actual = bottomUpParse [
                                Lambda
                                FuncName "x"
                                ArgName "ignoredArg"
                                FuncDot
                                Literal "always return this string"
                               ]
    let expected = 
        Expression [
            FuncDeclaration [
                Leaf Lambda
                Leaf (FuncName "x")
                Leaf (ArgName "ignoredArg")
                Leaf FuncDot
                Expression [
                    Leaf (Literal "always return this string")
                ]
            ]
        ]
        |> Some

    Assert.AreEqual(expected, actual)

[<Test>]
let ``bottomUpParse - invalid input`` () = 
    let actual = bottomUpParse [
                                Lambda
                                Identifier "notExpectingAnIdHere"
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
let ``bottomUpParse - bracketed function invocation`` () =
    (* Parsing:
            <isPalindrome "racecar"> 
    *)
    let actual = 
        bottomUpParse [
            OpenAngleBracket
            Identifier "isPalindrome"
            Literal "racecar"
            CloseAngleBracket
        ]

    let expected = 
        Expression [
            Leaf (OpenAngleBracket)
            Expression [
                FuncInvocation [
                    Expression [Leaf (Identifier "isPalindrome")]
                    Expression [Leaf (Literal "racecar")]
                ]
            ]
            Leaf (CloseAngleBracket)
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
                Expression [Leaf (Identifier "print")]
                Expression [
                    Leaf OpenAngleBracket
                    Expression [
                        FuncInvocation [
                            Expression [Leaf (Identifier "isPalindrome")]
                            Expression [Leaf (Literal "racecar")]
                        ]
                    ]
                    Leaf CloseAngleBracket
                ]
            ]
        ]
        |> Some

    Assert.AreEqual(expected, actual)

[<Test>]
let ``bottomUpParse - boolean`` () =
    let actual = 
        bottomUpParse [
            Literal "foo"
            Equality
            Literal "bar"
        ]
    let expected = 
        Expression [
            Boolean [
                Expression [Leaf (Literal "foo")]
                Leaf Equality
                Expression [Leaf (Literal "bar")]
            ]
        ]
        |> Some

    Assert.AreEqual(expected, actual)