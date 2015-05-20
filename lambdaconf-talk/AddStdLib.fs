module AddStdLib

let addStdLib = function
    (* It would be more optimal to statically determine which parts of the std lib were necessary
       and only include those, instead of dumping it in every time.
    *)
    | Some code -> "var print = console.log.bind(console);\n" + code |> Some
    | None -> None