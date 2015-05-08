module TokenizeTest

open Tokenize
open NUnit.Framework

[<Test>]
let ``tokenize - no input``() = 
    let actual = tokenize []
    let expected = []
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``tokenize - some input``() = 
    let actual = tokenize ["λ x -> 1"]
    let expected = ["λ"; "x"; "->"; "1"]
    Assert.AreEqual(expected, actual)

[<Test>]
let ``tokenize - many spaces``() = 
    let actual = tokenize ["λ     x  ->   1"]
    let expected = ["λ"; "x"; "->"; "1"]
    Assert.AreEqual(expected, actual)