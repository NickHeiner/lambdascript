﻿module Grammar

open Lex

(* 

Grammar: 
	Expr -> FuncDecl | FuncInvocation | StringLookup | Boolean | < Expr > | identifier | literal | Expr ; Expr
	FuncDecl -> lambda funcName argName funcDot Expr
	FuncInvocation -> Expr Expr
	StringLookup -> Expr [ regex ]
	Boolean -> Expr equality Expr | Boolean or Boolean | Boolean and Boolean 

*)

(* TODO SententialForms is too broad - we know exactly how many arguments we are expecting *)
type ParseTree = 
    | Expression of SententialForms
    | FuncDeclaration of SententialForms
    | FuncInvocation of SententialForms
    | StringLookup of SententialForms
    | Boolean of SententialForms
    | Leaf of LexSymbol

and SententialForms = ParseTree list
