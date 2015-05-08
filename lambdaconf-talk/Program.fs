// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open ReadFile
open Tokenize
open Lex
open Grammar
open BottomUpParse

let sampleTree = 
    Expression [
        FuncDeclaration [
            Leaf Lambda; 
            Leaf (Identifier "x"); 
            Leaf FuncArrow; 
            Expression [
                Leaf (Literal "1")
            ]
        ]
    ]

(* lol I wish I knew how to have code in separate files haha *)
open NUnit.Framework

[<Test>]
let ``tokenize - no input``() = 
    let actual = tokenize []
    let expected = []
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``tokenize - some input``() = 
    let actual = tokenize ["λ x -> 1"]
    let expected = ["λ"; "x"; "->"; "1"]
    Assert.AreEqual(expected, actual)

[<Test>]
let ``tokenize - many spaces``() = 
    let actual = tokenize ["λ     x  ->   1"]
    let expected = ["λ"; "x"; "->"; "1"]
    Assert.AreEqual(expected, actual)

[<Test>]
let ``lex - no input``() = 
    Assert.AreEqual(lex [], [])

[<Test>]
let ``lex - some input``() = 
    let actual = lex ["λ"; "x"; "->"; "1"]
    let expected = [
        Lambda;
        Identifier "x";
        FuncArrow;
        Literal "1"
    ]
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``lex - different identifier``() = 
    let actual = lex ["λ"; "f"; "->"; "1"]
    let expected = [
        Lambda;
        Identifier "f";
        FuncArrow;
        Literal "1"
    ]
    
    Assert.AreEqual(expected, actual)

[<Test>]
let ``bottomUpParse - sample``() = 
    let actual = bottomUpParse [
                                Lambda;
                                Identifier "x";
                                FuncArrow;
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
                                FuncArrow;
                                Literal "1"
                               ]
    Assert.AreEqual(None, actual)
(* end tests *)

[<EntryPoint>]
let rec main argv = 
    let entryPoint = argv.[0]
    printfn "Entry point file: %s" entryPoint
    
    let result = 
        GetFileContents entryPoint 
            |> tokenize 
            |> lex 
            |> bottomUpParse

    match result with
    | Some any -> printfn "Compilation complete: %A" any; 0
    | None -> printfn "Invalid input"; 1
