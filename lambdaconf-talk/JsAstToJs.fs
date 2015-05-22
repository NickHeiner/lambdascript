module JsAstToJs

open EdgeJs
open Util

let jsAstToJs jsAstOpt = 
    logStep "transforming js AST to js"
    match jsAstOpt with
    | None -> None
    | Some json ->
        let edgeFunc = Edge.Func @"
            const escodegen = require('escodegen');

            return function(jsAstStr, cb) {
                const jsAst = JSON.parse(jsAstStr),
                    generatedCode = escodegen.generate(jsAst);

                return cb(null, generatedCode);
            };
        "
    
        let task = edgeFunc.Invoke json
        task |> Async.AwaitTask |> ignore
        task.Result :?> string |> Some