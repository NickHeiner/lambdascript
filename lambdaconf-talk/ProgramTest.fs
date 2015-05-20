module ProgramTest

open NUnit.Framework

[<Test>]
let ``program - run sample.lambda`` () =
    let testProc = 
        System.Diagnostics.Process.Start
            ("lambdaconf_talk.exe", "..\..\sample.lambda")

    testProc.WaitForExit()

    Assert.AreEqual(0, testProc.ExitCode)

    testProc.Dispose()