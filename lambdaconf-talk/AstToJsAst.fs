module AstToJsAst

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open CstToAst

let (astToJsAst : Ast option -> string option) = function
    | None -> None
    | Some ast -> 
        let rec astToJsAstRec (ast : Ast) : JObject = 
            let typeProp (value : string) = new JProperty("type", value)

            let inExpressionStatement (expr : JObject) =
                new JObject([typeProp "ExpressionStatement"; new JProperty("expression", expr)])

            match ast with
            | Lit value -> new JObject([typeProp "Literal"; new JProperty("value", value)]) |> inExpressionStatement
            | Ident name -> new JObject([typeProp "Identifier"; new JProperty("name", name)]) |> inExpressionStatement
            | _ -> new JObject([new JProperty("id", 1); new JProperty("v", 123)])

        let body = new JArray([astToJsAstRec ast])

        new JObject([new JProperty("type", "program"); new JProperty("body", body)])
        |> JsonConvert.SerializeObject
        |> Some

