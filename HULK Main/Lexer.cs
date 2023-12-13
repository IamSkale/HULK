namespace HULK
{
    public class Lexer
    {
        public List<Token> tokens;

        List<string> vars = new List<string>();

        public List<string> lexicalErrors;

        public Lexer(string input)
        {
            tokens = new List<Token>();

            lexicalErrors = new List<string>();

            vars = new List<string>();

            FillTokensList(input, tokens, lexicalErrors, vars);
        }

        public static void FillTokensList(string input, List<Token> tokens, List<string> lexicalErrors, List<string> vars)
        {
            int parenthesisCount = 0;

            int ifCount = 0;

            int thenCount = 0;

            int elseCount = 0;

            int letCount = 0;

            int inCount = 0;

            int commaCount = 0;

            int equalCount = 0;

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
                            Tokenize(tempWord, tokens, lexicalErrors, vars);

                            tempWord = "";
                        }
                        tempWord += currentChar;

                        for (int j = i + 1; j < input.Length; j++)
                        {
                            char auxCurrentChar = input[j];

                            tempWord += auxCurrentChar;

                            if (auxCurrentChar == '\"' || j == input.Length - 1)
                            {
                                Tokenize(tempWord, tokens, lexicalErrors, vars);

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
                            Tokenize(tempWord, tokens, lexicalErrors, vars);

                            tempWord = "";
                        }

                        if (input[i + 1] == '=' || (input[i + 1] == '>' && currentChar == '='))
                        {
                            tempWord += currentChar;

                            tempWord += input[i + 1];

                            Tokenize(tempWord, tokens, lexicalErrors, vars);

                            tempWord = "";

                            i++;
                        }
                        else
                        {
                            if (currentChar == '=')
                            {
                                equalCount++;
                            }

                            tempWord += currentChar;

                            Tokenize(tempWord, tokens, lexicalErrors, vars);

                            tempWord = "";
                        }
                    }
                    else if (Char.IsLetterOrDigit(currentChar))
                    {
                        if (tempWord != "")
                        {
                            Tokenize(tempWord, tokens, lexicalErrors, vars);

                            tempWord = "";
                        }

                        for (int j = i; j < input.Length; j++)
                        {
                            char auxCurrentChar = input[j];
                            if (j == input.Length - 1 && Char.IsLetterOrDigit(auxCurrentChar))
                            {
                                tempWord += auxCurrentChar;

                                Tokenize(tempWord, tokens, lexicalErrors, vars);

                                tempWord = "";

                                i = j;
                            }
                            else if (!Char.IsLetterOrDigit(auxCurrentChar))
                            {
                                if (tempWord == "let" || tempWord == "in" || tempWord == "if" || tempWord == "then" || tempWord == "else")
                                {
                                    switch (tempWord)
                                    {
                                        case "let":
                                            letCount++;
                                            break;
                                        case "in":
                                            inCount++;
                                            if (inCount > letCount)
                                            {
                                                lexicalErrors.Add("in");
                                            }
                                            break;
                                        case "if":
                                            ifCount++;
                                            break;
                                        case "then":
                                            thenCount++;
                                            if (thenCount > ifCount)
                                            {
                                                lexicalErrors.Add("then");
                                            }
                                            break;
                                        case "else":
                                            elseCount++;
                                            if (elseCount > ifCount)
                                            {
                                                lexicalErrors.Add("else");
                                            }
                                            break;
                                    }
                                }
                                Tokenize(tempWord, tokens, lexicalErrors, vars);

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
                            Tokenize(tempWord, tokens, lexicalErrors, vars);

                            tempWord = "";
                        }

                        tempWord += currentChar;

                        Tokenize(tempWord, tokens, lexicalErrors, vars);

                        tempWord = "";

                        if (currentChar == '(')
                        {
                            parenthesisCount++;
                        }
                        else if (currentChar == ')')
                        {
                            parenthesisCount -= 1;

                            if (parenthesisCount < 0)
                            {
                                lexicalErrors.Add(")");
                            }
                        }
                        else if (currentChar == ',')
                        {
                            commaCount++;
                        }
                    }
                }
                if (parenthesisCount > 0)
                {
                    for (int i = 0; i < parenthesisCount; i++)
                    {
                        lexicalErrors.Add("(");
                    }
                }
                if (ifCount > thenCount)
                {
                    for (int i = 0; i < ifCount - thenCount; i++)
                    {
                        lexicalErrors.Add("if");
                    }
                }
                if (letCount > inCount)
                {
                    for (int i = 0; i < letCount - inCount; i++)
                    {
                        lexicalErrors.Add("let");
                    }
                }
                if (equalCount == commaCount && commaCount != 0)
                {
                    lexicalErrors.Add(",");
                }
                if (commaCount > equalCount && equalCount > 0)
                {
                    for (int i = 0; i < commaCount - equalCount; i++)
                    {
                        lexicalErrors.Add(",");
                    }
                }
            }

            if (tokens[tokens.Count - 1].TokenType != TokenType.SEMICOLON)
            {
                lexicalErrors.Add(";");
            }
        }
        public static void Tokenize(string token, List<Token> tokens, List<string> lexicalErrors, List<string> vars)
        {
            if (token[0] == '\"')
            {
                if (token[token.Length - 1] == '\"')
                {
                    tokens.Add(new Token(token.Substring(1, token.Length - 2), TokenType.STRING));
                }
                else
                {
                    lexicalErrors.Add(token);
                }
            }
            else if (Char.IsDigit(token[0]))
            {
                bool isInvalid = false;
                for (int i = 0; i < token.Length; i++)
                {
                    if (!Char.IsDigit(token[i]) && token[i] != '.')
                    {
                        isInvalid = true;
                        lexicalErrors.Add(token);
                        break;
                    }
                }
                if (isInvalid == false)
                {
                    tokens.Add(new Token(token, TokenType.NUMBER));
                }
            }
            else if (token == "let" || token == "in" || token == "if" || token == "then" || token == "else" || token == "true" || token == "false" || token == "+" || token == "-" || token == "/" || token == "*" || token == "^" || token == "%" || token == "@" || token == "=" || token == "<" || token == ">" || token == "&" || token == "|" || token == "!" || token == "<=" || token == ">=" || token == "==" || token == "!=" || token == "(" || token == ")" || token == "," || token == ";" || token == "=>")
            {
                switch (token.ToLower())
                {
                    case "let":
                        tokens.Add(new Token(token, TokenType.LET));
                        break;
                    case "in":
                        tokens.Add(new Token(token, TokenType.IN));
                        break;
                    case "if":
                        tokens.Add(new Token(token, TokenType.IF));
                        break;
                    case "then":
                        tokens.Add(new Token(token, TokenType.THEN));
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
                    tokens.Add(new Token(token, TokenType.FUNCTION));
                }
                else
                {
                    if (tokens.Count != 0 && (tokens[tokens.Count - 1].TokenType == TokenType.LET || tokens[tokens.Count - 1].TokenType == TokenType.COMMA))
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
                lexicalErrors.Add(token);
            }
        }
    }
}