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
    let actual = tokenize ["λ x . 1"]
    let expected = ["λ"; "x"; "."; "1"]
    Assert.AreEqual(expected, actual)

[<Test>]
let ``tokenize - many spaces`` () = 
    let actual = tokenize ["λ     x  .   1"]
    let expected = ["λ"; "x"; "."; "1"]
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