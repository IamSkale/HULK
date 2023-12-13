namespace HULK
{

    public class AST { }
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
}