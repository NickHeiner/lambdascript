module JsAstToJs

open EdgeJs

let jsAstToJs _ = 
    let edgeFunc = Edge.Func @"
        return function(data, cb) {
            return cb(null, 1 + 1);
        };
    "

    let task = edgeFunc.Invoke "no data to pass in right now"
    task |> Async.AwaitTask |> ignore
    task.Result