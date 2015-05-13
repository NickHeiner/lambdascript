﻿module CstToAst

open Grammar
open Lex
open Util

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
    | Bool of BooleanExpression
    | StringReLookup of StringLookupInfo
    | Lit of string
    | Ident of string
    | Unknown

let cstToAst (astOpt : ParseTree option) : Ast option = 
    match astOpt with
    | None -> None
    | Some ast -> 
        let rec cstToAstRec ast = 
            log "cstToAstRec" ast
            match ast with
            | Leaf (Literal value) -> Lit value
            | Leaf (Identifier value) -> Ident value
            
            | Expression [Leaf OpenAngleBracket; Expression _ as expr; Leaf CloseAngleBracket] -> cstToAstRec expr
            | Expression (hd::tl) when List.isEmpty tl -> cstToAstRec hd

            (* I bet I could DRY this out *)
            | Boolean [Expression _ as lhs; Leaf Equality; Expression _ as rhs] -> 
                Bool { leftHandSide = cstToAstRec lhs; operator = Equal; rightHandSide = cstToAstRec rhs }
            | Boolean [Expression _ as lhs; Leaf And; Expression _ as rhs] -> 
                Bool { leftHandSide = cstToAstRec lhs; operator = Intersection; rightHandSide = cstToAstRec rhs }
            | Boolean [Expression _ as lhs; Leaf Or; Expression _ as rhs] -> 
                Bool { leftHandSide = cstToAstRec lhs; operator = Union; rightHandSide = cstToAstRec rhs }

            | StringLookup [
                            Expression str as toLookup
                            Leaf OpenSquareBracket 
                            Leaf (RegexLiteral re)
                            Leaf CloseSquareBracket
                ] -> StringReLookup {
                        lookupSource = cstToAstRec toLookup
                        regex = re    
                    }
            | _ -> Unknown

        cstToAstRec ast |> Some
