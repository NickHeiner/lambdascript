module BottomUpParseTest

open Lex
open Grammar
open BottomUpParse
open NUnit.Framework

[<Test>]
let ``bottomUpParse - function declaration`` () = 
    (* Parsing
        λ constantFn arg . arg
    *)
    let actual = 
        bottomUpParse [
            Lambda
            Identifier "constantFn"
            Identifier "arg"
            FuncDot
            Identifier "arg"
        ]
    let expected = 
        Expression [
            FuncDeclaration [
                Leaf Lambda
                Leaf (Identifier "x")
                Leaf (Identifier "arg")
                Leaf FuncDot
                Expression [
                    Leaf (Identifier "arg")
                ]
            ]
        ] 
        |> Some
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