module Program

open ReadFile
open Tokenize
open Lex
open Grammar
open BottomUpParse
open CstToAst

[<EntryPoint>]
let rec main argv = 
    let entryPoint = argv.[0]
    printfn "Entry point file: %s" entryPoint
    
    let result = 
        GetFileContents entryPoint 
            |> tokenize 
            |> lex 
            |> bottomUpParse
            |> cstToAst

    match result with
    | Some any -> printfn "Compilation complete: %A" any; 0
    | None -> printfn "Invalid input"; 1
