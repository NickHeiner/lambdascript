interface ILambdaScriptAstNode {
    // I would like to make this an enum but was not able to fully make it work,
    // because what we are getting from jison is a string.
    type: string
}

interface IFunctionInvocation extends ILambdaScriptAstNode {
    func: ILambdaScriptAstNode;
    arg: ILambdaScriptAstNode;
}

interface ILiteral extends ILambdaScriptAstNode {
    value: string
}

interface IIdentifier extends ILambdaScriptAstNode {
    name: string
}

interface AstToJsAstError extends Error {
    ast: ILambdaScriptAstNode
}

function callExpression(callee: ESTree.Expression|ESTree.Super, args: Array<ESTree.Expression>): ESTree.CallExpression {
    return {
        type: 'CallExpression',
        callee: callee,
        arguments: args
    };
}

function astToJsAst(ast: ILambdaScriptAstNode): ESTree.Program {
    function astToJsAstRec(ast: ILambdaScriptAstNode): ESTree.Statement {
        switch (ast.type) {
            case 'FunctionInvocation':
                const funcInvocationAst = <IFunctionInvocation>ast,
                    exprStatement: ESTree.ExpressionStatement = {
                        type: 'ExpressionStatement',
                        expression: callExpression(
                            astToJsAstRec(funcInvocationAst.func),
                            [astToJsAstRec(funcInvocationAst.arg)]
                        )
                    };
                return exprStatement;

            case 'Literal':
                const litAst = <ILiteral>ast,
                    literal: ESTree.Literal = {value: litAst.value, type: 'Literal'};
                return literal;

            case 'Identifier':
                const identAst = <IIdentifier>ast,
                    identifier: ESTree.Identifier = {name: identAst.name, type: 'Identifier'};
                return identifier;

            default:
                let err = <AstToJsAstError>new Error(`AST node type not implemented: ${JSON.stringify(ast)}`);
                err.ast = ast;
                throw err;
        }
    }
    
    return {
        type: 'Program',
        body: [astToJsAstRec(ast)],

        sourceType: 'I am not sure what this value is supposed to be'
    };
}

export = astToJsAst;
