namespace HULK
{
    public class Parser
    {
        public List<Token> tokens;

        public Token currentToken;

        public int tokenPosition;

        public List<Error> errors;

        public Parser(List<Token> tokens, List<Error> errors)
        {
            this.tokens = tokens;

            tokenPosition = 0;

            currentToken = this.tokens[tokenPosition];

            this.errors = errors;
        }

        public void GetNextToken()
        {
            tokenPosition++;
            if (tokenPosition < tokens.Count)
            {
                currentToken = tokens[tokenPosition];
            }
        }

        public AST AndOrOr(Dictionary<object, AST> varsDictionary)
        {
            AST node = Comparators(varsDictionary);
            while (currentToken.TokenType == TokenType.AND || currentToken.TokenType == TokenType.OR)
            {
                Token auxCurrentToken = currentToken;

                GetNextToken();

                node = new BinaryExpression(node, auxCurrentToken.TokenValue, Comparators(varsDictionary));
            }
            return node;
        }
        private AST Comparators(Dictionary<object, AST> varsDictionary)
        {
            AST node = PlusOrMinusOrConcat(varsDictionary);
            while (currentToken.TokenType == TokenType.MORE || currentToken.TokenType == TokenType.LESS || currentToken.TokenType == TokenType.E_LESS || currentToken.TokenType == TokenType.E_MORE || currentToken.TokenType == TokenType.SAME || currentToken.TokenType == TokenType.DIFFERENT)
            {
                Token auxCurrentToken = currentToken;

                GetNextToken();

                node = new BinaryExpression(node, auxCurrentToken.TokenValue, PlusOrMinusOrConcat(varsDictionary));
            }
            return node;
        }
        private AST PlusOrMinusOrConcat(Dictionary<object, AST> varsDictionary)
        {
            AST node = MultiplyOrDivideOrMod(varsDictionary);
            while (currentToken.TokenType == TokenType.PLUS || currentToken.TokenType == TokenType.MINUS || currentToken.TokenType == TokenType.CONCAT)
            {
                Token auxCurrentToken = currentToken;

                GetNextToken();

                node = new BinaryExpression(node, auxCurrentToken.TokenValue, MultiplyOrDivideOrMod(varsDictionary));

            }
            return node;
        }
        private AST MultiplyOrDivideOrMod(Dictionary<object, AST> varsDictionary)
        {
            AST node = Pow(varsDictionary);
            while (currentToken.TokenType == TokenType.MULTIPLY || currentToken.TokenType == TokenType.DIVIDE || currentToken.TokenType == TokenType.MOD)
            {
                Token auxCurrentToken = currentToken;

                GetNextToken();

                node = new BinaryExpression(node, auxCurrentToken.TokenValue, Pow(varsDictionary));
            }
            return node;
        }
        private AST Pow(Dictionary<object, AST> varsDictionary)
        {
            AST node = Element(varsDictionary);
            while (currentToken.TokenType == TokenType.POW)
            {
                Token auxCurrentToken = currentToken;

                GetNextToken();

                node = new BinaryExpression(node, auxCurrentToken.TokenValue, Element(varsDictionary));
            }
            return node;
        }
        private AST Element(Dictionary<object, AST> varsDictionary)
        {
            AST node = new Null();
            if (currentToken.TokenType == TokenType.NUMBER)
            {
                Token auxCurrentToken = currentToken;
                GetNextToken();
                node = new Number(auxCurrentToken.TokenValue);
            }
            else if (currentToken.TokenType == TokenType.PI)
            {
                Token auxCurrentToken = currentToken;
                GetNextToken();
                node = new Number(Convert.ToSingle(Math.PI));
            }
            else if (currentToken.TokenType == TokenType.STRING)
            {
                Token auxCurrentToken = currentToken;
                GetNextToken();
                node = new CharChain(auxCurrentToken.TokenValue);
            }
            else if (currentToken.TokenType == TokenType.TRUE || currentToken.TokenType == TokenType.FALSE)
            {
                Token auxCurrentToken = currentToken;
                GetNextToken();
                node = new Boolean(auxCurrentToken.TokenValue);
            }
            else if (currentToken.TokenType == TokenType.O_PARENTHESES)
            {
                GetNextToken();
                node = AndOrOr(varsDictionary);
                if (currentToken.TokenType != TokenType.C_PARENTHESES)
                {
                    errors.Add(new Error(")", "expected token"));
                }
                GetNextToken();
            }
            else if (currentToken.TokenType == TokenType.VAR || varsDictionary.ContainsKey(currentToken.TokenValue.ToString()) || (currentToken.TokenType == TokenType.FUNCTION && tokens[tokenPosition+1].TokenType != TokenType.O_PARENTHESES))
            {
                node = new Var(currentToken.TokenValue);
                GetNextToken();
            }
            else if (currentToken.TokenType == TokenType.FUNCTION)
            {
                List<AST> functionCallParams = new List<AST>();
                Token auxCurrentToken = currentToken;
                GetNextToken();
                if (currentToken.TokenType == TokenType.O_PARENTHESES)
                {
                    GetNextToken();
                    functionCallParams.Add(AndOrOr(varsDictionary));

                    while (currentToken.TokenType == TokenType.COMMA)
                    {
                        GetNextToken();
                        functionCallParams.Add(AndOrOr(varsDictionary));
                    }

                    if (currentToken.TokenType != TokenType.C_PARENTHESES)
                    {
                        errors.Add(new Error(")", "expected token"));
                    }
                    node = new FunctionCall(auxCurrentToken.TokenValue.ToString(), functionCallParams);
                    GetNextToken();
                }
            }
            else if (currentToken.TokenType == TokenType.NOT || currentToken.TokenType == TokenType.INNERFUNCTION)
            {
                Token auxCurrentToken = currentToken;

                GetNextToken();

                if (currentToken.TokenType != TokenType.O_PARENTHESES)
                {
                    errors.Add(new Error("(", "expected token"));
                }

                node = new UnaryExpression(auxCurrentToken.TokenValue, AndOrOr(varsDictionary));
            }
            else if (currentToken.TokenType == TokenType.IF)
            {
                AST leftNode = new AST();

                AST midNode = new AST();

                AST rightNode = new AST();

                GetNextToken();

                if (currentToken.TokenType != TokenType.O_PARENTHESES)
                {
                    errors.Add(new Error("(", "expected token"));
                }

                leftNode = AndOrOr(varsDictionary);

                midNode = AndOrOr(varsDictionary);

                if (currentToken.TokenType == TokenType.ELSE)
                {
                    GetNextToken();
                    rightNode = AndOrOr(varsDictionary);
                }
                else
                {
                    rightNode = new CharChain("null");
                    GetNextToken();
                }

                node = new TernaryExpression(leftNode, midNode, rightNode);
            }
            else if (currentToken.TokenType == TokenType.LET)
            {
                Dictionary<object, AST> auxVarsDictionary = new Dictionary<object, AST>(varsDictionary);
                GetNextToken();
                while (true)
                {
                    if (currentToken.TokenType == TokenType.VAR)
                    {
                        Token auxCurrentToken = currentToken;
                        GetNextToken();
                        if (currentToken.TokenType == TokenType.ASIGNATION)
                        {
                            GetNextToken();
                            if (auxVarsDictionary.ContainsKey(auxCurrentToken.TokenValue.ToString()))
                            {
                                auxVarsDictionary[auxCurrentToken.TokenValue.ToString()] = AndOrOr(auxVarsDictionary);
                            }
                            else
                            {
                                var variable = (auxCurrentToken.TokenValue.ToString(), AndOrOr(auxVarsDictionary));
                                if (!(variable.Item2 is Null))
                                {
                                    auxVarsDictionary.Add(variable.Item1, variable.Item2);
                                }
                            }

                            if (currentToken.TokenType != TokenType.COMMA)
                            {
                                break;
                            }
                            GetNextToken();
                        }
                        else
                        {
                            errors.Add(new Error("=", "expected token"));
                        }
                    }
                    else
                    {
                        errors.Add(new Error("var", "identifier expected"));
                    }
                }

                if (currentToken.TokenType != TokenType.IN)
                {
                    errors.Add(new Error("in", "expected token"));
                }
                GetNextToken();

                node = new Declaration(AndOrOr(auxVarsDictionary), auxVarsDictionary);
            }
            return node;
        }
    }
}