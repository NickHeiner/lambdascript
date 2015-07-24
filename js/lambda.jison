/* description: Compiles LambdaScript. */

/* lexical grammar */
%lex

%s string

%%
\s+               /* skip whitespace */

["]               { this.begin('STRING'); return 'STRING_START'; }
<string>[^(\")]   { return 'STRING_CHAR'; }
<string>[\"]      { return 'ESCAPED_QUOTE'; }
<string>["]       { this.popState(); return 'STRING_END'; }

print             { return 'IDENTIFIER'; }

/lex

/* operator associations and precedence */
/* TODO */

%start e

%% /* language grammar */

string_chars
    : 'STRING_CHAR'
        {
            return [$1];
        }
    | 'STRING_CHAR' string_chars
        {
            return [$1].concat($2);
        }
    ;

literal
    : 'STRING_START' string_chars 'STRING_END'
        {
            return {
                type: 'Literal',
                value: $2.join('')
            };
        }
    ;

e
    : 'IDENTIFIER' literal
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
