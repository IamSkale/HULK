namespace HULK
{
    public abstract class NodeVisitor
    {
        protected object Visit(AST node, List<string> semanticErrors)
        {
            if (node is TernaryExpression)
            {
                return VisitTernaryExpression((TernaryExpression)node, semanticErrors);
            }
            if (node is BinaryExpression)
            {
                return VisitBinaryExpression((BinaryExpression)node, semanticErrors);
            }
            if (node is UnaryExpression)
            {
                return (VisitUnaryExpression((UnaryExpression)node, semanticErrors));
            }
            if (node is Number)
            {
                return VisitNum((Number)node);
            }
            if (node is CharChain)
            {
                return VisitCharchain((CharChain)node);
            }
            if (node is Boolean)
            {
                return VisitBoolean((Boolean)node);
            }
            return "null";
        }
        public abstract object VisitNum(Number node);
        public abstract object VisitCharchain(CharChain node);
        public abstract object VisitBoolean(Boolean node);
        public abstract object VisitUnaryExpression(UnaryExpression node, List<string> semanticErrors);
        public abstract object VisitBinaryExpression(BinaryExpression node, List<string> semanticErrors);
        public abstract object VisitTernaryExpression(TernaryExpression node, List<string> semanticErrors);
    }

    public class Evaluator : NodeVisitor
    {
        public Parser parser;
        public AST tree;
        public List<string> semanticErrors;
         
        public Evaluator(AST tree)
        {
            semanticErrors = new List<string>();
            this.tree = tree;
        }

        public object Interpreter(List<string> semanticErrors)
        {
            return Visit(tree, semanticErrors);
        }
        public override object VisitNum(Number node)
        {
            return node.Value;
        }
        public override object VisitCharchain(CharChain node)
        {
            return node.Value;
        }
        public override object VisitBoolean(Boolean node)
        {
            return node.Value;
        }
        public override object VisitUnaryExpression(UnaryExpression node, List<string> semanticErrors)
        {
            object result = 0;

            object thisNode = Visit(node.node, semanticErrors);

            if (node.symbol == "!")
            {
                if (thisNode.ToString() == "true" || thisNode.ToString() == "false" || thisNode is bool)
                {
                    result = !(Convert.ToBoolean(thisNode));
                }
                else
                {
                    semanticErrors.Add(node.symbol + Convert.ToString(thisNode));
                }
            }
            else if (node.symbol == "sin" || node.symbol == "cos" || node.symbol == "log" || node.symbol == "sqrt")
            {
                if (!(thisNode.ToString() == "true" || thisNode.ToString() == "false" || thisNode is bool))
                {
                    try
                    {
                        switch (node.symbol)
                        {
                            case "sin":
                                result = Math.Sin(Convert.ToSingle(thisNode));
                                break;
                            case "cos":
                                result = Math.Cos(Convert.ToSingle(thisNode));
                                break;
                            case "log":
                                result = Math.Log(Convert.ToSingle(thisNode));
                                break;
                            case "sqrt":
                                result = Math.Sqrt(Convert.ToSingle(thisNode));
                                break;
                        }
                    }
                    catch (System.Exception)
                    {
                        semanticErrors.Add(node.symbol + " " + Convert.ToString(thisNode));
                    }
                }
            }
            else if (node.symbol == "print" || node.symbol == "in")
            {
                result = thisNode;
            }

            return result;
        }
        public override object VisitBinaryExpression(BinaryExpression node, List<string> semanticErrors)
        {
            object result = 0;

            object leftNode = Visit(node.leftNode, semanticErrors);

            object rightNode = Visit(node.rightNode, semanticErrors);

            if (leftNode != "null" && rightNode != "null")
            {
                if (node.symbol == "+" || node.symbol == "-" || node.symbol == "/" || node.symbol == "*" || node.symbol == "%" || node.symbol == ">" || node.symbol == "<" || node.symbol == ">=" || node.symbol == "<=" || node.symbol == "^")
                {
                    if (!(((leftNode.ToString() == "true" || leftNode.ToString() == "false") && rightNode is bool) || (leftNode is bool && (rightNode.ToString() == "true" || rightNode.ToString() == "false")) || (leftNode is bool && rightNode is bool) || ((leftNode.ToString() == "true" || leftNode.ToString() == "false") && (rightNode.ToString() == "true" || rightNode.ToString() == "false"))))
                    {
                        try
                        {
                            switch (node.symbol)
                            {
                                case "+":
                                    result = Convert.ToSingle(leftNode) + Convert.ToSingle(rightNode);
                                    break;
                                case "-":
                                    result = Convert.ToSingle(leftNode) - Convert.ToSingle(rightNode);
                                    break;
                                case "/":
                                    result = Convert.ToSingle(leftNode) / Convert.ToSingle(rightNode);
                                    break;
                                case "*":
                                    result = Convert.ToSingle(leftNode) * Convert.ToSingle(rightNode);
                                    break;
                                case "%":
                                    result = Convert.ToSingle(leftNode) % Convert.ToSingle(rightNode);
                                    break;
                                case "^":
                                    result = Math.Pow(Convert.ToSingle(leftNode), Convert.ToSingle(rightNode));
                                    break;
                                case ">":
                                    result = Convert.ToSingle(leftNode) > Convert.ToSingle(rightNode);
                                    break;
                                case "<":
                                    result = Convert.ToSingle(leftNode) < Convert.ToSingle(rightNode);
                                    break;
                                case ">=":
                                    result = Convert.ToSingle(leftNode) >= Convert.ToSingle(rightNode);
                                    break;
                                case "<=":
                                    result = Convert.ToSingle(leftNode) <= Convert.ToSingle(rightNode);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            semanticErrors.Add(Convert.ToString(leftNode) + " " + Convert.ToString(node.symbol) + " " + Convert.ToString(rightNode));
                        }
                    }
                    else
                    {
                        semanticErrors.Add(Convert.ToString(leftNode) + " " + Convert.ToString(node.symbol) + " " + Convert.ToString(rightNode));
                    }
                }
                else if (node.symbol == "==" || node.symbol == "!=" || node.symbol == "@")
                {
                    switch (node.symbol)
                    {
                        case "==":
                            result = Convert.ToString(leftNode) == Convert.ToString(rightNode);
                            break;
                        case "!=":
                            result = Convert.ToString(leftNode) != Convert.ToString(rightNode);
                            break;
                        case "@":
                            result = Convert.ToString(leftNode) + Convert.ToString(rightNode);
                            break;
                    }
                }
                else if (node.symbol == "|" || node.symbol == "&")
                {
                    if (((leftNode.ToString() == "true" || leftNode.ToString() == "false") && rightNode is bool) || (leftNode is bool && (rightNode.ToString() == "true" || rightNode.ToString() == "false")) || (leftNode is bool && rightNode is bool) || ((leftNode.ToString() == "true" || leftNode.ToString() == "false") && (rightNode.ToString() == "true" || rightNode.ToString() == "false")))
                    {
                        if (node.symbol == "|")
                        {
                            result = Convert.ToBoolean(leftNode) || Convert.ToBoolean(rightNode);
                        }
                        else if (node.symbol == "&")
                        {
                            result = Convert.ToBoolean(leftNode) && Convert.ToBoolean(rightNode);
                        }
                    }
                    else
                    {
                        semanticErrors.Add(Convert.ToString(leftNode) + " " + Convert.ToString(node.symbol) + " " + Convert.ToString(rightNode));
                    }
                }
            }
            else if (node.symbol == "-" && rightNode != "null")
            {
                if (!(((leftNode.ToString() == "true" || leftNode.ToString() == "false") && rightNode is bool) || (leftNode is bool && (rightNode.ToString() == "true" || rightNode.ToString() == "false")) || (leftNode is bool && rightNode is bool) || ((leftNode.ToString() == "true" || leftNode.ToString() == "false") && (rightNode.ToString() == "true" || rightNode.ToString() == "false"))))
                {
                    try
                    {
                        result = -Convert.ToSingle(rightNode);
                    }
                    catch (Exception ex)
                    {
                        semanticErrors.Add(Convert.ToString(leftNode) + " " + Convert.ToString(node.symbol) + " " + Convert.ToString(rightNode));
                    }
                }
            }
            else
            {
                if (rightNode == "null")
                {
                    semanticErrors.Add(Convert.ToString(leftNode) + " " + Convert.ToString(node.symbol));
                }
                else if (leftNode == "null")
                {
                    semanticErrors.Add(Convert.ToString(node.symbol) + " " + Convert.ToString(rightNode));
                }
            }
            return result;
        }
        public override object VisitTernaryExpression(TernaryExpression node, List<string> semanticErrors)
        {
            object leftNode = Visit(node.leftNode, semanticErrors);

            object midNode = Visit(node.midNode, semanticErrors);

            object rightNode = Visit(node.rightNode, semanticErrors);

            object result = 0;

            if (leftNode.ToString() == "true" || leftNode.ToString() == "false" || leftNode is bool || leftNode is Boolean)
            {
                if (Convert.ToBoolean(leftNode) == true)
                {
                    result = midNode;
                }
                else
                {
                    result = rightNode;
                }
            }
            else
            {
                semanticErrors.Add("if " + Convert.ToString(leftNode));
            }
            return result;
        }
    }
}