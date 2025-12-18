grammar MiniLanguage;

// --- REGULI PARSER (Sintaxă) ---
program: declaration* EOF;

declaration: varDecl | funcDecl;

// Cerința: Declarații de variabile (globale și locale)
varDecl: type ID ('=' expression)? ';';

// Cerința: Declarații de funcții cu parametri
funcDecl: (type | VOID) ID '(' paramList? ')' block;

paramList: param (',' param)*;
param: type ID;

block: '{' statement* '}';

// Cerința: Instrucțiuni (if, for, while, return, atribuiri, apeluri)
statement: varDecl                                     # VarDeclaration
         | ID ASSIGN expression ';'                    # Assignment
         | ID (PLUS_ASSIGN | MINUS_ASSIGN | MULT_ASSIGN | DIV_ASSIGN | MOD_ASSIGN) expression ';' # OpAssignment
         | ID (INC | DEC) ';'                          # IncrementDecrement
         | IF '(' expression ')' block (ELSE block)?   # IfStat
         | FOR '(' varDecl expression ';' ID ASSIGN expression ')' block # ForStat
         | WHILE '(' expression ')' block              # WhileStat
         | RETURN expression? ';'                      # ReturnStat
         | ID '(' argList? ')' ';'                     # FuncCall;

argList: expression (',' expression)*;

// Cerința: Expresii (aritmetice, relaționale, logice)
expression: expression (MULT | DIV | MOD) expression   # MulDivMod
          | expression (PLUS | MINUS) expression       # AddSub
          | expression (LT | GT | LE | GE | EQ | NE) expression # Relational
          | expression (AND | OR) expression           # Logical
          | NOT expression                             # NotExpr
          | '(' expression ')'                         # Parenthesis
          | ID                                         # IdentifierExpr
          | INT_LIT                                    # IntLiteral
          | FLOAT_LIT                                  # FloatLiteral
          | STRING_LIT                                 # StringLiteral
          | ID '(' argList? ')'                        # FuncCallExpr;

type: INT | FLOAT | DOUBLE | STRING | CONST type;

// --- REGULI LEXER (Unități Lexicale) ---

// Cuvinte cheie
INT: 'int';
FLOAT: 'float';
DOUBLE: 'double';
STRING: 'string';
CONST: 'const';
VOID: 'void';
IF: 'if';
ELSE: 'else';
FOR: 'for';
WHILE: 'while';
RETURN: 'return';

// Operatori aritmetici
PLUS: '+';
MINUS: '-';
MULT: '*';
DIV: '/';
MOD: '%';

// Operatori de atribuire
ASSIGN: '=';
PLUS_ASSIGN: '+=';
MINUS_ASSIGN: '-=';
MULT_ASSIGN: '*=';
DIV_ASSIGN: '/=';
MOD_ASSIGN: '%=';

// Operatori incrementare/decrementare
INC: '++';
DEC: '--';

// Operatori relaționali
EQ: '==';
NE: '!=';
LT: '<';
GT: '>';
LE: '<=';
GE: '>=';

// Operatori logici
AND: '&&';
OR: '||';
NOT: '!';

// Delimitatori
LPAREN: '(';
RPAREN: ')';
LBRACE: '{';
RBRACE: '}';
COMMA: ',';
SEMICOLON: ';';

// Identificatori și Literali
ID: [a-zA-Z_][a-zA-Z0-9_]*;
INT_LIT: [0-9]+;
FLOAT_LIT: [0-9]+ '.' [0-9]+;
STRING_LIT: '"' ( ~["\r\n\\] | '\\' . )* '"';

// Ignorarea spațiilor și a comentariilor (// și /* */)
WS: [ \t\r\n]+ -> skip;
LINE_COMMENT: '//' ~[\r\n]* -> skip;
BLOCK_COMMENT: '/*' .*? '*/' -> skip;

// Eroare lexicală: caractere invalide
INVALID_CHAR: . ;