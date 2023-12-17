namespace HULK
{

    public class Token
    {
        public object TokenValue { get; set; }

        public TokenType TokenType { get; set; }

        public Token(object tokenValue, TokenType tokenType)
        {
            TokenValue = tokenValue;

            TokenType = tokenType;
        }
    }

    public enum TokenType
    {
        #region Identifier
        VAR,
        FUNCTION,
        INNERFUNCTION,
        #endregion

        #region Puntuator
        O_PARENTHESES,
        C_PARENTHESES,
        COMMA,
        SEMICOLON,
        #endregion

        #region KeyWord
        IF,
        ELSE,
        TRUE,
        FALSE,
        LET,
        IN,
        PI,
        #endregion

        #region Operator
        PLUS,
        MINUS,
        MOD,
        MULTIPLY,
        DIVIDE,
        OR,
        AND,
        MORE,
        LESS,
        E_MORE,
        E_LESS,
        ASIGNATION,
        DIFFERENT,
        NOT,
        CONCAT,
        POW,
        SAME,
        DECLARATION,
        #endregion

        #region VarType
        STRING,
        NUMBER,
        #endregion
    }
}