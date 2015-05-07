// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System.IO

let GetFileContents filePath = 
    File.ReadAllLines (filePath, System.Text.Encoding.UTF8) |> Array.toList

let tokenize =
    List.map (fun (line : string) -> 
        line.Split ' ' |> Array.toList |> List.filter ((<>) "")
    ) >>
    List.concat

type LexSymbol = Lambda | Identifier of string | FuncArrow | Literal of string 
let lex (tokens : list<string>) = 
    List.map (fun token -> 
        match token with
        | "λ" -> Lambda 
        | "->" -> FuncArrow 
        (* How do I parseInt? How do I use regex? *)
        | "1" -> Literal "1"
        | _ -> Identifier token
    ) tokens

(*
    Our grammar is:

    Expression -> FuncDeclaration | Literal
    FuncDeclaration -> Lambda Identifier FuncArrow Expression
*)
type ParseTree = 
    | Leaf of LexSymbol 
    | Expression of List<ParseTree>
    | FuncDeclaration of List<ParseTree>

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

let getMatchingRule = function
    | [Leaf (Literal lit)] -> 
        Some (Expression [Leaf (Literal "1")])
    | [Leaf Lambda; Leaf (Identifier id); Leaf FuncArrow; Expression e] -> 
        Some (FuncDeclaration [
                Leaf Lambda; 
                Leaf (Identifier id); 
                Leaf FuncArrow; 
                Expression e
            ]
       )
    | [FuncDeclaration children] -> Some (Expression [FuncDeclaration children])
    | _ -> None

(* What is the right way to do this? And is it just a builtin? *)
let stackEmpty = []
let stackPush list elem = elem::list
let stackPop list = List.head list, List.tail list

(* Is there a real log system in f#? *)
let log label obj = printfn "%s %A" label obj

let bottomUpParse lexSymbols = 
    let input = List.map Leaf lexSymbols
    log "parsing" input
    (* Keep popping from the stack until we find a handle. *)
    let tryFindHandle parseStack =
        let rec tryFindHandleRec (possibleHandle : ParseTree list) (restOfStack : ParseTree list) : ParseTree list option =
            log "checking to see if this is a handle" possibleHandle;
            match getMatchingRule possibleHandle with
            (* If we found something *)
            (* We were searching backwards through the parse stack, so before we return it, we need to re-reverse it. *)
            | Some parseTree -> Some (parseTree::restOfStack)
            (* Nothing found *)
            | None -> 
                log "no handle found" possibleHandle;
                match restOfStack with
                (* There are no handles on the parseStack right now *)
                | [] -> None
                (* We can keep going *)
                | _::_ -> 
                    let nextHandlePartToTry, remainingStack = stackPop restOfStack
                    tryFindHandleRec (List.append possibleHandle [nextHandlePartToTry]) remainingStack

        tryFindHandleRec [] (List.rev parseStack)
    
    let shift (parseStack : 'a list) (input : 'a list) : ('a list * 'a list) option =
        match input with
        (* If we want to shift, but we have no more inputs, then our input is not valid. *)
        | [] -> None
        | head::tail -> 
            log "shifting on to parse stack" head;
            Some ((List.append parseStack [head]), tail)

    (* When we think we may be done, we need to know if the state we wound up in is valid. *)
    let acceptableEndState = function
        | [Expression _] -> true
        | _ -> false

    let rec parseStep (parseStack : ParseTree list) (input : ParseTree list) : ParseTree option =
        match tryFindHandle parseStack with
        (* Reduce *)
        | Some nextParseStack -> parseStep nextParseStack input
        (* Shift *)
        | None ->
            match shift parseStack input with
            (* If we have no more input, we can make a decision now as to whether or not our input is valid. *)
            | None -> if acceptableEndState parseStack then Some (List.head parseStack) else None
            (* If we do have more input, we will have to keep looking. *)
            | Some (nextParseStack, nextInput) -> parseStep nextParseStack nextInput

    parseStep [] input

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
