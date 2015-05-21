module Tokenize

open System.Text.RegularExpressions
open Util

let tokenize lines =
    logStep "tokenizing"
    let tokenRegex = Regex @"\s+|(\[|\]|\<|\>|;)"
    lines
    |> List.map tokenRegex.Split
    |> List.map Array.toList
    |> List.concat
    |> List.filter ((<>) "")
