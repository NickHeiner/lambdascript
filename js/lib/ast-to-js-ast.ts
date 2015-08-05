const _ = require('lodash');

import '../types';

function single<T>(arr: Array<T>): T {
    if (arr.length > 1) {
        throw new Error('Expected array to have only a single element, but was: `' + arr + '`');
    }

    return arr[0];
}

function callExpression(callee: ESTree.Expression|ESTree.Super, args: Array<ESTree.Expression>): ESTree.CallExpression {
    return {
        type: 'CallExpression',
        callee: callee,
        arguments: args
    };
}

function astToJsAst(ast: ILambdaScriptAstNode): ESTree.Program {
    function astToJsAstRec(ast: ILambdaScriptAstNode): Array<ESTree.Expression|ESTree.Statement> {
        switch (ast.type) {
            case 'FunctionInvocation':
                const funcInvocationAst = <IFunctionInvocation> ast,
                    callExpr = callExpression(
                        single(astToJsAstRec(funcInvocationAst.func)),
                        astToJsAstRec(funcInvocationAst.arg)
                    );
                return [callExpr];

            case 'Literal':
                const litAst = <ILiteral> ast,
                    literal: ESTree.Literal = {value: litAst.value, type: 'Literal'};
                return [literal];

            case 'Identifier':
                const identAst = <IIdentifier> ast,
                    identifier: ESTree.Identifier = {name: identAst.name, type: 'Identifier'};
                return [identifier];

            case 'Boolean':
                const booleanAst = <IBoolean> ast,
                    jsOperatorOfLsOperator: ILsOperatorToJsOperatorMap = {
                        is: '===',
                        and: '&&',
                        or: '||'
                    },
                    booleanExpr: ESTree.BinaryExpression = {
                        type: 'BinaryExpression',
                        left: single(astToJsAstRec(booleanAst.left)),
                        operator: jsOperatorOfLsOperator[booleanAst.operator],
                        right: single(astToJsAstRec(booleanAst.right))
                    };

                return [booleanExpr];

            case 'StringRegexLookup':
                const stringRegexLookupAst = <IStringRegexLookup> ast,
                    stringRegexLookup = callExpression(
                        {
                            type: 'Identifier',

                            // TODO This will prevent anyone from having a function with this name in their code.
                            // In general, lsc does not provide any protection from the nodejs environment.
                            name: '__stringRegexLookup'
                        },
                        [
                            single(astToJsAstRec(stringRegexLookupAst.source)),
                             {
                                type: 'Literal',
                                value: stringRegexLookupAst.regex
                             }
                        ]
                    );

                return [stringRegexLookup];

            case 'FunctionDeclaration':
                const funcDeclAst = <IFunctionDeclaration> ast,
                    funcDecl: ESTree.FunctionDeclaration = {
                        type: 'FunctionDeclaration',
                        id: {
                            type: 'Identifier',
                            name: funcDeclAst.funcName
                        },
                        params: [{
                            type: 'Identifier',
                            name: funcDeclAst.argName
                        }],
                        body: {
                            type: 'BlockStatement',
                            body: [{
                                type: 'ReturnStatement',
                                argument: single(astToJsAstRec(funcDeclAst.body))
                            }]
                        },
                        generator: false
                    };

                return [funcDecl];

            case 'ExpressionList':
                const exprListAst = <IExpressionList> ast,
                    expressions = astToJsAstRec(exprListAst.expr1).concat(astToJsAstRec(exprListAst.expr2));
                return _.map(expressions, function(expr: ESTree.Expression|ESTree.Statement) {
                    if (expr.type === 'CallExpression') {
                        return {
                            type: 'ExpressionStatement',
                            expression: expr
                        };
                    }

                    return expr;
                });

            default:
                let err = <IAstToJsAstError>new Error(`AST node type not implemented: ${JSON.stringify(ast)}`);
                err.ast = ast;
                throw err;
        }
    }
    
    return {
        type: 'Program',
        body: astToJsAstRec(ast),

        sourceType: 'I am not sure what this value is supposed to be'
    };
}

export = astToJsAst;
