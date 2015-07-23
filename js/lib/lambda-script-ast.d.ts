// I wonder if it's an anti-pattern to have this file. Is this only for imported js?
module LambdaScriptAst {
    export const enum NodeType {
        FunctionInvocation,
        Literal,
        Identifier
    }

    export interface ILambdaScriptAstNode {
        type: NodeType
    }

    export interface IFunctionInvocation extends ILambdaScriptAstNode {
        func: ILambdaScriptAstNode;
        arg: ILambdaScriptAstNode;
    }

    export interface ILiteral extends ILambdaScriptAstNode {
        val: string
    }

    export interface IIdentifier extends ILambdaScriptAstNode {
        name: string
    }

    export interface AstToJsAstError extends Error {
        ast: ILambdaScriptAstNode
    }
}

