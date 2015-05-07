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

[<EntryPoint>]
let main argv = 
    let entryPoint = argv.[0]
    printfn "Entry point file: %s" entryPoint
    
    GetFileContents entryPoint |> 
        tokenize |>
        lex |>
        printfn "%A"

    0 // return an integer exit code
