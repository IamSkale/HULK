namespace HULK
{
    public class Parser
    {
        public Dictionary<string, string> syntaxErrors;
        public Dictionary<object, (List<object>, List<Token>)> functionDeclarations;
        public AST tree;
        public Lexer lexer;
        public Parser(Lexer lexer, Dictionary<string, string> syntaxErrors, Dictionary<object, (List<object>, List<Token>)> functionDeclarations)
        {
            this.syntaxErrors = syntaxErrors;

            this.lexer = lexer;

            this.functionDeclarations = functionDeclarations;
        }
        public void FunctionDeclaration(List<Token> tokens)
        {
            object functionName = "";

            List<object> functionParameters = new List<object>();

            List<Token> auxTokens = new List<Token>();

            int auxIndex = 0;

            int commaCount = 0;

            bool commaExceed = false;

            if (tokens[1].TokenType == TokenType.FUNCTION)
            {
                functionName = tokens[1].TokenValue;

                if (tokens[2].TokenType == TokenType.O_PARENTHESES)
                {
                    for (int i = 3; i < tokens.Count; i++)
                    {
                        if (tokens[i].TokenType == TokenType.FUNCTION || tokens[i].TokenType == TokenType.VAR)
                        {
                            functionParameters.Add(tokens[i].TokenValue);
                        }
                        else if (tokens[i].TokenType == TokenType.C_PARENTHESES)
                        {
                            auxIndex = i + 1;
                            break;
                        }
                        else if (tokens[i].TokenType == TokenType.COMMA)
                        {
                            commaCount++;
                            if (commaCount > functionParameters.Count)
                            {
                                syntaxErrors.Add(",", "unexpected token");
                                commaExceed = true;
                            }
                        }
                        else if (i == tokens.Count - 1 || tokens[i].TokenType == TokenType.SEMICOLON)
                        {
                            syntaxErrors.Add(")", "expected token");
                            auxIndex = i;
                        }
                    }
                    if (auxIndex == tokens.Count - 1)
                    {
                        return;
                    }

                    if (!commaExceed && commaCount == functionParameters.Count)
                    {
                        syntaxErrors.Add(",", "unexpected token");
                    }
                    else if (!commaExceed)
                    {
                        if (tokens[auxIndex].TokenType == TokenType.DECLARATION)
                        {
                            for (int i = auxIndex + 1; i < tokens.Count; i++)
                            {
                                if (tokens[i].TokenType == TokenType.SEMICOLON || i == tokens.Count - 1)
                                {
                                    break;
                                }

                                auxTokens.Add(tokens[i]);
                            }
                            if (auxTokens.Count != 0)
                            {
                                if (functionDeclarations.ContainsKey(functionName.ToString()))
                                {
                                    functionDeclarations[functionName] = (functionParameters, auxTokens);
                                }
                                else
                                {
                                    functionDeclarations.Add(functionName, (functionParameters, auxTokens));
                                }
                            }
                            else
                            {
                                syntaxErrors.Add("function declaration", "body expected");
                            }
                        }
                        else
                        {
                            syntaxErrors.Add("function declaration", "declaration symbol expected");
                        }
                    }
                }
                else
                {
                    syntaxErrors.Add("(", "expected token");
                }
            }
            else
            {
                syntaxErrors.Add("function declaration", "name expected");
            }
        }
        public AST ConditionalOrDeclaration(List<Token> tokens, int tokenPosition, Token currentToken, Dictionary<object, AST> varsDictionary)
        {
            if (currentToken.TokenType == TokenType.IF)
            {
                var innerTreesList = new List<List<Token>>();

                AST leftNode = new AST();

                AST midNode = new AST();

                AST rightNode = new AST();

                int ifCount = 0;

                int thenCount = 0;

                int elseCount = 0;

                while (true)
                {
                    if (currentToken.TokenType == TokenType.SEMICOLON || tokenPosition == tokens.Count - 1)
                    {
                        break;
                    }
                    if (currentToken.TokenType == TokenType.IF)
                    {
                        var innerTree = new List<Token>();
                        ifCount++;
                        for (int i = tokenPosition + 1; i < tokens.Count(); i++)
                        {
                            if (tokens[i].TokenType == TokenType.IF)
                            {
                                ifCount++;
                            }
                            else if (tokens[i].TokenType == TokenType.THEN)
                            {
                                thenCount++;
                                if (thenCount == ifCount)
                                {
                                    tokenPosition = i;
                                    if (tokenPosition < tokens.Count())
                                        currentToken = tokens[tokenPosition];
                                    break;
                                }
                            }
                            else if (tokens[i].TokenType == TokenType.ELSE)
                            {
                                elseCount++;
                            }
                            innerTree.Add(tokens[i]);
                        }

                        innerTreesList.Add(innerTree);
                    }
                    else if (currentToken.TokenType == TokenType.THEN)
                    {
                        var innerTree = new List<Token>();
                        for (int i = tokenPosition + 1; i < tokens.Count(); i++)
                        {
                            if (tokens[i].TokenType == TokenType.IF)
                            {
                                ifCount++;
                            }
                            if (tokens[i].TokenType == TokenType.THEN)
                            {
                                thenCount++;
                            }
                            if (tokens[i].TokenType == TokenType.ELSE)
                            {
                                elseCount++;
                                if (elseCount == ifCount)
                                {
                                    tokenPosition = i;

                                    if (tokenPosition < tokens.Count())
                                        currentToken = tokens[tokenPosition];
                                    break;
                                }
                            }
                            if (tokens[i].TokenType == TokenType.SEMICOLON || i == tokens.Count - 1)
                            {
                                tokenPosition = i;
                                if (tokenPosition < tokens.Count())
                                    currentToken = tokens[tokenPosition];
                                break;
                            }
                            innerTree.Add(tokens[i]);
                        }

                        innerTreesList.Add(innerTree);
                    }
                    else if (currentToken.TokenType == TokenType.ELSE)
                    {
                        var innerTree = new List<Token>();
                        for (int i = tokenPosition + 1; i < tokens.Count(); i++)
                        {
                            tokenPosition = i;
                            currentToken = tokens[i];

                            if (tokens[i].TokenType == TokenType.SEMICOLON)
                            {
                                break;
                            }

                            innerTree.Add(tokens[i]);
                        }
                        innerTreesList.Add(innerTree);
                    }
                }

                if (innerTreesList[0].Count == 0 || innerTreesList[1].Count == 0)
                {
                    if (innerTreesList[0].Count == 0)
                    {
                        syntaxErrors.Add("if Statement", "expected statement");
                    }
                    else if (innerTreesList[1].Count == 0)
                    {
                        syntaxErrors.Add("then Statement", "expected statement");
                    }
                }
                else
                {
                    leftNode = ConditionalOrDeclaration(innerTreesList[0], 0, innerTreesList[0][0], varsDictionary);

                    midNode = ConditionalOrDeclaration(innerTreesList[1], 0, innerTreesList[1][0], varsDictionary);

                    if (innerTreesList.Count == 3 && innerTreesList[2].Count != 0)
                    {
                        rightNode = ConditionalOrDeclaration(innerTreesList[2], 0, innerTreesList[2][0], varsDictionary);
                    }
                    else
                    {
                        rightNode = new CharChain("null");
                    }
                }

                tree = new TernaryExpression(leftNode, midNode, rightNode);

                return tree;
            }
            else if (currentToken.TokenType == TokenType.LET)
            {
                var auxVarsDictionary = new Dictionary<object, AST>(varsDictionary);

                var innerTree = new List<Token>();

                var varDeclarations = new List<Token>();

                int letCountI = 0;

                int inCount = 0;

                while (true)
                {
                    if (currentToken.TokenType == TokenType.SEMICOLON || tokenPosition == tokens.Count - 1)
                    {
                        break;
                    }
                    if (currentToken.TokenType == TokenType.LET)
                    {
                        letCountI++;

                        for (int i = tokenPosition + 1; i < tokens.Count; i++)
                        {
                            if (tokens[i].TokenType == TokenType.LET)
                            {
                                letCountI++;
                            }
                            else if (tokens[i].TokenType == TokenType.IN)
                            {
                                inCount++;
                                if (inCount == letCountI)
                                {
                                    tokenPosition = i;
                                    if (tokenPosition < tokens.Count())
                                        currentToken = tokens[tokenPosition];

                                    for (int j = tokenPosition + 1; j < tokens.Count; j++)
                                    {
                                        if (j == tokens.Count - 1)
                                        {
                                            if (tokens[j].TokenType != TokenType.SEMICOLON)
                                            {
                                                innerTree.Add(tokens[j]);
                                            }
                                            tokenPosition = j;
                                            if (tokenPosition < tokens.Count())
                                                currentToken = tokens[tokenPosition];
                                            break;
                                        }
                                        innerTree.Add(tokens[j]);
                                    }
                                    break;
                                }
                            }
                            varDeclarations.Add(tokens[i]);
                        }

                        if (varDeclarations[0].TokenType == TokenType.VAR)
                        {
                            for (int i = 0; i < varDeclarations.Count; i++)
                            {
                                int commaCount = 0;

                                int letCountII = 0;

                                int asignationCount = 0;

                                List<Token> varAsignation = new List<Token>();

                                if (varDeclarations[i].TokenType == TokenType.VAR && varDeclarations[i + 1].TokenType == TokenType.ASIGNATION)
                                {
                                    asignationCount++;
                                    for (int j = i + 2; j < varDeclarations.Count; j++)
                                    {
                                        if (varDeclarations[j].TokenType == TokenType.ASIGNATION)
                                        {
                                            asignationCount++;
                                        }
                                        if (varDeclarations[j].TokenType == TokenType.LET)
                                        {
                                            letCountII++;
                                        }
                                        if (varDeclarations[j].TokenType == TokenType.COMMA)
                                        {
                                            commaCount++;
                                            if (commaCount == asignationCount - letCountII)
                                            {
                                                auxVarsDictionary.Add(varDeclarations[i].TokenValue, ConditionalOrDeclaration(varAsignation, 0, varAsignation[0], auxVarsDictionary));
                                                i = j;
                                                break;
                                            }
                                        }
                                        varAsignation.Add(varDeclarations[j]);
                                        if (j == varDeclarations.Count - 1)
                                        {
                                            if (auxVarsDictionary.ContainsKey(varDeclarations[i].TokenValue.ToString()))
                                            {
                                                auxVarsDictionary[varDeclarations[i].TokenValue] = ConditionalOrDeclaration(varAsignation, 0, varAsignation[0], auxVarsDictionary);
                                            }
                                            else
                                            {
                                                auxVarsDictionary.Add(varDeclarations[i].TokenValue, ConditionalOrDeclaration(varAsignation, 0, varAsignation[0], auxVarsDictionary));
                                            }
                                            i = j;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            syntaxErrors.Add(varDeclarations[0].TokenValue.ToString(), "var declaration");
                        }
                    }
                }
                tree = new UnaryExpression("in", ConditionalOrDeclaration(innerTree, 0, innerTree[0], auxVarsDictionary));

                return tree;
            }
            else
            {
                SemiParser semiParser = new SemiParser(tokens, lexer, syntaxErrors, functionDeclarations);

                AST node = semiParser.AndOrOr(varsDictionary);

                return node;
            }
        }
    }

    public class SemiParser
    {
        List<Token> innerTokens;
        Token currentToken;
        public int tokenPosition;
        public Lexer lexer;
        public Dictionary<string, string> syntaxErrors;
        public Dictionary<object, (List<object>, List<Token>)> functionDeclarations;
        public SemiParser(List<Token> tokens, Lexer lexer, Dictionary<string, string> syntaxErrors, Dictionary<object, (List<object>, List<Token>)> functionDeclarations)
        {
            innerTokens = tokens;
            tokenPosition = 0;
            currentToken = innerTokens[tokenPosition];
            this.lexer = lexer;
            this.syntaxErrors = syntaxErrors;
            this.functionDeclarations = functionDeclarations;
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
            AST node = new AST();
            if (currentToken.TokenType == TokenType.NUMBER)
            {
                Token auxCurrentToken = currentToken;
                GetNextToken();
                node = new Number(auxCurrentToken.TokenValue);
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
                List<Token> aux = new List<Token>();
                int parenthesisCount = 1;
                while (tokenPosition != innerTokens.Count - 1)
                {
                    if (currentToken.TokenType == TokenType.C_PARENTHESES)
                    {
                        parenthesisCount--;
                        if (parenthesisCount == 0)
                        {
                            break;
                        }
                    }
                    if (currentToken.TokenType == TokenType.O_PARENTHESES)
                    {
                        parenthesisCount++;
                    }

                    aux.Add(currentToken);
                    GetNextToken();
                }
                Parser parser = new Parser(lexer, syntaxErrors, functionDeclarations);
                node = parser.ConditionalOrDeclaration(aux, 0, aux[0], varsDictionary);
                GetNextToken();
            }
            else if (currentToken.TokenType == TokenType.VAR || varsDictionary.ContainsKey(currentToken.TokenValue))
            {
                if (varsDictionary.ContainsKey(currentToken.TokenValue))
                {
                    Token auxCurrentToken = currentToken;
                    node = varsDictionary[auxCurrentToken.TokenValue];
                    GetNextToken();
                }
                else
                {
                    syntaxErrors.Add(currentToken.TokenValue.ToString(), "not implemented");
                }
            }
            else if (currentToken.TokenType == TokenType.FUNCTION || currentToken.TokenType == TokenType.NOT)
            {
                Parser parser = new Parser(lexer, syntaxErrors, functionDeclarations);

                if (currentToken.TokenType != TokenType.NOT && currentToken.TokenValue.ToString() != "print" && currentToken.TokenValue.ToString() != "log" && currentToken.TokenValue.ToString() != "sin" && currentToken.TokenValue.ToString() != "cos" && currentToken.TokenValue.ToString() != "sqrt" && !varsDictionary.ContainsKey(currentToken.TokenValue.ToString()) && !parser.functionDeclarations.ContainsKey(currentToken.TokenValue.ToString()))
                {
                    syntaxErrors.Add(currentToken.TokenValue.ToString(), "not implemented");
                }
                else if (currentToken.TokenType == TokenType.FUNCTION && parser.functionDeclarations.ContainsKey(currentToken.TokenValue.ToString()))
                {
                    Token auxCurrentToken = currentToken;
                    GetNextToken();
                    GetNextToken();
                    List<Token> functionParameters = new List<Token>();
                    int parenthesisCount = 1;
                    while (tokenPosition != innerTokens.Count - 1)
                    {
                        if (currentToken.TokenType == TokenType.C_PARENTHESES)
                        {
                            parenthesisCount--;
                            if (parenthesisCount == 0)
                            {
                                break;
                            }
                        }
                        if (currentToken.TokenType == TokenType.O_PARENTHESES)
                        {
                            parenthesisCount++;
                        }

                        functionParameters.Add(currentToken);
                        GetNextToken();
                    }
                    node = FunctionCall(auxCurrentToken.TokenValue, varsDictionary, functionParameters, parser);
                    GetNextToken();
                }
                else
                {
                    Token auxCurrentToken = currentToken;

                    GetNextToken();

                    node = new UnaryExpression(auxCurrentToken.TokenValue, AndOrOr(varsDictionary));
                }
            }

            return node;
        }
        AST FunctionCall(object functionName, Dictionary<object, AST> varsDictionary, List<Token> functionParameters, Parser parser)
        {
            List<object> functionDeclarationParameters = functionDeclarations[functionName].Item1;

            List<Token> tokens = functionDeclarations[functionName].Item2;

            List<AST> functionCallParameters = new List<AST>();

            AST node = parser.ConditionalOrDeclaration(functionParameters, 0, functionParameters[0], varsDictionary);

            Dictionary<object, AST> functionParametersDictionary = new Dictionary<object, AST>();

            functionParametersDictionary.Add(functionDeclarationParameters[0], node);

            AST functionCall = parser.ConditionalOrDeclaration(tokens, 0, tokens[0], functionParametersDictionary);

            return functionCall;
        }

        public void GetNextToken()
        {
            tokenPosition++;
            if (tokenPosition < innerTokens.Count())
                currentToken = innerTokens[tokenPosition];
        }
    }
}