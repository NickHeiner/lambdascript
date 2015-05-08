module Tokenize

let tokenize =
    List.map (fun (line : string) -> 
        line.Split ' ' |> Array.toList |> List.filter ((<>) "")
    ) >>
    List.concat
