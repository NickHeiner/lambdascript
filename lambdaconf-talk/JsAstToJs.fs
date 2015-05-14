module JsAstToJs

open EdgeJs

let jsAstToJs = function
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
        Some task.Result