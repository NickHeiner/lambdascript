module AddStdLibTest

open NUnit.Framework
open AddStdLib

[<Test>]
let ``addStdLib - prepends necessary js`` () =
    let expected = "'use strict';\nvar print = console.log.bind(console);\nprint('hi');"
    let actual = "print('hi');" |> Some |> addStdLib |> Option.get
    
    Assert.AreEqual(expected, actual) 
