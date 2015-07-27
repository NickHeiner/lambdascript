/* description: Compiles LambdaScript. */

/* lexical grammar */
%lex

%x string
%x string-escape
%x string-regex-lookup

%%
\s+               /* skip whitespace */

\<                 { return 'OPEN_ANGLE_BRACKET'; }
\>                 { return 'CLOSE_ANGLE_BRACKET'; }
(is|and|or)        { return 'BOOLEAN_OPERATOR'; }

["]                { this.begin('string'); return 'STRING_START'; }
<string>[\\]       { this.begin('string-escape'); }
<string-escape>["] { this.popState(); return 'ESCAPED_QUOTE'; }
<string>["]        { this.popState(); return 'STRING_END'; }
<string>.          { return 'STRING_CHAR'; }

\[\/.*\/\]         { yytext = yytext.substring(2, yyleng-2); return 'STRING_REGEX_LOOKUP'; }

/* Does making this a lambda break it? */
f                  { return 'FUNC_DECL_START'; }
\.                 { return 'FUNC_DOT'; }

\S+                { return 'IDENTIFIER'; }

/lex

/* I hope that splitting this declaration into multiple lines doesn't cause problems – I'm just doing this for line len.
%token OPEN_ANGLE_BRACKET CLOSE_ANGLE_BRACKET BOOLEAN_OPERATOR STRING_START ESCAPED_QUOTE STRING_END STRING_CHAR
%token FUNC_DECL_START FUNC_DOT

%right STRING_REGEX_LOOKUP
%right IDENTIFIER

/* operator associations and precedence */
/* TODO */

%start program

%% /* language grammar */
/* TODO: There are some shift/reduce conflicts in here – it would be better to sort those out. */

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

e
    : IDENTIFIER e
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
    | OPEN_ANGLE_BRACKET e BOOLEAN_OPERATOR e CLOSE_ANGLE_BRACKET
        {
            $$ = {
                type: 'Boolean',
                left: $2,
                operator: $3,
                right: $4
            }
        }
    | e STRING_REGEX_LOOKUP
        {
            $$ = {
                type: 'StringRegexLookup',
                source: $1,
                regex: $2
            };
        }
    | STRING_START string_chars STRING_END
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
    | FUNC_DECL_START IDENTIFIER IDENTIFIER FUNC_DOT e
        {
            $$ = {
                type: 'FunctionDeclaration',
                funcName: $2,
                argName: $3,
                body: $5
            };
        }
    ;

program
    : e { return $1; }
    ;
