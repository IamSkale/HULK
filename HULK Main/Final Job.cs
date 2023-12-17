namespace HULK
{
    public class FinalJob
    {
        public void FinalWork()
        {
            Dictionary<string, Function> functionDeclarations = new Dictionary<string, Function>();

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
                        List<Error> errors = new List<Error>();

                        Lexer lexer = new Lexer(functionDeclarations, errors);

                        lexer.FillTokensList(input);

                        if (errors.Count() == 0)
                        {
                            Parser parser = new Parser(lexer.tokens, errors);
                            if ((lexer.tokens[0].TokenValue).ToString() == "function")
                            {
                                lexer.FunctionDeclaration(lexer.tokens);

                                WritingSyntaxErrors(errors);
                            }
                            else
                            {
                                AST tree = parser.AndOrOr(new Dictionary<object, AST>());

                                if (errors.Count() == 0)
                                {
                                    Evaluator evaluator = new Evaluator(tree, functionDeclarations, errors);

                                    object result = evaluator.Interpreter();

                                    if (errors.Count() == 0)
                                    {
                                        System.Console.WriteLine(Convert.ToString(result));
                                    }
                                    else
                                    {
                                        WritingSemanticErrors(errors);
                                    }
                                }
                                else
                                {
                                    WritingSyntaxErrors(errors);
                                }
                            }
                        }
                        else
                        {
                            WritingLexicErrors(errors);
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
        private void WritingLexicErrors(List<Error> lexicalErrors)
        {
            foreach (var item in lexicalErrors)
            {
                if (item.context == "unexpected token")
                {
                    System.Console.WriteLine("SYNTAX ERROR: Unexpected Token : { " + item.errorExpression + " }");
                }
                else if (item.context == "expected token")
                {
                    System.Console.WriteLine("SYNTAX ERROR: Expected Token : { " + item.errorExpression + " }");
                }
                else if (item.context == "invalid token")
                {
                    System.Console.WriteLine("LEXICAL ERROR: Invalid Token : { " + item.errorExpression + " }");
                }
                else if (item.errorExpression == "function declaration")
                {
                    switch (item.context)
                    {
                        case "name expected":
                            System.Console.WriteLine("SYNTAX ERROR: Function Identifier Expected at Function Implementation");
                            break;
                        case "declaration symbol expected":
                            System.Console.WriteLine("SYNTAX ERROR: Declaration Symbol Expected at Function Implementation");
                            break;
                        case "body expected":
                            System.Console.WriteLine("SYNTAX ERROR: Function Body Expected at Function Implementation");
                            break;
                    }
                }
            }
        }

        private void WritingSyntaxErrors(List<Error> syntaxErrors)
        {
            foreach (var item in syntaxErrors)
            {
                if (item.context == "expected statement")
                {
                    System.Console.WriteLine("SYNTAX ERROR: Expected " + item.errorExpression);
                }
                else if (item.context == "var declaration")
                {
                    System.Console.WriteLine("SYNTAX ERROR: Expected Variable Declaration in this Context");
                }
                else if (item.context == "expected token")
                {
                    System.Console.WriteLine("SYNTAX ERROR: Expected Token in this Context : { " + item.errorExpression + " }");
                }
                else if (item.errorExpression == "var" && item.context == "identifier expected")
                {
                    System.Console.WriteLine("SYNTAX ERROR: Var Identifier Expected at Var Implementation");
                }
            }
        }

        private void WritingSemanticErrors(List<Error> semanticErrors)
        {
            foreach (var item in semanticErrors)
            {
                if (item.context == "invalid expression")
                {
                    System.Console.WriteLine("SEMANTIC ERROR: Invalid Expression : { " + item.errorExpression + " }");
                }
                else if (item.context == "not implemented")
                {
                    System.Console.WriteLine("SYNTAX ERROR: Not Declarated in this Context: { " + item.errorExpression + " }");
                }
                else if (item.context == "parameters count")
                {
                    System.Console.WriteLine("SEMANTIC ERROR: You can't pass that amount of parameters when Calling the Function: { " + item.errorExpression + " }");
                }
            }
        }
    }
}