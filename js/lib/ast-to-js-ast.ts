type LambdaScriptAst = IFunctionInvocationAstNode

class IFunctionInvocationAstNode {
    func: LambdaScriptAst;
    arg: LambdaScriptAst;
}

interface AstToJsAstError extends Error {
    ast: LambdaScriptAst
}

function callExpression(callee: ESTree.Expression|ESTree.Super, args: Array<ESTree.Expression>): ESTree.CallExpression {
    return {
        type: 'CallExpression',
        callee: callee,
        arguments: args
    };
}

function astToJsAst(ast: LambdaScriptAst): ESTree.Program {
    function astToJsAstRec(ast: LambdaScriptAst): ESTree.Statement {
        if (ast instanceof IFunctionInvocationAstNode) {
            const exprStatement: ESTree.ExpressionStatement = {
                type: 'ExpressionStatement',
                expression: callExpression(astToJsAstRec(ast.func), [astToJsAstRec(ast.arg)])
            };
            return exprStatement;
        } else {
            let err = <AstToJsAstError>new Error('AST node type not implemented');
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
