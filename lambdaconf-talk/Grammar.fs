module Grammar

open Lex

(* 

Grammar: 
	Expr -> FuncDecl | FuncInvocation | StringLookup | Boolean | < Expr > | identifier | literal
	FuncDecl -> lambda identifier identifier funcDot Expr
	FuncInvocation -> identifier Expr
	StringLookup -> identifier [ regex ]
	Boolean -> Expr is Expr | Boolean or Boolean | Boolean and Boolean 

*)

type ParseTree = 
    | Expression of SententialForms
    | FuncDeclaration of SententialForms
    | FuncInvocation of SententialForms
    | StringLookup of SententialForms
    | Boolean of SententialForms
    | Leaf of LexSymbol

and SententialForms = ParseTree list
