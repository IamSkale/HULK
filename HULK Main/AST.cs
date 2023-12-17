namespace HULK
{

    public class AST
    {

    }
    public class TernaryExpression : AST
    {
        public AST leftNode;

        public AST midNode;

        public AST rightNode;

        public TernaryExpression(AST leftNode, AST midNode, AST rightNode)
        {
            this.leftNode = leftNode;

            this.midNode = midNode;

            this.rightNode = rightNode;
        }
    }
    public class BinaryExpression : AST
    {
        public AST leftNode;

        public AST rightNode;

        public string symbol;

        public BinaryExpression(AST leftNode, object symbol, AST rightNode)
        {
            this.leftNode = leftNode;

            this.rightNode = rightNode;

            this.symbol = symbol.ToString()!;
        }
    }
    public class UnaryExpression : AST
    {
        public string symbol;

        public AST node;

        public UnaryExpression(object symbol, AST node)
        {
            this.node = node;

            this.symbol = symbol.ToString();
        }

    }

    public class Number : AST
    {
        public object Value;

        public Number(object Value)
        {
            this.Value = Value;
        }
    }
    public class CharChain : AST
    {
        public object Value;

        public CharChain(object value)
        {
            this.Value = value;
        }
    }
    public class Boolean : AST
    {
        public object Value;

        public Boolean(object value)
        {
            this.Value = value;
        }
    }
    public class FunctionCall : AST
    {
        public string functionName;

        public List<AST> parameters;

        public FunctionCall(string functionName, List<AST> parameters)
        {
            this.functionName = functionName;

            this.parameters = parameters;
        }
    }
    public class Function : AST
    {
        public List<string> functionParameters;

        public AST functionBody;

        public Function(List<string> functionParameters, AST functionBody)
        {
            this.functionParameters = functionParameters;

            this.functionBody = functionBody;
        }
    }
    public class Declaration : AST
    {
        public AST declarationBody;

        public Dictionary<object, AST> varsDictionary;

        public Declaration(AST declarationBody, Dictionary<object, AST> varsDictionary)
        {
            this.declarationBody = declarationBody;

            this.varsDictionary = varsDictionary;
        }
    }
    public class Var : AST
    {
        public object varName;

        public Var(object varName)
        {
            this.varName = varName;
        }
    }
    public class Error : AST
    {
        public string context;
        public string errorExpression;
        public Error(string errorExpression, string context)
        {
            this.context = context;
            this.errorExpression = errorExpression;
        }
    }
    public class Null : AST
    {
        public string value;
        public Null()
        {
            value = "null";
        }
    }
}