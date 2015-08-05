interface ILoc {
    first_line: number;
    last_line: number;
    first_column: number;
    last_column: number;
}

interface ILambdaScriptAstNode {
    // I would like to make this an enum but was not able to fully make it work,
    // because what we are getting from jison is a string.
    type: string;
    loc?: ILoc;
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
    regexLoc?: ILoc;
}

interface IFunctionDeclaration extends ILambdaScriptAstNode {
    funcName: string;
    argName: string;
    body: ILambdaScriptAstNode;
}

interface IExpressionList extends ILambdaScriptAstNode {
    expr1: ILambdaScriptAstNode;
    expr2: ILambdaScriptAstNode;
}

interface IAstToJsAstError extends Error {
    ast: ILambdaScriptAstNode;
}

// Why is this necessary?
interface ILsOperatorToJsOperatorMap {
    [key: string]: string;
}
