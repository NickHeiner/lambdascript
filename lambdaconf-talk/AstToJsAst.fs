module AstToJsAst

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open CstToAst

exception JsAstGenerationError of string

let (astToJsAst : Ast option -> string option) = function
    | None -> None
    | Some ast -> 
        let typeProp (value : string) = new JProperty("type", value)

        let inExpressionStatement (expr : JObject) =
            new JObject([typeProp "ExpressionStatement"; new JProperty("expression", expr)])

        let rec astToJsAstRec (ast : Ast) : JObject = 
            let identObj (name : string) =
                new JObject([typeProp "Identifier"; new JProperty("name", name)])

            match ast with
            | Lit value -> new JObject([typeProp "Literal"; new JProperty("value", value)])
            | Ident name -> new JObject([typeProp "Identifier"; new JProperty("name", name)])
            | FunctionInvoke invocation -> 
                let callee = astToJsAstRec invocation.func
                let arguments = astToJsAstRec invocation.arg

                new JObject([typeProp "CallExpression"; new JProperty("callee", callee); new JProperty("arguments", new JArray([arguments]))])

            | _ -> JsAstGenerationError "not yet implemented" |> raise

        let body = new JArray([astToJsAstRec ast |> inExpressionStatement])

        new JObject([new JProperty("type", "Program"); new JProperty("body", body)])
        |> JsonConvert.SerializeObject
        |> Some

