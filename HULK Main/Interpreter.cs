namespace HULK
{
    public abstract class NodeVisitor
    {
        protected object Visit(AST node)
        {
            if (node is TernaryExpression)
            {
                return VisitTernaryExpression((TernaryExpression)node);
            }
            if (node is BinaryExpression)
            {
                return VisitBinaryExpression((BinaryExpression)node);
            }
            if (node is UnaryExpression)
            {
                return (VisitUnaryExpression((UnaryExpression)node));
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
            if (node is Var)
            {
                return VisitVar((Var)node);
            }
            if (node is FunctionCall)
            {
                return VisitFunctionCall((FunctionCall)node);
            }
            if (node is Declaration)
            {
                return VisitDeclaration((Declaration)node);
            }
            return "null";
        }
        public abstract object VisitNum(Number node);
        public abstract object VisitCharchain(CharChain node);
        public abstract object VisitBoolean(Boolean node);
        public abstract object VisitVar(Var node);
        public abstract object VisitFunctionCall(FunctionCall node);
        public abstract object VisitDeclaration(Declaration node);
        public abstract object VisitUnaryExpression(UnaryExpression node);
        public abstract object VisitBinaryExpression(BinaryExpression node);
        public abstract object VisitTernaryExpression(TernaryExpression node);
    }

    public class Evaluator : NodeVisitor
    {
        public static Dictionary<object, object> varsDictionary;
        public AST tree;
        public List<Error> errors;
        public Dictionary<string, Function> functionDeclarations;

        public Evaluator(AST tree, Dictionary<string, Function> functionDeclarations, List<Error> errors)
        {
            this.errors = errors;
            this.tree = tree;
            this.functionDeclarations = functionDeclarations;
            varsDictionary = new Dictionary<object, object>();
        }

        public object Interpreter()
        {
            return Visit(tree);
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
        public override object VisitVar(Var node)
        {
            if (varsDictionary.ContainsKey(node.varName.ToString()) && varsDictionary[node.varName.ToString()] != null)
            {
                return varsDictionary[node.varName.ToString()];
            }
            else
            {
                errors.Add(new Error(node.varName.ToString(),"not implemented"));
                return null;
            }
        }
        public override object VisitFunctionCall(FunctionCall node)
        {
            var auxVarsDictionary = varsDictionary;
            var auxVarsDictionaryII = new Dictionary<object, object>();
            if (functionDeclarations.ContainsKey(node.functionName))
            {
                if (node.parameters.Count != functionDeclarations[node.functionName].functionParameters.Count)
                {
                    errors.Add(new Error(node.functionName.ToString(), "parameters count"));
                    return null;
                }
                else
                {
                    for (int i = 0; i < node.parameters.Count; i++)
                    {
                        auxVarsDictionaryII.Add(functionDeclarations[node.functionName].functionParameters[i], Visit(node.parameters[i]));
                    }
                    varsDictionary = auxVarsDictionaryII;
                    object result = Visit(functionDeclarations[node.functionName].functionBody);
                    varsDictionary = auxVarsDictionary;
                    return result;
                }
            }
            else
            {
                errors.Add(new Error(node.functionName.ToString(), "not implemented"));
                return null;
            }
        }
        public override object VisitDeclaration(Declaration node)
        {
            var auxVarsDictionary = varsDictionary;
            foreach (var item in node.varsDictionary)
            {
                if (varsDictionary.ContainsKey(item.Key))
                {
                    varsDictionary[item.Key] = Visit(item.Value);
                }
                else
                {
                    varsDictionary.Add(item.Key, Visit(item.Value));
                }
            }
            object result = Visit(node.declarationBody);
            varsDictionary = auxVarsDictionary;
            return result;
        }
        public override object VisitUnaryExpression(UnaryExpression node)
        {
            object result = 0;

            object thisNode = Visit(node.node);

            if (node.symbol == "!")
            {
                if (thisNode.ToString() == "true" || thisNode.ToString() == "false" || thisNode is bool)
                {
                    result = !(Convert.ToBoolean(thisNode));
                }
                else
                {
                    errors.Add(new Error(node.symbol + Convert.ToString(thisNode), "invalid expression"));
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
                        errors.Add(new Error(node.symbol + " " + Convert.ToString(thisNode), "invalid expression"));
                    }
                }
            }
            else if (node.symbol == "print")
            {
                result = thisNode;
            }

            return result;
        }
        public override object VisitBinaryExpression(BinaryExpression node)
        {
            object result = 0;

            object leftNode = Visit(node.leftNode);

            object rightNode = Visit(node.rightNode);

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
                            errors.Add(new Error(Convert.ToString(leftNode) + " " + Convert.ToString(node.symbol) + " " + Convert.ToString(rightNode), "invalid expression"));
                        }
                    }
                    else
                    {
                        errors.Add(new Error(Convert.ToString(leftNode) + " " + Convert.ToString(node.symbol) + " " + Convert.ToString(rightNode), "invalid expression"));
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
                        errors.Add(new Error(Convert.ToString(leftNode) + " " + Convert.ToString(node.symbol) + " " + Convert.ToString(rightNode), "invalid expression"));
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
                        errors.Add(new Error(Convert.ToString(leftNode) + " " + Convert.ToString(node.symbol) + " " + Convert.ToString(rightNode), "invalid expression"));
                    }
                }
            }
            else
            {
                if (rightNode == "null")
                {
                    errors.Add(new Error(Convert.ToString(leftNode) + " " + Convert.ToString(node.symbol), "invalid expression"));
                }
                else if (leftNode == "null")
                {
                    errors.Add(new Error(Convert.ToString(node.symbol) + " " + Convert.ToString(rightNode), "invalid expression"));
                }
            }
            return result;
        }
        public override object VisitTernaryExpression(TernaryExpression node)
        {
            object leftNode = Visit(node.leftNode);

            if (leftNode.ToString() == "true" || leftNode.ToString() == "false" || leftNode is bool || leftNode is Boolean)
            {
                if (Convert.ToBoolean(leftNode) == true)
                {
                    return Visit(node.midNode);
                }
                else
                {
                    return Visit(node.rightNode);
                }
            }
            else
            {
                errors.Add(new Error("if " + Convert.ToString(leftNode), "invalid expression"));
                return null;
            }
        }
    }
}