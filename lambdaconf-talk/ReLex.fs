module ReLex

open Lex

let reLex = 
    (*
        This is a bit of a hack.

        If we see the following input:

            λ f x . x

        We would lex it as:

            [Lambda; Identifier "f"; Identifier "x"; FuncDot; Identifier "x"]

        When we go to bottom-up parse, we will start with an empty parse stack:

            []

        This does not match any grammar rules, so we shift from our input:

            [Lambda]

        This does not match any grammar rules, so we shift from our input:

            [Lambda; Identifier "f"]

        Ah-ha! Popping off the stack, we see that Identifier "f" matches the following rule:

            Expression -> Identifier

        We reduce:

            [Lambda; Expression]

        When we keep going, we'll do the same thing for the argument, to give us:

            [Lambda; Expression; Expression]

        Of course, an Expression followed by an Expression is a function invocation, so we get:

            [Lambda; FuncInvocation]

        And a Lambda followed by a FuncInvocation is ungrammatical, so we throw a parse error.

        To avoid this mess, we need to add a bit of context sensitivity to the lexer. To avoid polluting
        the original lexer, we will do it as a separate phase in this function. We look for a lambda 
        followed by two identifiers and rewrite it to be a lambda followed by a func name and an arg name.

        It is not obvious that this is the best approach, but it doesn't feel awful, either. Maybe that's
        just the price you pay for having a spare language that doesn't have a bunch of extraneous 
        purely syntactic tokens.
    *)
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

