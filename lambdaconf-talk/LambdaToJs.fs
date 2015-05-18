module LambdaToJs

open Tokenize
open Lex
open ReLex
open Grammar
open BottomUpParse
open CstToAst
open AstToJsAst
open JsAstToJs

let lambdaToJs =
    tokenize
    >> lex
    >> reLex
    >> bottomUpParse
    >> cstToAst
    >> astToJsAst
    >> jsAstToJs