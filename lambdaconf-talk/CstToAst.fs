module CstToAst

open Grammar
open Lex

type Argument = 
    | Lit of string
    | Ident of string

type BooleanOperator = 
    | Intersection
    | Union
    | Equal

type BooleanExpression = {
    leftHandSide: Ast
    operator: BooleanOperator
    rightHandSide: Ast
    }

and FunctionDeclaration = {
    functionName: string
    argName: string
    body: Ast
    }
    
and FunctionInvocation = {
    func: ParseTree
    arg: Argument
    }


and StringLookupInfo = {
    lookupSource: Ast
    regex: string
    }

and Ast =
    | FunctionDecl of FunctionDeclaration
    | FunctionInvoke of FunctionInvocation
    | Boolean of BooleanExpression
    | StringReLookup of StringLookupInfo
    | Lit of string
    | Ident of string

let cstToAst (astOpt : ParseTree option) : Ast option = 
    match astOpt with
    | None -> None
    | Some ast -> None 