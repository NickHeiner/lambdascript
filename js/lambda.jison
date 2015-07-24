/* description: Compiles LambdaScript. */

/* lexical grammar */
%lex

%x string
%x string-escape

%%
\s+               /* skip whitespace */

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
