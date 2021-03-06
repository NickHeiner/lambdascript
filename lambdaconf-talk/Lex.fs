﻿module Lex

open System.Text.RegularExpressions
open Util

type LexSymbol = 
    | Lambda 
    | Identifier of string 
    | FuncDot
    | Literal of string 
    
    | FuncName of string
    | ArgName of string

    | OpenAngleBracket
    | CloseAngleBracket
    | OpenSquareBracket
    | CloseSquareBracket 
    | Equality
    | And
    | Or
    | ExpressionSep
    | RegexLiteral of string

let lex = 
    logStep "lexing"
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
        | ";" -> ExpressionSep
        | FirstRegexGroup "\"(.*)\"" str -> Literal str
        | FirstRegexGroup "/(.*)/" regexContents -> RegexLiteral regexContents
        | _ -> Identifier token
    )


