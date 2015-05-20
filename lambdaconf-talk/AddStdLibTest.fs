module AddStdLibTest

open NUnit.Framework
open AddStdLib

[<Test>]
let ``addStdLib - adds print statement`` () =
    let expected = "var print = console.log.bind(console);\nprint('hi');"
    let actual = "print('hi');" |> Some |> addStdLib |> Option.get
    
    Assert.AreEqual(expected, actual) 
