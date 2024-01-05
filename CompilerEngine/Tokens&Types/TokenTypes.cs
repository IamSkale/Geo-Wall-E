namespace Geo_Wall_E;

enum TokenTypes
{
    //Operators
    EQUAL, SUM, REST, PROD, DIV, POWER, PERCENT, NO, AND, OR, LOWER, LOWER_EQUAL, GREATER, GREATER_EQUAL, EQUAL_EQUAL, NO_EQUAL,

    //KeyCommands
    COMMA, SEMICOLON, IMPORT,

    //Brackets
    OPEN_BRACKET, CLOSED_BRACKET,

    //Secuences
    OPEN_KEY, CLOSED_KEY, THREE_DOTS,

    //If-else commands
    IF, ELSE, THEN,

    //Let-in commands
    LET, IN,

    //Identifier
    ID,

    //Evaluator
    EVALUATE,

    //Literals
    NUMBER,

    //String
    STRING,

    //Lines
    LINE,

    //Segments
    SEGMENT,

    //Circles
    CIRCLE,

    //Extra unit
    ARC, RAY,

    //Point
    POINT,

    //Measure
    MEASURE,

    //Draw
    DRAW,

    //Random
    RANDOMS,

    //Count
    COUNT,

    //Samples
    SAMPLES,

    //Intersect
    INTERSECT,

    //Drawing colors
    COLOR, COLOR_RED, COLOR_BLUE, COLOR_YELLOW, COLOR_WHITE, COLOR_BLACK,
    COLOR_GREEN, COLOR_GRAY, COLOR_PURPLE, COLOR_CYAN,

    //Restore
    RESTORE,

    //Print
    PRINT,

    //End of file
    END,

}