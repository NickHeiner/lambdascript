module Lex

open System.Text.RegularExpressions

type LexSymbol = 
    | Lambda 
    | Identifier of string 
    | FuncDot
    | Literal of string 
    | OpenAngleBracket
    | CloseAngleBracket
    | OpenSquareBracket
    | CloseSquareBracket 
    | Is
    | And
    | Or
    | RegexLiteral of string

let lex (tokens : list<string>) = 
    // From http://fsharpforfunandprofit.com/posts/convenience-active-patterns/
    let (|FirstRegexGroup|_|) pattern input =
       let m = Regex.Match(input,pattern) 
       if (m.Success) then Some m.Groups.[1].Value else None  

    List.map (fun token -> 
        match token with
        | "λ" -> Lambda 
        | "." -> FuncDot
        | "<" -> OpenAngleBracket
        | ">" -> CloseAngleBracket
        | "[" -> OpenSquareBracket
        | "]" -> CloseSquareBracket
        | FirstRegexGroup "\"(.*)\"" str -> Literal str
        | _ -> Identifier token
    ) tokens


