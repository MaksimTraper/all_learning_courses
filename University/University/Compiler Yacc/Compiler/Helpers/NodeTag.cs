namespace Compiler.Helpers
{
    /// <summary> 
    /// Выполняемые операции
    /// </summary>
    internal enum NodeTag
    {
        error,          //stuff
        foo,
        literal,
        name,
        myBreak,    //math
        div,
        smaller,
        minus,
        mul,
        negate,
        bigger,
        plus,
        subString,
        assign,         //states
        changeMode,
        clear,
        display,
        exit,
        getType,
        help,
        disjunction,
        condition,
        print,
        power,
        sqrt,
        sequence,
        notequal
    }
}
