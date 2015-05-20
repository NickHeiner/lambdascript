module Tokenize

open System.Text.RegularExpressions

let tokenize lines =
    let tokenRegex = Regex @" |(\[|\]|\<|\>|;)"
    lines
    |> List.map tokenRegex.Split
    |> List.map Array.toList
    |> List.concat
    |> List.filter ((<>) "")
