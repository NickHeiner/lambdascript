module AddStdLib

let addStdLib = function
    (* It would be more optimal to statically determine which parts of the std lib were necessary
       and only include those, instead of dumping it in every time.
    *)
    | Some code -> """
'use strict';

function stringLookup(str, regex) {
    var match = str.match(regex);
    if (match) {
        return match[1];
    }

    return '';
}

var print = console.log.bind(console);""" + code |> Some
    | None -> None