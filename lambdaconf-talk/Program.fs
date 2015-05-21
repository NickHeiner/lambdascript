module Program

open ReadFile
open Util
open System.Diagnostics

open LambdaToJs

[<EntryPoint>]
let rec main argv = 
    let entryPoint = argv.[0]
    log "Entry point file: %s" entryPoint
    
    let result = 
        GetFileContents entryPoint 
        |> lambdaToJs


    match result with
    | Some js -> 

        printfn """
---
Compilation complete: 

%s
               """ js

        logStep "Running generated js"

        let startInfo = new ProcessStartInfo("node", sprintf "-e \"%s\"" js)
        startInfo.RedirectStandardOutput <- true
        startInfo.UseShellExecute <- false
        startInfo.CreateNoWindow <- true
        let nodeProc = Process.Start(startInfo)
        nodeProc.WaitForExit()

        printfn """
Program output:
---
%s
              """ (nodeProc.StandardOutput.ReadToEnd())
              
        nodeProc.Dispose()

        0
    | None -> 
        log "Invalid input" ()
        1
