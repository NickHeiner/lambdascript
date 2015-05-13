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

            let callExpr (callee : JObject) (arguments : JObject) = 
                new JObject([
                                typeProp "CallExpression"
                                new JProperty("callee", callee)
                                new JProperty("arguments", new JArray([arguments]))
                            ])

            match ast with
            | Lit value -> new JObject([typeProp "Literal"; new JProperty("value", value)])
            | Ident name -> identObj name
            | FunctionInvoke invocation -> 
                let callee = astToJsAstRec invocation.func
                let arguments = astToJsAstRec invocation.arg

                callExpr callee arguments

            | StringReLookup strLookup -> 
                let memberExpression (obj : JObject) (property : JObject) computed =
                    new JObject([
                                    typeProp "MemberExpression"
                                    new JProperty("computed", sprintf "%b" computed)
                                    new JProperty("object", obj)
                                    new JProperty("property", property)
                                ])

                let regexMatch =
                    let callee = 
                        let propertyObj = new JObject([typeProp "Identifier"; new JProperty("name", "match")])
                        let strToLookUp = astToJsAstRec strLookup.lookupSource
                        memberExpression strToLookUp propertyObj false

                    (* Not gonna lie this spacing is a bummer. *)
                    let args = new JObject([
                                                typeProp "Literal"
                                                new JProperty("value", strLookup.regex)
                                                new JProperty("regex", new JObject([
                                                                                     new JProperty("pattern", sprintf "/%s/" strLookup.regex)
                                                                                     new JProperty("flags", "")
                                                                                     ]))
                                            ])
                
                    callExpr callee args

                let propertyObj = new JObject([typeProp "Literal"; new JProperty("value", 1)])
                memberExpression regexMatch propertyObj true

            | _ -> JsAstGenerationError "not yet implemented" |> raise

        let body = new JArray([astToJsAstRec ast |> inExpressionStatement])

        new JObject([new JProperty("type", "Program"); new JProperty("body", body)])
        |> JsonConvert.SerializeObject
        |> Some

