module AstToJsAst

open Newtonsoft.Json.Linq

let astToJsAst ast = function
    | None -> None
    | Some ast -> 
        let rec astToJsAstRec ast = function
            | _ -> 0

        astToJsAstRec ast |> Some