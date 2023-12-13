namespace HULK
{
    public class FinalJob
    {
        string input;
        private void WritingLexicErrors(List<string> lexicalErrors)
        {
            foreach (var item in lexicalErrors)
            {
                if (item == "(" || item == ")")
                {
                    System.Console.WriteLine("Unexpected Token : { " + item + " }");
                }
                else if (item == "let" || item == "in" || item == "if" || item == "then" || item == "else" || item == "," || item == "=")
                {
                    System.Console.WriteLine("Unexpected in this Context : { " + item + " }");
                }
                else if (item == ";")
                {
                    System.Console.WriteLine("Expected Token : { " + item + " }");
                }
                else
                {
                    System.Console.WriteLine("Invalid Token : " + "{ " + item + " }");
                }
            }
        }

        private void WritingSyntaxErrors(Dictionary<string, string> syntaxErrors)
        {
            foreach (var item in syntaxErrors)
            {
                if (item.Value == "expected statement")
                {
                    System.Console.WriteLine("Expected " + item.Key);
                }
                else if (item.Value == "var declaration")
                {
                    System.Console.WriteLine("Expected Variable Declaration in this Context");
                }
                else if (item.Value == "not implemented")
                {
                    System.Console.WriteLine("Not Declarated in this Context: { " + item.Key + " }");
                }
                else if (item.Value == "not a parameter")
                {
                    System.Console.WriteLine("Not a Parameter for the Context: { " + item.Key + " }");
                }
                else if (item.Key == "function declaration")
                {
                    switch (item.Value)
                    {
                        case "name expected":
                            System.Console.WriteLine("Function Name Expected at Function Implementation");
                            break;
                        case "declaration symbol expected":
                            System.Console.WriteLine("Declaration Symbol Expected at Function Implementation");
                            break;
                        case "body expected":
                            System.Console.WriteLine("Function Body Expected at Function Implementation");
                            break;
                    }
                }
                else if(item.Value == "unexpected token")
                {
                    System.Console.WriteLine("Unexpected Token in this Context : { " + item.Key + " }");
                }
                else if(item.Value == "expected token")
                {
                    System.Console.WriteLine("Expected Token in this Context : { " + item.Key + " }");
                }
            }
        }

        private void WritingSemanticErrors(List<string> semanticErrors)
        {
            foreach (var item in semanticErrors)
            {
                System.Console.WriteLine("Invalid Expression : { " + item + " }");
            }
        }
        public void FinalWork()
        {
            Dictionary<object, (List<object>, List<Token>)> functionDeclarations = new Dictionary<object, (List<object>, List<Token>)>();

            System.Console.WriteLine("If you want to write a Codeline press \"Enter\" and if you want to exit press \"Ctrl + C\": ");

            while (true)
            {
                ConsoleKeyInfo option = Console.ReadKey();

                if (option.Key == ConsoleKey.Enter)
                {
                    System.Console.Write("> ");

                    string input = Console.ReadLine();

                    if (input != "")
                    {
                        Lexer lexer = new Lexer(input);

                        if (lexer.lexicalErrors.Count() == 0)
                        {
                            Parser parser = new Parser(lexer, new Dictionary<string, string>(),functionDeclarations);

                            if ((lexer.tokens[0].TokenValue).ToString() == "function")
                            {
                                parser.FunctionDeclaration(lexer.tokens);

                                WritingSyntaxErrors(parser.syntaxErrors);
                            }
                            else
                            {
                                AST tree = parser.ConditionalOrDeclaration(lexer.tokens, 0, lexer.tokens[0], new Dictionary<object, AST>());

                                if (parser.syntaxErrors.Count == 0)
                                {
                                    Evaluator evaluator = new Evaluator(tree);

                                    object result = evaluator.Interpreter(evaluator.semanticErrors);

                                    if (evaluator.semanticErrors.Count() == 0)
                                    {
                                        System.Console.WriteLine(Convert.ToString(result));
                                    }
                                    else
                                    {
                                        WritingSemanticErrors(evaluator.semanticErrors);
                                    }
                                }
                                else
                                {
                                    WritingSyntaxErrors(parser.syntaxErrors);
                                }
                            }
                        }
                        else
                        {
                            WritingLexicErrors(lexer.lexicalErrors);
                        }
                    }
                }
                else
                {
                    System.Console.WriteLine("");
                    System.Console.WriteLine("Invalid Key, please try again");
                }
            }
        }
    }
}