/* description: Compiles LambdaScript. */

/* lexical grammar */
%lex

%x string
%x string-escape

%%
\s+               /* skip whitespace */

["]               { console.log('begin string', yytext); this.begin('string'); return 'STRING_START'; }
<string>[\\]       { console.log('begin string escape'); this.begin('string-escape'); }
<string-escape>["] { yytext = '\\' + yytext; console.log('end string escape', yytext); this.popState(); return 'ESCAPED_QUOTE'; }
<string>["]       { console.log('pop state', yytext); this.popState(); return 'STRING_END'; }
<string>.         { console.log('string char', yytext); return 'STRING_CHAR'; }

print             { return 'IDENTIFIER'; }

/lex

/* operator associations and precedence */
/* TODO */

%start e

%% /* language grammar */

string_chars
    : 'STRING_CHAR'
        {
            console.log('singleton string char');
            return [$1];
        }
    | 'ESCAPED_QUOTE' string_chars
        {
            console.log('escaped quote prepend');
            return [$1].concat($2);
        }
    | 'STRING_CHAR' string_chars
        {
            console.log('string char prepend');
            return [$1].concat($2);
        }
    ;

literal
    : 'STRING_START' string_chars 'STRING_END'
        {
            console.log('literal match');
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
