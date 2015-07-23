interface ILambdaScriptAst {
    // This is just a parent type, with no content of its own. Is there a better way to express this?
}

interface IFunctionInvocationAstNode extends ILambdaScriptAst {
    func: ILambdaScriptAst,
    arg: ILambdaScriptAst
}

function astToJsAst(ast: IFunctionInvocationAstNode) {
    return {
        "type": "Program",
        "body": [
            {
                "type": "ExpressionStatement",
                "expression": {
                    "type": "CallExpression",
                    "callee": {
                        "type": "MemberExpression",
                        "computed": false,
                        "object": {
                            "type": "Identifier",
                            "name": "console"
                        },
                        "property": {
                            "type": "Identifier",
                            "name": "log"
                        }
                    },
                    "arguments": [
                        {
                            "type": "Literal",
                            "value": "hi",
                            "raw": "'hi'"
                        }
                    ]
                }
            }
        ]
    };
}

export = astToJsAst;
