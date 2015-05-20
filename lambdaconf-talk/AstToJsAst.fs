module AstToJsAst

open Newtonsoft.Json
open Newtonsoft.Json.Linq

open CstToAst
open Util

exception JsAstGenerationError of string

let astToJsAst astOpt = 
    logStep "transforming lambda script AST to js AST"
    match astOpt with
    | None -> None
    | Some ast -> 
        let typeProp (value : string) = new JProperty("type", value)
        
        let inExpressionStatement (expr : JObject) =
            new JObject([typeProp "ExpressionStatement"; new JProperty("expression", expr)])

        let rec astToJsAstRec (ast : Ast) : JObject = 
            let identObj (name : string) =
                new JObject([typeProp "Identifier"; new JProperty("name", name)])

            let callExpr (callee : JObject) (arguments : JObject option) = 
                let argMembers = match arguments with
                                 | Some arg -> [arg]
                                 | None -> []

                new JObject([
                                typeProp "CallExpression"
                                new JProperty("callee", callee)
                                new JProperty("arguments", new JArray(argMembers))
                            ])

            match ast with
            | Lit value -> new JObject([typeProp "Literal"; new JProperty("value", value)])
            | Ident name -> identObj name
            | FunctionInvoke invocation -> 
                let callee = astToJsAstRec invocation.func
                let arguments = astToJsAstRec invocation.arg

                callExpr callee (Some arguments)

            | StringReLookup strLookup -> 
                let memberExpression (obj : JObject) (property : JObject) (computed : bool) =
                    new JObject([
                                    typeProp "MemberExpression"
                                    new JProperty("computed", computed)
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
                
                    callExpr callee (Some args)

                let propertyObj = new JObject([typeProp "Literal"; new JProperty("value", 1)])
                memberExpression regexMatch propertyObj true

            | Bool boolExpr -> 
                let jsOperator = match boolExpr.operator with
                                 | Union -> "||"
                                 | Intersection -> "&&"
                                 | Equal -> "==="

                new JObject([
                                typeProp "LogicalExpression"
                                new JProperty("operator", jsOperator)
                                new JProperty("left", astToJsAstRec boolExpr.leftHandSide)
                                new JProperty("right", astToJsAstRec boolExpr.rightHandSide)
                            ])

            | ExpressionList (e, e') -> 
                let selfInvokingFunc = 
                    let blockStatement =
                        new JObject([
                                    typeProp "BlockStatement"
                                    new JProperty("body", 
                                        new JArray([
                                                   astToJsAstRec e |> inExpressionStatement
                                                   new JObject([
                                                               typeProp "ReturnStatement"
                                                               new JProperty("argument", astToJsAstRec e')
                                                              ])
                                                  ])
                                        )
                                    ])


                    new JObject([
                                typeProp "FunctionExpression"
                                new JProperty("id", null)
                                new JProperty("params", new JArray([]))
                                new JProperty("defaults", new JArray([]))
                                new JProperty("body", blockStatement)
                                new JProperty("generator", false)
                                new JProperty("expression", false)
                                ])

                callExpr selfInvokingFunc None 

            | FunctionDecl funcDecl -> 
                let idObj = identObj funcDecl.funcName
                let paramsObj = new JArray([identObj funcDecl.argName])
                let innerBody = astToJsAstRec funcDecl.body

                let blockStatement = 
                    new JObject([
                                typeProp "BlockStatement"
                                new JProperty("body", 
                                    new JArray([
                                               new JObject([typeProp "ReturnStatement"; new JProperty("argument", innerBody)])
                                              ])
                                    )
                                ])

                let varDeclaration = 
                    let funcExpression = 
                        new JObject([
                                    typeProp "FunctionExpression"
                                    new JProperty("id", idObj)
                                    new JProperty("params", paramsObj)
                                    new JProperty("defaults", new JArray([]))
                                    new JProperty("body", blockStatement)
                                    new JProperty("generator", false)
                                    new JProperty("expression", false)
                                    ])

                    new JObject([
                                typeProp "VariableDeclarator"
                                new JProperty("id", idObj)
                                new JProperty("init", funcExpression)
                                ])

                new JObject([
                            typeProp "VariableDeclaration"
                            new JProperty("declarations", new JArray([varDeclaration]))
                            new JProperty("kind", "var")
                            ])

            (* TODO: This is a failure to use the type system properly. *)
            | Unknown -> JsAstGenerationError "An AST should not be of type Unknown" |> raise

        let body = 
            let innerJsAstWrapped = 
                let innerJsAst = astToJsAstRec ast
                (* This is a bit of a hack. *)
                if (innerJsAst.GetValue "type").ToString() = "VariableDeclaration" 
                then innerJsAst 
                else inExpressionStatement innerJsAst

            new JArray([innerJsAstWrapped])

        new JObject([new JProperty("type", "Program"); new JProperty("body", body)])
        |> JsonConvert.SerializeObject
        |> Some

