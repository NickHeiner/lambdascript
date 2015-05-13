module CstToAst

open Grammar
open Lex
open Util

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
    func: Ast
    arg: Ast
    }


and StringLookupInfo = {
    lookupSource: Ast
    regex: string
    }

(* TODO All these names are a mess. *)
and Ast =
    | FunctionDecl of FunctionDeclaration
    | FunctionInvoke of FunctionInvocation
    | Bool of BooleanExpression
    | StringReLookup of StringLookupInfo
    | Lit of string
    | Ident of string

    (* This is just to avoid writing "|> Some" all over the place *)
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

            | FuncInvocation [Expression _ as func; Expression _ as arg] -> 
                FunctionInvoke { func = cstToAstRec func; arg = cstToAstRec arg }

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

        match cstToAstRec ast with
        | Unknown -> None
        | _ as anythingElse -> Some anythingElse
