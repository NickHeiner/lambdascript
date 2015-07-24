/* description: Compiles LambdaScript. */

/* lexical grammar */
%lex

%%
\s+               /* skip whitespace */
\"hello\-world\"     {return 'LITERAL';}
print             { return 'IDENTIFIER'; }

/lex

/* operator associations and precedence */
/* TODO */

%start e

%% /* language grammar */

e
    : 'IDENTIFIER' 'LITERAL'
        {
            return {
                type: 'FunctionInvocation',
                func: {
                    type: 'Identifier',
                    name: $1
                },
                arg: {
                    type: 'Literal',
                    value: $2
                }
            };
        }
    ;
