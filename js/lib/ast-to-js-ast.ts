interface ILambdaScriptAstNode {
    // I would like to make this an enum but was not able to fully make it work,
    // because what we are getting from jison is a string.
    type: string;
}

interface IFunctionInvocation extends ILambdaScriptAstNode {
    func: ILambdaScriptAstNode;
    arg: ILambdaScriptAstNode;
}

interface ILiteral extends ILambdaScriptAstNode {
    value: string;
}

interface IIdentifier extends ILambdaScriptAstNode {
    name: string;
}

interface IBoolean extends ILambdaScriptAstNode {
    left: ILambdaScriptAstNode;
    operator: string;
    right: ILambdaScriptAstNode;
}

interface IStringRegexLookup extends ILambdaScriptAstNode {
    source: ILambdaScriptAstNode;
    regex: string;
}

interface IAstToJsAstError extends Error {
    ast: ILambdaScriptAstNode;
}

function callExpression(callee: ESTree.Expression|ESTree.Super, args: Array<ESTree.Expression>): ESTree.CallExpression {
    return {
        type: 'CallExpression',
        callee: callee,
        arguments: args
    };
}

// Why is this necessary?
interface ILsOperatorToJsOperatorMap {
    [key: string]: string;
}

function astToJsAst(ast: ILambdaScriptAstNode): ESTree.Program {
    function astToJsAstRec(ast: ILambdaScriptAstNode): ESTree.Expression {
        switch (ast.type) {
            case 'FunctionInvocation':
                const funcInvocationAst = <IFunctionInvocation> ast,
                    exprStatement: ESTree.ExpressionStatement = {
                        type: 'ExpressionStatement',
                        expression: callExpression(
                            astToJsAstRec(funcInvocationAst.func),
                            [astToJsAstRec(funcInvocationAst.arg)]
                        )
                    };
                return exprStatement;

            case 'Literal':
                const litAst = <ILiteral> ast,
                    literal: ESTree.Literal = {value: litAst.value, type: 'Literal'};
                return literal;

            case 'Identifier':
                const identAst = <IIdentifier> ast,
                    identifier: ESTree.Identifier = {name: identAst.name, type: 'Identifier'};
                return identifier;

            case 'Boolean':
                const booleanAst = <IBoolean> ast,
                    jsOperatorOfLsOperator: ILsOperatorToJsOperatorMap = {
                        is: '===',
                        and: '&&',
                        or: '||'
                    },
                    boolean: ESTree.BinaryExpression = {
                        type: 'BinaryExpression',
                        left: astToJsAstRec(booleanAst.left),
                        operator: jsOperatorOfLsOperator[booleanAst.operator],
                        right: astToJsAstRec(booleanAst.right)
                    };

                return boolean;

            case 'StringRegexLookup':
                const stringRegexLookupAst = <IStringRegexLookup> ast,
                    stringRegexLookup: ESTree.CallExpression = {
                        type: 'CallExpression',
                        callee: {
                            type: 'Identifier',

                            // TODO This will prevent anyone from having a function with this name in their code.
                            // In general, lsc does not provide any protection from the nodejs environment.
                            name: '__stringRegexLookup'
                        },
                        arguments: [
                            astToJsAstRec(stringRegexLookupAst.source),
                             {
                                type: 'Literal',
                                value: stringRegexLookupAst.regex
                             }
                        ]
                    };

                return stringRegexLookup;

            default:
                let err = <IAstToJsAstError>new Error(`AST node type not implemented: ${JSON.stringify(ast)}`);
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
