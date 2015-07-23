/* description: Compiles LambdaScript. */

/* lexical grammar */
%lex

%%
\s+               /* skip whitespace */
hi                {return 'LITERAL';}
print             { return 'IDENTIFIER'; }

/lex

/* operator associations and precedence */
/* TODO */

%start expressions

%% /* language grammar */

expressions
    : e
        {return $1;}
    ;

e
    : e e
        {
            return {
                type: 'FunctionInvocation',
                func: $1,
                arg: $2
            };
        }
    | 'IDENTIFIER'
        {
            return {
                type: 'Identifier',
                name: $1
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
