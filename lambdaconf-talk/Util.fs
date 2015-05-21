module Util

(* Is there a real log system in f#? *)

(* I could have a more sophisticated way of doing this but w/e. *)
let SILENCE_DEBUG_LOGS = false

let log label obj = if not SILENCE_DEBUG_LOGS then printfn "%s %A" label obj else ()

let logStep label = printfn ">> Starting %s" label
    
