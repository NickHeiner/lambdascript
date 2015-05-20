module Program

open ReadFile
open Util

open LambdaToJs

[<EntryPoint>]
let rec main argv = 
    let entryPoint = argv.[0]
    log "Entry point file: %s" entryPoint
    
    let result = 
        GetFileContents entryPoint 
        |> lambdaToJs

    match result with
    | Some any -> log "Compilation complete: %A" any; 0
    | None -> log "Invalid input" (); 1
