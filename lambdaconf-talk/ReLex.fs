module ReLex

open Lex

let reLex = 
    List.fold (fun acc el -> 
        let nextElem = 
            match el with
            | Identifier ident -> 
                match acc with
                | [] -> el
                | hd::tl -> match hd with
                            | Lambda -> FuncName ident
                            | FuncName _ -> ArgName ident
                            | _ -> el
            | _ -> el

        nextElem::acc
    ) [] 
    >> List.rev

