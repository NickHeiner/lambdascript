/* description: Compiles LambdaScript. */

/* lexical grammar */
%lex

%%
\s+               /* skip whitespace */
hi                {return 'STRING_LITERAL';}
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
    : 'IDENTIFIER' 'STRING_LITERAL'
        {
            return {
                type: 'FunctionInvocation',
                func: $1,
                arg: $2
            };
        }
    ;
