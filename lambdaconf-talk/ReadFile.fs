module ReadFile

open System.IO

let GetFileContents filePath = 
    File.ReadAllLines (filePath, System.Text.Encoding.UTF8) |> Array.toList

    