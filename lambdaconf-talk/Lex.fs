module Lex

open System.Text.RegularExpressions

type LexSymbol = 
    | Lambda 
    | Identifier of string 
    | FuncDot
    | Literal of string 
    
    (* TODO Determine if this hack is ok *)
    | FuncName of string
    | ArgName of string

    | OpenAngleBracket
    | CloseAngleBracket
    | OpenSquareBracket
    | CloseSquareBracket 
    | Equality
    | And
    | Or
    | RegexLiteral of string

let lex = 
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
        | "is" -> Equality
        | "and" -> And
        | "or" -> Or
        | FirstRegexGroup ":(.*)" str -> ArgName str
        | FirstRegexGroup "@(.*)" str -> FuncName str
        | FirstRegexGroup "\"(.*)\"" str -> Literal str
        | FirstRegexGroup "/(.*)/" regexContents -> RegexLiteral regexContents
        | _ -> Identifier token
    )


