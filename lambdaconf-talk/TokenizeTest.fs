module TokenizeTest

open Tokenize
open NUnit.Framework

[<Test>]
let ``tokenize - no input`` () = 
    let actual = tokenize []
    let expected = []
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``tokenize - some input`` () = 
    let actual = tokenize ["λ f x . x"]
    let expected = ["λ"; "f"; "x"; "."; "x"]
    Assert.AreEqual(expected, actual)

[<Test>]
let ``tokenize - many spaces`` () = 
    let actual = tokenize ["λ     x ignoredArg  .   1"]
    let expected = ["λ"; "x"; "ignoredArg"; "."; "1"]
    Assert.AreEqual(expected, actual)

[<Test>]
let ``tokenize - function invocation`` () =
    let actual = tokenize ["print <isPalindrome \"racecar\">"]
    let expected = ["print"; "<"; "isPalindrome"; "\"racecar\""; ">"]
    Assert.AreEqual(expected, actual) 

[<Test>]
let ``tokenize - sample.lambda`` () =
    let actual = tokenize [ "λ isPalindrome str ."
                            "str is \"\" or <"
                            "str[/^(.)/] is str[/.*(.)$/]"
                            "and isPalindrome str[/^.(.*)./]"
                            ">"]
    let expected = ["λ"; "isPalindrome"; "str"; "."; "str"; "is"; "\"\""; "or"; "<";
        "str"; "["; "/^(.)/"; "]"; "is"; "str"; "["; "/.*(.)$/"; "]"; "and"; "isPalindrome";
        "str"; "["; "/^.(.*)./"; "]"; ">"]

    Assert.AreEqual(expected, actual)

[<Test>]
let ``tokenize - expression list`` () =
    let actual = tokenize ["""λ f x . x; f "hello" """]
    let expected = ["λ"; "f"; "x"; "."; "x"; ";"; "f"; "\"hello\""]

    Assert.AreEqual(expected, actual)