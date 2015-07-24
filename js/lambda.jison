/* description: Compiles LambdaScript. */

/* lexical grammar */
%lex

%%
\s+               /* skip whitespace */
hello\-world     {return 'LITERAL';}
print             { return 'IDENTIFIER'; }

/lex

/* operator associations and precedence */
/* TODO */

%start e

%% /* language grammar */

e
    : 'IDENTIFIER' e
        {
            return {
                type: 'FunctionInvocation',
                func: {type: 'Identifier', name: $1},
                arg: $2
            };
        }
    | 'LITERAL'
        {
            return {
                type: 'Literal',
                value: $1
            };
        }
    ;
