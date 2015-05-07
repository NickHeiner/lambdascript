// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System.IO

let GetFileContents filePath = 
    File.ReadAllLines (filePath, System.Text.Encoding.UTF8) |> Array.toSeq

let tokenize =
    Seq.map (fun (line : string) -> line.Split ' ' |> Array.toSeq) >>
    Seq.concat

type LexSymbol = Lambda | Identifier of string | FuncArrow | Literal of string 
let lex (tokens : seq<string>) = 
    Seq.map (fun token -> 
        match token with
        | "λ" -> Lambda 
        | "->" -> FuncArrow 
        | "1" -> Literal "1"
        | _ -> Identifier token
    ) tokens

type GrammarEntry = 
    | Expression
    | FuncDeclaration
    | Terminal of LexSymbol

(* Is there some way to enforce that GrammarEntry may not be a Terminal? *)
type GrammarRule = (GrammarEntry * List<GrammarEntry>)

(*
    Expression -> FuncDeclaration | Literal
    FuncDeclaration -> Lambda Identifier FuncArrow Expression
*)
let getMatchingRule = function
    | [Terminal (Literal lit)] -> Some Expression
    | [Terminal Lambda; Terminal (Identifier id); Terminal FuncArrow; Expression] -> Some FuncDeclaration
    | _ -> None

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

[<EntryPoint>]
let main argv = 
    let entryPoint = argv.[0]
    printfn "Entry point file: %s" entryPoint
    
    GetFileContents entryPoint |> 
        tokenize |>
        lex |>
        printfn "%A"

    0 // return an integer exit code
