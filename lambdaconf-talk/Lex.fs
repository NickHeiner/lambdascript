module Lex

open System.Text.RegularExpressions

type LexSymbol = 
    | Lambda 
    | Identifier of string 
    | FuncName of string
    | ArgName of string
    | FuncDot
    | Literal of string 
    | OpenAngleBracket
    | CloseAngleBracket
    | OpenSquareBracket
    | CloseSquareBracket 
    | Equality
    | And
    | Or
    | RegexLiteral of string

let lex tokens = 
    // From http://fsharpforfunandprofit.com/posts/convenience-active-patterns/
    let (|FirstRegexGroup|_|) pattern input =
       let m = Regex.Match(input,pattern) 
       if (m.Success) then Some m.Groups.[1].Value else None  

    let (|DirectlyFollowsLambda|_|) index list token =
       if index > 1 && List.item (index - 1) list = "λ" then Some token else None

    List.mapi (fun index token -> 
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
        | FirstRegexGroup "\"(.*)\"" str -> Literal str
        | FirstRegexGroup "/(.*)/" regexContents -> RegexLiteral regexContents
        | DirectlyFollowsLambda index tokens ident -> FuncName ident 
        | _ -> Identifier token
    ) tokens


