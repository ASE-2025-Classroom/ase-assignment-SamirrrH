#nullable disable

using BOOSE;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BOOSEapp1
{
    /// <summary>
    /// Calls a user method and passes values into its parameters.
    /// </summary>
    public class AppCall : CompoundCommand, ICommand
    {
        /// <summary>
        /// Stores the method name that should be called.
        /// </summary>
        private string methodName;

        /// <summary>
        /// Stores the argument text values typed after the method name.
        /// </summary>
        private readonly List<string> arguments = new List<string>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppCall() : base() { }

        /// <summary>
        /// Checks parameters but no checks are needed here.
        /// </summary>
        /// <param name="parameter">Parameters passed to the command.</param>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Reads the line and extracts the method name and its arguments.
        /// It supports both "call myMethod 10" and "myMethod 10".
        /// </summary>
        public override void Compile()
        {
            /// <summary>
            /// Saves the current line number in the program.
            /// </summary>
            this.LineNumber = this.Program.Count;

            /// <summary>
            /// Splits the parameter text by spaces, tabs, and commas.
            /// </summary>
            string[] parts = (this.ParameterList ?? string.Empty)
                .Trim()
                .Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);

            /// <summary>
            /// A call must include at least a method name.
            /// </summary>
            if (parts.Length < 1)
                throw new CanvasException("Call requires: methodName [arguments]");

            /// <summary>
            /// Used to decide where the method name starts in the array.
            /// </summary>
            int idx = 0;

            /// <summary>
            /// If the first word is "call", the real method name is the next word.
            /// </summary>
            if (parts[0].Equals("call", StringComparison.OrdinalIgnoreCase))
                idx = 1;

            /// <summary>
            /// If there is no method name after "call", stop with an error.
            /// </summary>
            if (idx >= parts.Length)
                throw new CanvasException("Call requires: methodName [arguments]");

            /// <summary>
            /// Stores the method name.
            /// </summary>
            methodName = parts[idx].Trim();

            /// <summary>
            /// Clears old arguments and stores the new ones.
            /// </summary>
            arguments.Clear();
            for (int i = idx + 1; i < parts.Length; i++)
                arguments.Add(parts[i].Trim());
        }

        /// <summary>
        /// Finds the method, sets up its parameter variables, then jumps into the method.
        /// </summary>
        public override void Execute()
        {
            /// <summary>
            /// Looks for the method definition inside the stored program.
            /// </summary>
            AppMethod target = FindMethod(methodName);
            if (target == null)
                throw new CanvasException($"Method '{methodName}' not found");

            /// <summary>
            /// Checks the number of arguments matches the method parameter count.
            /// </summary>
            if (arguments.Count != target.LocalVariables.Length)
                throw new CanvasException(
                    $"Method '{methodName}' expects {target.LocalVariables.Length} args, got {arguments.Count}");

            /// <summary>
            /// Loops through each argument and stores it into the method parameter variable.
            /// </summary>
            for (int i = 0; i < arguments.Count; i++)
            {
                /// <summary>
                /// Gets the parameter name and type from the method header.
                /// </summary>
                string paramName = target.LocalVariables[i];
                string paramType = target.ParameterTypes[i];

                /// <summary>
                /// Converts the argument into a final value string (number or evaluated expression).
                /// </summary>
                string valueStr = EvaluateArg(arguments[i]);

                /// <summary>
                /// If the parameter type is int, parse and store an integer value.
                /// </summary>
                if (paramType.Equals("int", StringComparison.OrdinalIgnoreCase))
                {
                    int intVal = ParseIntStrict(valueStr);

                    /// <summary>
                    /// If the parameter variable does not exist yet, create it.
                    /// </summary>
                    if (!this.Program.VariableExists(paramName))
                    {
                        AppInteg p = new AppInteg();
                        p.Set(this.Program, $"{paramName} = {intVal}");
                        p.Compile();
                        p.Execute();
                    }
                    else
                    {
                        /// <summary>
                        /// If it already exists, update it with the new value.
                        /// </summary>
                        var v = this.Program.GetVariable(paramName);

                        /// <summary>
                        /// If it is your custom int class, set its Value directly.
                        /// </summary>
                        if (v is AppInteg ai)
                        {
                            ai.Value = intVal;
                        }
                        else
                        {
                            /// <summary>
                            /// Otherwise update using the program method as a fallback.
                            /// </summary>
                            this.Program.UpdateVariable(paramName, intVal);
                        }
                    }
                }
                /// <summary>
                /// If the parameter type is real, parse and store a double value.
                /// </summary>
                else if (paramType.Equals("real", StringComparison.OrdinalIgnoreCase))
                {
                    double realVal = ParseDoubleStrict(valueStr);

                    /// <summary>
                    /// If the parameter variable does not exist yet, create it.
                    /// </summary>
                    if (!this.Program.VariableExists(paramName))
                    {
                        AppReal p = new AppReal();
                        p.Set(this.Program, $"{paramName} = {realVal.ToString(CultureInfo.InvariantCulture)}");
                        p.Compile();
                        p.Execute();
                    }
                    else
                    {
                        /// <summary>
                        /// If it already exists, update it with the new value.
                        /// </summary>
                        var v = this.Program.GetVariable(paramName);

                        /// <summary>
                        /// If it is your custom real class, set its Value directly.
                        /// </summary>
                        if (v is AppReal ar)
                        {
                            ar.Value = realVal;
                        }
                        else
                        {
                            /// <summary>
                            /// Otherwise update using the program method as a fallback.
                            /// </summary>
                            this.Program.UpdateVariable(paramName, realVal);
                        }
                    }
                }
                /// <summary>
                /// Any other parameter type is not supported.
                /// </summary>
                else
                {
                    throw new CanvasException($"Unsupported parameter type '{paramType}'");
                }
            }

            /// <summary>
            /// Saves where to return to after the method finishes.
            /// </summary>
            target.ReturnLine = this.Program.PC;

            /// <summary>
            /// Jumps to the method header line so execution enters the method body next.
            /// </summary>
            this.Program.PC = target.LineNumber;
        }

        /// <summary>
        /// Turns an argument into a value string.
        /// If it is already a number, it is returned.
        /// Otherwise it is evaluated as an expression.
        /// </summary>
        /// <param name="arg">The raw argument text.</param>
        /// <returns>The final value as a string.</returns>
        private string EvaluateArg(string arg)
        {
            arg = (arg ?? string.Empty).Trim();

            if (int.TryParse(arg, NumberStyles.Integer, CultureInfo.InvariantCulture, out _)) return arg;
            if (double.TryParse(arg, NumberStyles.Any, CultureInfo.InvariantCulture, out _)) return arg;

            return this.Program.EvaluateExpression(arg);
        }

        /// <summary>
        /// Converts a string into an int using invariant culture.
        /// </summary>
        /// <param name="s">Text to convert.</param>
        /// <returns>The parsed int value.</returns>
        private int ParseIntStrict(string s)
        {
            if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v))
                throw new CanvasException($"Cannot parse int argument '{s}'");
            return v;
        }

        /// <summary>
        /// Converts a string into a double using invariant culture.
        /// </summary>
        /// <param name="s">Text to convert.</param>
        /// <returns>The parsed double value.</returns>
        private double ParseDoubleStrict(string s)
        {
            if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double v))
                throw new CanvasException($"Cannot parse real argument '{s}'");
            return v;
        }

        /// <summary>
        /// Searches through the program to find a method with the given name.
        /// </summary>
        /// <param name="name">The method name to look for.</param>
        /// <returns>The method command if found, otherwise null.</returns>
        private AppMethod FindMethod(string name)
        {
            for (int i = 0; i < this.Program.Count; i++)
            {
                if (this.Program[i] is AppMethod m &&
                    m.MethodName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return m;
            }
            return null;
        }
    }
}
