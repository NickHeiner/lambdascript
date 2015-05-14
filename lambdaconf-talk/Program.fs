module Program

open ReadFile

open LambdaToJs

[<EntryPoint>]
let rec main argv = 
    let entryPoint = argv.[0]
    printfn "Entry point file: %s" entryPoint
    
    let result = 
        GetFileContents entryPoint 
        |> lambdaToJs

    match result with
    | Some any -> printfn "Compilation complete: %A" any; 0
    | None -> printfn "Invalid input"; 1
