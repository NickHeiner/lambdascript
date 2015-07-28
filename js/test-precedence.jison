%lex

%%
\s+                   /* skip whitespace */
\w+                   return 'WORD';

/lex

%left WORD

%start expressions

%%
e
    : e e
        {$$ = $1 + '(' + $2 + ')';}
    | WORD
        {$$ = $1;}
    ;

expressions
    : e
        { return $1;}
    ;
