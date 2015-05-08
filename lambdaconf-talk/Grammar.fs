module Grammar

open Lex

(*
    Our grammar is:

    Expression -> FuncDeclaration | Literal
    FuncDeclaration -> Lambda Identifier FuncArrow Expression
*)
type ParseTree = 
    | Leaf of LexSymbol 
    | Expression of List<ParseTree>
    | FuncDeclaration of List<ParseTree>
