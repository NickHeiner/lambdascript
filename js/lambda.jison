/* description: Compiles LambdaScript. */

%token IDENTIFIER STRING_LITERAL

/* lexical grammar */
%lex

%%
\s+                   /* skip whitespace */
\".*\"                {return STRING_LITERAL;}

.*                    { return IDENTIFIER; }

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
    : IDENTIFIER STRING_LITERAL
    ;
