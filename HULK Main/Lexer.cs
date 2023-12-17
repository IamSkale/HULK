namespace HULK
{
    public class Lexer
    {
        public List<Token> tokens;

        public List<string> vars = new List<string>();

        public List<Error> errors;

        public Dictionary<string, Function> functionDeclarations;

        public Lexer(Dictionary<string, Function> functionDeclarations, List<Error> errors)
        {
            tokens = new List<Token>();

            this.errors = errors;

            vars = new List<string>();

            this.functionDeclarations = functionDeclarations;
        }
        public void FunctionDeclaration(List<Token> tokens)
        {
            object functionName = "";

            List<string> functionParameters = new List<string>();

            List<Token> functionBody = new List<Token>();

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
                            functionParameters.Add(tokens[i].TokenValue.ToString());
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
                                errors.Add(new Error(",", "unexpected token"));
                                commaExceed = true;
                            }
                        }
                        else if (i == tokens.Count - 1 || tokens[i].TokenType == TokenType.SEMICOLON)
                        {
                            errors.Add(new Error(")", "expected token"));
                            auxIndex = i;
                        }
                    }
                    if (auxIndex == tokens.Count - 1)
                    {
                        return;
                    }

                    if (!commaExceed && commaCount == functionParameters.Count)
                    {
                        errors.Add(new Error(",", "unexpected token"));
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

                                functionBody.Add(tokens[i]);
                            }
                            if (functionBody.Count != 0)
                            {
                                ParserII parserII = new ParserII(functionBody, errors);
                                if (functionDeclarations.ContainsKey(functionName.ToString()))
                                {
                                    functionDeclarations[functionName.ToString()] = new Function(functionParameters, parserII.AndOrOr(new Dictionary<object, AST>()));
                                }
                                else
                                {
                                    functionDeclarations.Add(functionName.ToString(), new Function(functionParameters, parserII.AndOrOr(new Dictionary<object, AST>())));
                                }
                            }
                            else
                            {
                                errors.Add(new Error("function declaration", "body expected"));
                            }
                        }
                        else
                        {
                            errors.Add(new Error("function declaration", "declaration symbol expected"));
                        }
                    }
                }
                else
                {
                    errors.Add(new Error("(", "expected token"));
                }
            }
            else
            {
                errors.Add(new Error("function declaration", "name expected"));
            }
        }
        public void FillTokensList(string input)
        {
            if (input != "")
            {
                string tempWord = "";
                for (int i = 0; i < input.Length; i++)
                {
                    char currentChar = input[i];

                    if (currentChar == '\"')
                    {
                        if (tempWord != "")
                        {
                            if (tempWord == ";" && i != input.Length - 1)
                            {
                                errors.Add(new Error(";", "unexpected token"));
                            }
                            Tokenize(tempWord, tokens, vars);

                            tempWord = "";
                        }
                        tempWord += currentChar;

                        for (int j = i + 1; j < input.Length; j++)
                        {
                            char auxCurrentChar = input[j];

                            tempWord += auxCurrentChar;

                            if (auxCurrentChar == '\"' || j == input.Length - 1)
                            {
                                Tokenize(tempWord, tokens, vars);

                                tempWord = "";

                                i = j;

                                break;
                            }
                        }
                    }
                    else if ((currentChar == '<' || currentChar == '>' || currentChar == '=' || currentChar == '!'))
                    {
                        if (tempWord != "")
                        {
                            if (tempWord == ";" && i != input.Length - 1)
                            {
                                errors.Add(new Error(";", "unexpected token"));
                            }
                            Tokenize(tempWord, tokens, vars);

                            tempWord = "";
                        }

                        if (input[i + 1] == '=' || (input[i + 1] == '>' && currentChar == '='))
                        {
                            tempWord += currentChar;

                            tempWord += input[i + 1];

                            Tokenize(tempWord, tokens, vars);

                            tempWord = "";

                            i++;
                        }
                        else
                        {
                            tempWord += currentChar;

                            Tokenize(tempWord, tokens, vars);

                            tempWord = "";
                        }
                    }
                    else if (Char.IsLetterOrDigit(currentChar))
                    {
                        if (tempWord != "")
                        {
                            if (tempWord == ";" && i != input.Length - 1)
                            {
                                errors.Add(new Error(";", "unexpected token"));
                            }
                            Tokenize(tempWord, tokens, vars);

                            tempWord = "";
                        }

                        for (int j = i; j < input.Length; j++)
                        {
                            char auxCurrentChar = input[j];
                            if (j == input.Length - 1 && Char.IsLetterOrDigit(auxCurrentChar))
                            {
                                tempWord += auxCurrentChar;

                                Tokenize(tempWord, tokens, vars);

                                tempWord = "";

                                i = j;
                            }
                            else if (!Char.IsLetterOrDigit(auxCurrentChar))
                            {
                                if (tempWord == ";" && i != input.Length - 1)
                                {
                                    errors.Add(new Error(";", "unexpected token"));
                                }
                                Tokenize(tempWord, tokens, vars);

                                tempWord = "";

                                i = j - 1;

                                break;
                            }
                            else
                            {
                                tempWord += auxCurrentChar;
                            }
                        }
                    }
                    else if (currentChar != ' ')
                    {
                        if (tempWord != "")
                        {
                            Tokenize(tempWord, tokens, vars);

                            tempWord = "";
                        }

                        tempWord += currentChar;

                        Tokenize(tempWord, tokens, vars);

                        tempWord = "";
                    }
                }
            }

            if (tokens[tokens.Count - 1].TokenType != TokenType.SEMICOLON)
            {
                errors.Add(new Error(";", "expected token"));
            }
        }
        private void Tokenize(string token, List<Token> tokens, List<string> vars)
        {
            if (token[0] == '\"')
            {
                if (token[token.Length - 1] == '\"')
                {
                    tokens.Add(new Token(token.Substring(1, token.Length - 2), TokenType.STRING));
                }
                else
                {
                    errors.Add(new Error(token, "invalid token"));
                }
            }
            else if (Char.IsDigit(token[0]))
            {
                bool isInvalid = false;
                for (int i = 1; i < token.Length; i++)
                {
                    if (!Char.IsDigit(token[i]) && token[i] != '.')
                    {
                        isInvalid = true;

                        errors.Add(new Error(token, "invalid token"));

                        break;
                    }
                }
                if (isInvalid == false)
                {
                    tokens.Add(new Token(token, TokenType.NUMBER));
                }
            }
            else if (token == "PI" || token == "let" || token == "in" || token == "if" || token == "else" || token == "true" || token == "false" || token == "+" || token == "-" || token == "/" || token == "*" || token == "^" || token == "%" || token == "@" || token == "=" || token == "<" || token == ">" || token == "&" || token == "|" || token == "!" || token == "<=" || token == ">=" || token == "==" || token == "!=" || token == "(" || token == ")" || token == "," || token == ";" || token == "=>")
            {
                switch (token.ToLower())
                {
                    case "pi":
                        tokens.Add(new Token(token, TokenType.PI));
                        break;
                    case "let":
                        tokens.Add(new Token(token, TokenType.LET));
                        break;
                    case "in":
                        tokens.Add(new Token(token, TokenType.IN));
                        break;
                    case "if":
                        tokens.Add(new Token(token, TokenType.IF));
                        break;
                    case "else":
                        tokens.Add(new Token(token, TokenType.ELSE));
                        break;
                    case "true":
                        tokens.Add(new Token(token, TokenType.TRUE));
                        break;
                    case "false":
                        tokens.Add(new Token(token, TokenType.FALSE));
                        break;
                    case ">":
                        tokens.Add(new Token(token, TokenType.MORE));
                        break;
                    case "<":
                        tokens.Add(new Token(token, TokenType.LESS));
                        break;
                    case ">=":
                        tokens.Add(new Token(token, TokenType.E_MORE));
                        break;
                    case "<=":
                        tokens.Add(new Token(token, TokenType.E_LESS));
                        break;
                    case "==":
                        tokens.Add(new Token(token, TokenType.SAME));
                        break;
                    case "!=":
                        tokens.Add(new Token(token, TokenType.DIFFERENT));
                        break;
                    case "!":
                        tokens.Add(new Token(token, TokenType.NOT));
                        break;
                    case "(":
                        tokens.Add(new Token(token, TokenType.O_PARENTHESES));
                        break;
                    case ")":
                        tokens.Add(new Token(token, TokenType.C_PARENTHESES));
                        break;
                    case ",":
                        tokens.Add(new Token(token, TokenType.COMMA));
                        break;
                    case ";":
                        tokens.Add(new Token(token, TokenType.SEMICOLON));
                        break;
                    case "|":
                        tokens.Add(new Token(token, TokenType.OR));
                        break;
                    case "&":
                        tokens.Add(new Token(token, TokenType.AND));
                        break;
                    case "=":
                        tokens.Add(new Token(token, TokenType.ASIGNATION));
                        break;
                    case "+":
                        tokens.Add(new Token(token, TokenType.PLUS));
                        break;
                    case "-":
                        tokens.Add(new Token(token, TokenType.MINUS));
                        break;
                    case "/":
                        tokens.Add(new Token(token, TokenType.DIVIDE));
                        break;
                    case "*":
                        tokens.Add(new Token(token, TokenType.MULTIPLY));
                        break;
                    case "^":
                        tokens.Add(new Token(token, TokenType.POW));
                        break;
                    case "@":
                        tokens.Add(new Token(token, TokenType.CONCAT));
                        break;
                    case "%":
                        tokens.Add(new Token(token, TokenType.MOD));
                        break;
                    case "=>":
                        tokens.Add(new Token(token, TokenType.DECLARATION));
                        break;
                }
            }
            else if (Char.IsLetter(token[0]))
            {
                if (token == "print" || token == "sin" || token == "cos" || token == "log" || token == "sqrt")
                {
                    tokens.Add(new Token(token, TokenType.INNERFUNCTION));
                }
                else
                {
                    if ((tokens.Count != 0 && !functionDeclarations.ContainsKey(token) && (tokens[tokens.Count - 1].TokenType == TokenType.LET || tokens[tokens.Count - 1].TokenType == TokenType.COMMA || tokens[tokens.Count - 1].TokenType == TokenType.O_PARENTHESES)))
                    {
                        tokens.Add(new Token(token, TokenType.VAR));
                        vars.Add(token);
                    }
                    else
                    {
                        if (vars.Contains(token))
                        {
                            tokens.Add(new Token(token, TokenType.VAR));
                        }
                        else
                        {
                            tokens.Add(new Token(token, TokenType.FUNCTION));
                        }
                    }
                }
            }
            else
            {
                errors.Add(new Error(token, "invalid token"));
            }
        }
    }
}