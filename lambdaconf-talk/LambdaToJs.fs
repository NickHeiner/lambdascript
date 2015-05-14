module LambdaToJs

open Tokenize
open Lex
open Grammar
open BottomUpParse
open CstToAst
open AstToJsAst
open JsAstToJs

let lambdaToJs =
    tokenize
    >> lex
    >> bottomUpParse
    >> cstToAst
    >> astToJsAst
    >> jsAstToJs