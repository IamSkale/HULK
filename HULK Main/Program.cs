namespace HULK
{
    public class Program
    {
        public static void Main()
        {
            FinalJob finalJob = new FinalJob();

            finalJob.FinalWork();
            /*
            var functions = new Dictionary<object, (List<object>, List<Token>)>();
            List<string> input = new List<string>();
            string a = "function si(a) => a + 5 +8 - 4*8 + 9^2;";
            string b = "si(45-8*9+5);";
            string c = "function no(a) => a + 8*9;";
            string d = "si(14 + 45) - no(45 -5);";
            input.Add(a);
            input.Add(b);
            //input.Add(c);
            input.Add(d);
            foreach (var item in input)
            {
                Lexer lexer = new Lexer(item);
                Parser parser = new Parser(lexer, new Dictionary<string, string>(), functions);
                if (lexer.tokens[0].TokenValue.ToString() == "function")
                {
                    parser.FunctionDeclaration(lexer.tokens);
                }
                else
                {
                    AST tree = parser.ConditionalOrDeclaration(lexer.tokens, 0, lexer.tokens[0], new Dictionary<object, AST>());
                    Evaluator evaluator = new Evaluator(tree);
                    System.Console.WriteLine(Convert.ToString(evaluator.Interpreter(evaluator.semanticErrors)));
                }
            }
            */
        }
    }
}