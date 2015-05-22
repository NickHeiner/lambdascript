module ProgramTest

open System.Diagnostics
open NUnit.Framework

[<Test>]
let ``program - run sample.lambda`` () =
    let testProc = 
        let startInfo = new ProcessStartInfo("lambdaconf_talk.exe", "..\..\sample.lambda")
        startInfo.CreateNoWindow <- true
        Process.Start startInfo

    testProc.WaitForExit()

    Assert.AreEqual(0, testProc.ExitCode)

    testProc.Dispose()