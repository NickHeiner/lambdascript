/* description: Compiles LambdaScript. */

/* lexical grammar */
%lex

%x string
%x string-escape

%%
\s+               /* skip whitespace */

\<                 { return '<'; }
\>                 { return '>'; }
(is|and|or)        { return 'BOOLEAN_OPERATOR'; }

["]                { this.begin('string'); return 'STRING_START'; }
<string>[\\]       { this.begin('string-escape'); }
<string-escape>["] { this.popState(); return 'ESCAPED_QUOTE'; }
<string>["]        { this.popState(); return 'STRING_END'; }
<string>.          { return 'STRING_CHAR'; }

print              { return 'IDENTIFIER'; }

/lex

/* operator associations and precedence */
/* TODO */

%start program

%% /* language grammar */

string_chars
    : ESCAPED_QUOTE string_chars
        {
            $$ = [$1].concat($2);
        }
    | STRING_CHAR string_chars
        {
            $$ = [$1].concat($2);
        }
    | STRING_CHAR
        {
            $$ = [$1];
        }
    ;

literal
    : STRING_START string_chars STRING_END
        {
            $$ = {
                type: 'Literal',
                value: $2.join('')
            };
        }
    | STRING_START STRING_END
        {
          $$ = {
              type: 'Literal',
              value: ''
          };
        }
    ;

boolean
    : e BOOLEAN_OPERATOR e
        {
            $$ = {
                type: 'Boolean',
                left: $1,
                operator: $2,
                right: $3
            }
        }
    ;

e
    : IDENTIFIER literal
        {
            $$ = {
                type: 'FunctionInvocation',
                func: {
                    type: 'Identifier',
                    name: $1
                },
                arg: $2
            };
        }
    ;

program
    : e { return $1; }
    ;
