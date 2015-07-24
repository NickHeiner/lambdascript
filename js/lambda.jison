/* description: Compiles LambdaScript. */

/* lexical grammar */
%lex

%x string
%x string-escape

%%
\s+               /* skip whitespace */

["]               { console.log('STRING_START', yytext); this.begin('string'); return 'STRING_START'; }
<string>[\\]       { console.log('begin string escape'); this.begin('string-escape'); }
<string-escape>["] { yytext = '\\' + yytext; console.log('end string escape', yytext); this.popState(); return 'ESCAPED_QUOTE'; }
<string>["]       { console.log('STRING_END', yytext); this.popState(); return 'STRING_END'; }
<string>.         { console.log('string char', yytext); return 'STRING_CHAR'; }

print             { return 'IDENTIFIER'; }

/lex

/* operator associations and precedence */
/* TODO */

%start program

%% /* language grammar */

string_chars
    : ESCAPED_QUOTE string_chars
        {
            console.log('escaped quote prepend');
            $$ = [$1].concat($2);
        }
    | STRING_CHAR string_chars
        {
            console.log('string char prepend');
            $$ = [$1].concat($2);
        }
    | STRING_CHAR
        {
            console.log('singleton string char');
            $$ = [$1];
        }
    ;

literal
    : STRING_START string_chars STRING_END
        {
            console.log('literal match');
            $$ = {
                type: 'Literal',
                value: $2.join('')
            };
        }
    | STRING_START STRING_END
        {
          console.log('literal match empty string');
          $$ = {
              type: 'Literal',
              value: ''
          };
        }
    ;

e
    : IDENTIFIER literal
        {
            console.log('e match');
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
