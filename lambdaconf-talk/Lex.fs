module Lex

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
    List.map (fun token -> 
        match token with
        | "λ" -> Lambda 
        | "." -> FuncDot
        | _ -> Identifier token
    ) tokens


