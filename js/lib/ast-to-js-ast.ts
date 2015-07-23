/// <reference path="./lambda-script-ast.d.ts" />

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
            case NodeType.FunctionInvocation:
                const funcInvocationAst = <IFunctionInvocation>ast,
                    exprStatement: ESTree.ExpressionStatement = {
                        type: 'ExpressionStatement',
                        expression: callExpression(
                            astToJsAstRec(funcInvocationAst.func),
                            [astToJsAstRec(funcInvocationAst.arg)]
                        )
                    };
                return exprStatement;

            case NodeType.Literal:
                const litAst = <ILiteral>ast,
                    literal: ESTree.Literal = {value: litAst.val, type: 'Literal'};
                return literal;

            case NodeType.Identifier:
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
