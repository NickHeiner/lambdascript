/// <reference path="./lambda-script-ast.d.ts" />

const traverse = require('traverse'),
    _ = require('lodash'),

    nodeTypeInverted = _.invert(NodeType);

function jisonOutputToLambdaScriptAst(jisonOutput: Object): ILambdaScriptAstNode {
    return traverse.map(jisonOutput, function(node) {
        if (_.has(node, 'type')) {
            this.update(_.merge({}, node, {type: nodeTypeInverted[node.type]}))
        }
    });
}

export = jisonOutputToLambdaScriptAst;
