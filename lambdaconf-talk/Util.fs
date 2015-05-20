module Util

(* I could have a more sophisticated way of doing this but w/e. *)
let SILENCE_LOGS = true

(* Is there a real log system in f#? *)
let log label obj = if not SILENCE_LOGS then printfn "%s %A" label obj else ()
