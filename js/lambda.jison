/* Compiles LambdaScript. */

/* lexical grammar */
%lex

%x string
%x string-escape
%x string-regex-lookup

%%
\s+               /* skip whitespace */

\<                 return 'OPEN_ANGLE_BRACKET';
\>                 return 'CLOSE_ANGLE_BRACKET';
(is|and|or)        return 'BOOLEAN_OPERATOR';

["]                { this.begin('string'); return 'STRING_START'; }
<string>[\\]       this.begin('string-escape');
<string-escape>["] { this.popState(); return 'ESCAPED_QUOTE'; }
<string>["]        { this.popState(); return 'STRING_END'; }
<string>.          return 'STRING_CHAR';

\[\/.*\/\]         { yytext = yytext.substring(2, yyleng-2); return 'STRING_REGEX_LOOKUP'; }

/* Does making this a lambda break it? */
f                  return 'FUNC_DECL_START';
\.                 return 'FUNC_DOT';
/* This is intentionally ugly so I remember to fix it. */
\}                 return 'FUNC_END';

\;                 return 'EXPRESSION_SEP';
\w+                return 'IDENTIFIER';


/lex

%ebnf

%start program

%% /* language grammar */
/* TODO: There are some shift/reduce conflicts in here – it would be better to sort those out. */

e
    : OPEN_ANGLE_BRACKET e CLOSE_ANGLE_BRACKET
        {
            $$ = $2;
        }
    | IDENTIFIER
        {
            $$ = {
                type: 'Identifier',
                name: $1
            };
        }
    | e e
        {
            $$ = {
                type: 'FunctionInvocation',
                func: $1,
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
    | STRING_START (STRING_CHAR|ESCAPED_QUOTE)* STRING_END
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
    | FUNC_DECL_START IDENTIFIER IDENTIFIER FUNC_DOT e FUNC_END
        {
            $$ = {
                type: 'FunctionDeclaration',
                funcName: $2,
                argName: $3,
                body: $5
            };
        }
    | e EXPRESSION_SEP e
        {
            $$ = {
                type: 'ExpressionList',
                expr1: $1,
                expr2: $3
            };
        }
    ;

program
    : e { return $1; }
    ;
