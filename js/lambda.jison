/* Jison-formatted lex and parse rules for LambdaScript */

%token OPEN_ANGLE_BRACKET CLOSE_ANGLE_BRACKET IS STRING_LITERAL IDENTIFIER

%lex
%%

\s+         {/* skip whitespace */}
<           { return OPEN_ANGLE_BRACKET;}
>           { return OPEN_ANGLE_BRACKET;}

/* This needs to be made more sophisticated */
".*"        { return STRING_LITERAL;}

is          { return IS;}

.*          { return IDENTIFIER;}

/lex
