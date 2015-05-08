module Lex

type LexSymbol = Lambda | Identifier of string | FuncArrow | Literal of string 
let lex (tokens : list<string>) = 
    List.map (fun token -> 
        match token with
        | "λ" -> Lambda 
        | "->" -> FuncArrow 
        (* How do I parseInt? How do I use regex? *)
        | "1" -> Literal "1"
        | _ -> Identifier token
    ) tokens


