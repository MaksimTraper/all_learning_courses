%using Compiler.Helpers
%using Compiler.Nodes
%namespace Compiler
%output = ..\..\CompilerParser.cs 
%partial 
%sharetokens
%start list
%visibility internal

%YYSTYPE Node

%token LITERAL  
%token LETTER
%token PRINT
%token EVAL
%token GETTYPE
%token RESET
%token EXIT
%token HELP
%token EOL
%token SMALLER
%token BIGGER
%token SUBSTRING
%token ASSIGN
%token IF
%token MODECHANGE
%token BREAK
%token FOO
%token ADD
%token MINUS
%token DIV
%token MULT
%token NEG
%token CONDITION
%token DISJUNCTION
%token NOTEQUAL

%left UMINUS

%%

list
	:   /*empty */
    |   list statement EOL
    |   list error EOL						{ yyerrok(); }
    ;

//Подаваемые на вход предложения. Выполнение при появлении
statement	
	: sequence 								{ this.DoStat($1); }
	;

//Последовательность операторов. Выполнение по требованию
sequence	
	: action								{ $$ = MakeSequence(NodeTag.sequence, $1); }
	| sequence action 						{ $$ = MakeSequence(NodeTag.sequence, $1, $2); }
	;

//Действия над выражениями. Выполнение по требованию
action
	: ASSIGN '(' LETTER expr ')'			{ $$ = MakeBinary(NodeTag.assign, $3, $4); }
	| EXIT									{ $$ = MakeLeaf(NodeTag.exit); }
	| FOO									{ $$ = MakeLeaf(NodeTag.foo); }
	| BREAK									{ $$ = MakeLeaf(NodeTag.myBreak); }
	| LITERAL								{ $$ = $1; }
	| EVAL expr								{ $$ = MakeUnary(NodeTag.display, $2); }
	| GETTYPE expr							{ $$ = MakeUnary(NodeTag.getType, $2); }
	| MODECHANGE LITERAL					{ $$ = MakeUnary(NodeTag.changeMode, $2); }
	| HELP									{ $$ = MakeLeaf(NodeTag.help); }
	| RESET									{ $$ = MakeLeaf(NodeTag.clear); }
	| PRINT									{ $$ = MakeLeaf(NodeTag.print); }
	| '(' expr ')' CONDITION '(' sequence ')' { $$ = MakeBinary(NodeTag.condition, $2, $6); } 
	;

//Действия над операндами. Выполнение по требованию
expr	
	:  '(' expr MULT expr ')'			    { $$ = MakeBinary(NodeTag.mul, $2, $4); }
	|  '(' expr DIV expr ')'			    { $$ = MakeBinary(NodeTag.div, $2, $4); }
	|  '(' expr ADD expr ')'				{ $$ = MakeBinary(NodeTag.plus, $2, $4); }
	|  '(' expr MINUS expr ')'		        { $$ = MakeBinary(NodeTag.minus, $2, $4); }
	|  SMALLER '(' expr expr ')'		    { $$ = MakeBinary(NodeTag.smaller, $3, $4); }
	|  BIGGER '(' expr expr ')'				{ $$ = MakeBinary(NodeTag.bigger, $3, $4); }
	|  SUBSTRING '(' expr expr expr ')'		{ $$ = MakeTernary(NodeTag.subString, $3, $4, $5);}
	|  '(' expr NOTEQUAL expr ')'			{ $$ = MakeBinary(NodeTag.notequal, $2, $4); }
	|  '(' expr DISJUNCTION expr ')'		{ $$ = MakeBinary(NodeTag.disjunction, $2, $4); }
	|  '~'  expr 							{ $$ = MakeUnary(NodeTag.sqrt, $2); }
	|  LITERAL          					// $$ is automatically lexer.yylval
	|  LETTER								// $$ is automatically lexer.yylval
	|  NEG expr 							{ $$ = MakeUnary(NodeTag.negate, $2); }
	;

%%
/*
 * All the code is in the helper file RealTreeHelper.cs 
 */ 