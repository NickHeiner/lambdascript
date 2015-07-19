/* description: Parses end executes mathematical expressions. */

%token OPEN_ANGLE_BRACKET

/* lexical grammar */
%lex

%%
\s+                   /* skip whitespace */
"<"                   {return OPEN_ANGLE_BRACKET;}

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
    : OPEN_ANGLE_BRACKET
    ;
