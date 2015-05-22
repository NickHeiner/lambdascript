module Program

open ReadFile
open Util
open System.Diagnostics
open System.IO

open LambdaToJs

[<EntryPoint>]
let rec main argv = 
    let entryPoint = argv.[0]
    let writeToFile = if argv.Length >= 2 then Some argv.[1] else None
    log "Entry point file: %s" entryPoint
    
    let result = 
        GetFileContents entryPoint 
        |> lambdaToJs

    match result with
    | Some js -> 
        
        match writeToFile with
        | Some fileName ->
            log "Writing output js to %s" fileName
            File.WriteAllText(fileName, js, System.Text.Encoding.UTF8)
        | None -> ()

        printfn """
---
Compilation complete: 

%s
               """ js

        logStep "Running generated js"

        let startInfo = new ProcessStartInfo("node", sprintf "-e \"%s\"" js)
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false
        startInfo.CreateNoWindow <- true
        let nodeProc = Process.Start(startInfo)
        nodeProc.WaitForExit()

        printfn """
Program stdout:
---
%s

Program stderr:
---
%s
              """ (nodeProc.StandardOutput.ReadToEnd()) (nodeProc.StandardError.ReadToEnd())
              
        nodeProc.Dispose()

        0
    | None -> 
        log "Invalid input" ()
        1
