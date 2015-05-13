module AstToJsAst

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open CstToAst

let (astToJsAst : Ast option -> string option) = function
    | None -> None
    | Some ast -> 
        let rec astToJsAstRec ast = function
            | _ -> new JObject([new JProperty("id", 1); new JProperty("v", 123)]);

        astToJsAstRec ast |> JsonConvert.SerializeObject |> Some

