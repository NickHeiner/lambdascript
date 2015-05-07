// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System.IO

let GetFileContents filePath = 
    File.ReadAllLines (filePath, System.Text.Encoding.UTF8) |> Array.toList

let tokenize =
    List.map (fun (line : string) -> line.Split ' ' |> Array.toList) >>
    List.concat

type LexSymbol = Lambda | Identifier of string | FuncArrow | Literal of string 
let lex (tokens : list<string>) = 
    List.map (fun token -> 
        match token with
        | "λ" -> Lambda 
        | "->" -> FuncArrow 
        (* How do I parseInt? How do I use regex? *)
        | "1" -> Literal "1"
        | _ -> Identifier token
    ) tokens

(*
    Expression -> FuncDeclaration | Literal
    FuncDeclaration -> Lambda Identifier FuncArrow Expression
*)
type ParseTree = 
    | Leaf of LexSymbol 
    | Expression of List<ParseTree>
    | FuncDeclaration of List<ParseTree>

let sampleTree = 
    Expression [
        FuncDeclaration [
            Leaf Lambda; 
            Leaf (Identifier "x"); 
            Leaf FuncArrow; 
            Expression [
                Leaf (Literal "1")
            ]
        ]
    ]

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
    (* Keep popping from the stack until we find a handle. *)
    let tryFindHandle parseStack =
        let rec tryFindHandleRec (possibleHandle : ParseTree list) (restOfStack : ParseTree list) : ParseTree list option =
            match getMatchingRule possibleHandle with
            (* If we found something *)
            | Some parseTree -> Some (List.append restOfStack [parseTree])
            (* Nothing found *)
            | None -> 
                match restOfStack with
                (* There are no handles on the parseStack right now *)
                | [] -> None
                (* We can keep going *)
                | _::_ -> 
                    let nextHandlePartToTry, remainingStack = stackPop restOfStack
                    tryFindHandleRec (List.append possibleHandle [nextHandlePartToTry]) remainingStack

        tryFindHandleRec [] parseStack
    
    let shift (parseStack : 'a list) (input : 'a list) : ('a list * 'a list) option =
        match input with
        (* If we want to shift, but we have no more inputs, then our input is not valid. *)
        | [] -> None
        | head::tail -> Some ((List.append parseStack [head]), tail)

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
            
(* 
let parse lexSymbols =
    let grammarEntries = Seq.map Terminal lexSymbols
    let rec parseRec unusedSymbols parseStack =
        let matchingRule = getMatchingRule parseStack
        match matchingRule with
        | None -> match unusedSymbols with
            | [] -> match parseStack with
                (* If we just have an expression, then we are in a valid end state *)
                | [Expression] -> true
                (* If we have anything else, then the input is not valid. *)
                | _ -> false
            (* Shift *)
            | head::tail -> parseReq (Seq.tail lexSymbols) (parseStack::(Seq.head lexSymbols))
        | Some grammarEntry -> parseReq
    parseRec (Seq.tail grammarEntries) [(Seq.head grammarEntries)]
*)



[<EntryPoint>]
let main argv = 
    let entryPoint = argv.[0]
    printfn "Entry point file: %s" entryPoint
    
    GetFileContents entryPoint |> 
        tokenize |>
        lex |>
        printfn "%A"

    0 // return an integer exit code
