module CstToAst

open Grammar
open Lex

type Argument = 
    | Lit of string
    | Ident of string

type FunctionDeclaration = {
    functionName: string
    argName: string
    body: ParseTree
    }

type FunctionInvocation = {
    func: ParseTree
    arg: Argument
    }

type BooleanOperator = 
    | Intersection
    | Union
    | Equal

type BooleanExpression = {
    leftHandSide: ParseTree
    operator: BooleanOperator
    rightHandSide: ParseTree
    }

type StringLookupInfo = {
    string: ParseTree
    regex: string
    }

type Ast =
    | FunctionDecl of FunctionDeclaration
    | FunctionInvoke of FunctionInvocation
    | Boolean of BooleanExpression
    | StringReLookup of StringLookupInfo

let cstToAst (ast : ParseTree option) : Ast option = 
    match ast with
    | None -> None
    | Some _ -> Some (FunctionInvoke { func = Leaf (Identifier "f"); arg = Ident "x"})