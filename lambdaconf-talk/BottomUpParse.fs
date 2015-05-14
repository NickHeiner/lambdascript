module BottomUpParse

open Lex
open Util
open Grammar

let getMatchingRule = function
    (* Expressions *)
    | [Leaf (Identifier _)] as identifierLeaf -> Some (Expression identifierLeaf)
    | [Leaf (Literal _)] as literalLeaf -> Some (Expression literalLeaf)
    | [FuncDeclaration _] as funcDecl -> Some (Expression funcDecl)
    | [FuncInvocation _] as funcInvoke -> Some (Expression funcInvoke)
    | [StringLookup _] as stringLookup -> Some (Expression stringLookup)
    | [Boolean _] as boolean -> Some (Expression boolean)
    | [Leaf OpenAngleBracket; Expression _; Leaf CloseAngleBracket] as children -> Some (Expression children)

    (* Functions *)
    | [Leaf Lambda; Leaf (FuncName _); Leaf (ArgName _); Leaf FuncDot; Expression _] as children -> 
        Some (FuncDeclaration children)
    | [Expression _; Expression _] as expressions -> Some (FuncInvocation expressions)
    
    (* Booleans *)
    | [Expression _; Leaf Equality; Expression _] as children -> Some (Boolean children)
    | [Expression _; Leaf Or; Expression _] as children -> Some (Boolean children)
    | [Expression _; Leaf And; Expression _] as children -> Some (Boolean children)
    
    (* StringLookup *)
    | [Expression _; Leaf OpenSquareBracket; Leaf (RegexLiteral _); Leaf CloseSquareBracket] as children -> 
        Some (StringLookup children)
    
    (* We have no match *)
    | _ -> None

(* What is the right way to do this? And is it just a builtin? *)
let stackEmpty = []
let stackPush list elem = elem::list
let stackPop list = List.head list, List.tail list

let bottomUpParse lexSymbols = 
    let inputStack = lexSymbols |> List.map Leaf |> List.rev
    log "parsing" inputStack
    (* Keep popping from the stack until we find a handle. *)
    let tryFindHandle parseStack =
        let rec tryFindHandleRec (possibleHandle : ParseTree list) (restOfStack : ParseTree list) : ParseTree list option =
            log "checking to see if this is a handle" possibleHandle;
            match possibleHandle |> List.rev |> getMatchingRule with
            (* If we found something *)
            | Some parseTree -> 
                log "reducing by putting this on the stack" parseTree
                Some (parseTree::restOfStack)
            (* Nothing found *)
            | None -> 
                match restOfStack with
                (* There are no handles on the parseStack right now *)
                | [] -> None
                (* We can keep going *)
                | nextHandlePartToTry::remainingStack -> 
                    tryFindHandleRec (nextHandlePartToTry::possibleHandle) remainingStack

        tryFindHandleRec [] parseStack
    
    let shift (parseStack : 'a list) (input : 'a list) : ('a list * 'a list) option =
        match input with
        (* If we want to shift, but we have no more inputs, then our input is not valid. *)
        | [] -> None
        | head::tail -> 
            log "shifting on to parse stack" head;
            Some (stackPush parseStack head, tail)

    (* When we think we may be done, we need to know if the state we wound up in is valid. *)
    let acceptableEndState = function
        | [Expression _] -> true
        | _ -> false

    let rec parseStep (parseStack : ParseTree list) (input : ParseTree list) : ParseTree option =
        match tryFindHandle parseStack with
        (* Reduce *)
        | Some nextParseStack -> parseStep nextParseStack input
        (* Shift *)
        | None ->
            match shift parseStack input with
            (* If we have no more input, we can make a decision now as to whether or not our input is valid. *)
            | None -> if acceptableEndState parseStack then parseStack |> List.head |> Some else None
            (* If we do have more input, we will have to keep looking. *)
            | Some (nextParseStack, nextInput) -> parseStep nextParseStack nextInput

    parseStep [] inputStack