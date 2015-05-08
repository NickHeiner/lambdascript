module Util

(* Is there a real log system in f#? *)
let log label obj = printfn "%s %A" label obj
