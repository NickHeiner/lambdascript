module BottomUpParse

open Lex
open Util
open Grammar

let getMatchingRule = function
    | [Leaf (Literal lit)] -> 
        Some (Expression [Leaf (Literal "1")])
    | [Leaf Lambda; Leaf (Identifier id); Leaf FuncArrow; Expression e] -> 
        Some (FuncDeclaration [
                Leaf Lambda; 
                Leaf (Identifier id); 
                Leaf FuncArrow; 
                Expression e
            ]
       )
    | [FuncDeclaration children] -> Some (Expression [FuncDeclaration children])
    | _ -> None

(* What is the right way to do this? And is it just a builtin? *)
let stackEmpty = []
let stackPush list elem = elem::list
let stackPop list = List.head list, List.tail list


let bottomUpParse lexSymbols = 
    let input = List.map Leaf lexSymbols
    log "parsing" input
    (* Keep popping from the stack until we find a handle. *)
    let tryFindHandle parseStack =
        let rec tryFindHandleRec (possibleHandle : ParseTree list) (restOfStack : ParseTree list) : ParseTree list option =
            log "checking to see if this is a handle" possibleHandle;
            match getMatchingRule possibleHandle with
            (* If we found something *)
            (* We were searching backwards through the parse stack, so before we return it, we need to re-reverse it. *)
            | Some parseTree -> Some (parseTree::restOfStack)
            (* Nothing found *)
            | None -> 
                log "no handle found" possibleHandle;
                match restOfStack with
                (* There are no handles on the parseStack right now *)
                | [] -> None
                (* We can keep going *)
                | _::_ -> 
                    let nextHandlePartToTry, remainingStack = stackPop restOfStack
                    tryFindHandleRec (List.append possibleHandle [nextHandlePartToTry]) remainingStack

        tryFindHandleRec [] (List.rev parseStack)
    
    let shift (parseStack : 'a list) (input : 'a list) : ('a list * 'a list) option =
        match input with
        (* If we want to shift, but we have no more inputs, then our input is not valid. *)
        | [] -> None
        | head::tail -> 
            log "shifting on to parse stack" head;
            Some ((List.append parseStack [head]), tail)

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
            | None -> if acceptableEndState parseStack then Some (List.head parseStack) else None
            (* If we do have more input, we will have to keep looking. *)
            | Some (nextParseStack, nextInput) -> parseStep nextParseStack nextInput

    parseStep [] input