module MainTest

open NUnit.Framework

[<Test>]
let ``When 2 is added to 2 expect 4``() = 
    Assert.AreEqual(4, 2+2)

[<Test>]
let ``When 2 is added to 2 expect 5``() = 
    Assert.AreEqual(5, 2+2)