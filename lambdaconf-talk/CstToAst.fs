module CstToAst

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
    | Boolean of BooleanExpression
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
            | Expression [Leaf OpenAngleBracket; Expression e as expr; Leaf CloseAngleBracket] -> cstToAstRec expr
            | Expression (hd::tl) when List.isEmpty tl -> cstToAstRec hd
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
