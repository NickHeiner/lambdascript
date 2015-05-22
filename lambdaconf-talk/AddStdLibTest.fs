module AddStdLibTest

open NUnit.Framework
open AddStdLib

[<Test>]
let ``addStdLib - prepends necessary js`` () =
    let expected = """
'use strict';

function stringLookup(str, regex) {
    var match = str.match(regex);
    if (match) {
        return match[1];
    }

    return '';
}

var print = console.log.bind(console);print('hi');"""
    let actual = "print('hi');" |> Some |> addStdLib |> Option.get
    
    Assert.AreEqual(expected, actual) 
