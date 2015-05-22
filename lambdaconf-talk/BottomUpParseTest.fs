module BottomUpParseTest

open Lex
open ReLex
open Grammar
open BottomUpParse
open NUnit.Framework
open ReadFile
open Tokenize

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
    
[<Test>]
let ``bottomUpParse - boolean OR`` () =
    (* I wish I didn't need all these brackets. *)
    let actual = 
        bottomUpParse [
            OpenAngleBracket
            Literal "foo"
            Equality
            Literal "bar"
            CloseAngleBracket
            Or
            OpenAngleBracket
            Identifier "x"
            Equality
            Identifier "y"
            CloseAngleBracket
        ]
    let expected = 
        Expression [
            Boolean [
                Expression [
                    Leaf OpenAngleBracket
                    Expression [
                        Boolean [
                            Expression [Leaf (Literal "foo")]
                            Leaf Equality
                            Expression [Leaf (Literal "bar")]
                        ]
                    ]
                    Leaf CloseAngleBracket
                ]
                Leaf Or
                Expression [
                    Leaf OpenAngleBracket
                    Expression [
                        Boolean [
                            Expression [Leaf (Identifier "x")]
                            Leaf Equality
                            Expression [Leaf (Identifier "y")]
                        ]
                    ]
                    Leaf CloseAngleBracket
                ]
            ]
        ]
        |> Some

    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``bottomUpParse - boolean AND`` () =
    (* I wish I didn't need all these brackets. *)
    let actual = 
        bottomUpParse [
            OpenAngleBracket
            Literal "foo"
            Equality
            Literal "bar"
            CloseAngleBracket
            And
            OpenAngleBracket
            Identifier "x"
            Equality
            Identifier "y"
            CloseAngleBracket
        ]
    let expected = 
        Expression [
            Boolean [
                Expression [
                    Leaf OpenAngleBracket
                    Expression [
                        Boolean [
                            Expression [Leaf (Literal "foo")]
                            Leaf Equality
                            Expression [Leaf (Literal "bar")]
                        ]
                    ]
                    Leaf CloseAngleBracket
                ]
                Leaf And
                Expression [
                    Leaf OpenAngleBracket
                    Expression [
                        Boolean [
                            Expression [Leaf (Identifier "x")]
                            Leaf Equality
                            Expression [Leaf (Identifier "y")]
                        ]
                    ]
                    Leaf CloseAngleBracket
                ]
            ]
        ]
        |> Some

    Assert.AreEqual(expected, actual)

[<Test>]
let ``bottomUpParse - string lookup`` () =
    let expected = 
        Expression [
            StringLookup [
                Expression [
                    Leaf (Identifier "str")
                ]
                Leaf OpenSquareBracket
                Leaf (RegexLiteral "^a")
                Leaf CloseSquareBracket
            ]
        ] |> Some

    let actual = 
        bottomUpParse [
            Identifier "str"
            OpenSquareBracket
            RegexLiteral "^a"
            CloseSquareBracket
        ]

    Assert.AreEqual(expected, actual)

[<Test>]
let ``bottomUpParse - expression list`` () =
    let expected = 
        Expression [
            Expression [
                Leaf (Identifier "x")
            ]
            Leaf ExpressionSep
            Expression [
                Leaf (Identifier "y")
            ]
        ]
        |> Some
    let actual = 
        bottomUpParse [
            Identifier "x"
            ExpressionSep
            Identifier "y"
        ]

    Assert.AreEqual(expected, actual)

[<Test>]
let ``bottomUpParse - function and invocation`` () =
    let expected = 
        Expression [
            Expression [
                Leaf OpenAngleBracket
                Expression [
                    FuncDeclaration [
                        Leaf Lambda
                        Leaf <| FuncName "f"
                        Leaf <| ArgName "x"
                        Leaf FuncDot
                        Expression [Leaf <| Literal "return value"]
                    ]
                ]
                Leaf CloseAngleBracket
            ]
            Leaf ExpressionSep
            Expression [
                FuncInvocation [
                    Expression [Leaf <| Identifier "print"]
                    Expression [Leaf <| Literal "hello world"]
                ]
            ]    
         ]

    let actual = 
        bottomUpParse [
            OpenAngleBracket
            Lambda
            FuncName "f"
            ArgName "x"
            FuncDot
            Literal "return value"
            CloseAngleBracket
            ExpressionSep
            Identifier "print"
            Literal "hello world"
        ]
        |> Option.get

    Assert.AreEqual(expected, actual)

[<Test>]
let ``bottomUpParse - sample`` () =
    let actual = 
        "..\..\sample.lambda"
        |> GetFileContents
        |> tokenize
        |> lex
        |> reLex
        |> bottomUpParse
        |> Option.get

    let expected = 
        Expression
          [Expression
             [Leaf OpenAngleBracket;
              Expression
                [FuncDeclaration
                   [Leaf Lambda; Leaf (FuncName "isPalindrome"); Leaf (ArgName "str");
                    Leaf FuncDot;
                    Expression
                      [Boolean
                         [Expression
                            [Leaf OpenAngleBracket;
                             Expression
                               [Boolean
                                  [Expression [Leaf (Identifier "str")]; Leaf Equality;
                                   Expression [Leaf (Literal "")]]];
                             Leaf CloseAngleBracket]; Leaf Or;
                          Expression
                            [Leaf OpenAngleBracket;
                             Expression
                               [Boolean
                                  [Expression
                                     [Leaf OpenAngleBracket;
                                      Expression
                                        [Boolean
                                           [Expression
                                              [StringLookup
                                                 [Expression [Leaf (Identifier "str")];
                                                  Leaf OpenSquareBracket;
                                                  Leaf (RegexLiteral "^(.)");
                                                  Leaf CloseSquareBracket]];
                                            Leaf Equality;
                                            Expression
                                              [StringLookup
                                                 [Expression [Leaf (Identifier "str")];
                                                  Leaf OpenSquareBracket;
                                                  Leaf (RegexLiteral ".*(.)$");
                                                  Leaf CloseSquareBracket]]]];
                                      Leaf CloseAngleBracket]; Leaf And;
                                   Expression
                                     [FuncInvocation
                                        [Expression [Leaf (Identifier "isPalindrome")];
                                         Expression
                                           [StringLookup
                                              [Expression [Leaf (Identifier "str")];
                                               Leaf OpenSquareBracket;
                                               Leaf (RegexLiteral "^.(.*).");
                                               Leaf CloseSquareBracket]]]]]];
                             Leaf CloseAngleBracket]]]]]; Leaf CloseAngleBracket];
           Leaf ExpressionSep;
           Expression
             [Expression
                [Leaf OpenAngleBracket;
                 Expression
                   [FuncInvocation
                      [Expression [Leaf (Identifier "print")];
                       Expression
                         [Leaf OpenAngleBracket;
                          Expression
                            [FuncInvocation
                               [Expression [Leaf (Identifier "isPalindrome")];
                                Expression [Leaf (Literal "racecar")]]];
                          Leaf CloseAngleBracket]]]; Leaf CloseAngleBracket];
              Leaf ExpressionSep;
              Expression
                [FuncInvocation
                   [Expression [Leaf (Identifier "print")];
                    Expression
                      [Leaf OpenAngleBracket;
                       Expression
                         [FuncInvocation
                            [Expression [Leaf (Identifier "isPalindrome")];
                             Expression [Leaf (Literal "not-a-palindrome")]]];
                       Leaf CloseAngleBracket]]]]]

    Assert.AreEqual(expected, actual)