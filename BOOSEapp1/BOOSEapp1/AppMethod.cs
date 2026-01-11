#nullable disable

using BOOSE;
using System;
using System.Collections.Generic;

namespace BOOSEapp1
{
    /// <summary>
    /// Custom Method command for defining reusable procedures.
    /// Syntax:
    ///   method returnType methodName paramType paramName, paramType paramName, ...
    /// Example:
    ///   method int add int a, int b
    /// </summary>
    public class AppMethod : CompoundCommand, ICommand
    {
        /// <summary>
        /// The method name (used for calling and for the return variable name).
        /// </summary>
        private string methodName;

        /// <summary>
        /// The return type of the method (e.g. int or real).
        /// </summary>
        private string returnType;

        /// <summary>
        /// Lists of parameter names and their types, in the same order.
        /// </summary>
        private readonly List<string> parameterNames;
        private readonly List<string> parameterTypes;

        /// <summary>
        /// The program line to return to after the method finishes (-1 means not currently called).
        /// </summary>
        private int returnLineNumber;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppMethod() : base()
        {
            parameterNames = new List<string>();
            parameterTypes = new List<string>();
            returnLineNumber = -1;
        }

        /// <summary>
        /// Gets the method name.
        /// </summary>
        public string MethodName => methodName;

        /// <summary>
        /// Gets the return type text.
        /// </summary>
        public string Type => returnType;

        /// <summary>
        /// Returns parameter names as an array (used like local variables for the method).
        /// </summary>
        public string[] LocalVariables => parameterNames.ToArray();

        /// <summary>
        /// Gets the list of parameter types.
        /// </summary>
        public List<string> ParameterTypes => parameterTypes;

        /// <summary>
        /// Gets the list of parameter names.
        /// </summary>
        public List<string> ParameterNames => parameterNames;

        /// <summary>
        /// Gets or sets the line to jump back to after a call finishes.
        /// </summary>
        public int ReturnLine
        {
            get => returnLineNumber;
            set => returnLineNumber = value;
        }

        /// <summary>
        /// Checks parameters (not used here).
        /// </summary>
        /// <param name="parameter">List of parameters passed to the command.</param>
        public override void CheckParameters(string[] parameter)
        {
            // no validation
        }

        /// <summary>
        /// Reads the method header, parses parameters, and creates the return variable.
        /// </summary>
        public override void Compile()
        {
            /// <summary>
            /// Stores the line number where this METHOD starts.
            /// </summary>
            this.LineNumber = this.Program.Count;

            /// <summary>
            /// Clears old parameter info before parsing again.
            /// </summary>
            parameterNames.Clear();
            parameterTypes.Clear();

            /// <summary>
            /// The full method header text after the command.
            /// </summary>
            string plist = (this.ParameterList ?? string.Empty).Trim();

            /// <summary>
            /// Splits into return type, method name, and the rest (parameter text).
            /// </summary>
            // Split into: returnType, methodName, restOfLine
            // e.g. "int mullMethod int one, int two"
            string[] head = plist.Split(new[] { ' ', '\t' }, 3, StringSplitOptions.RemoveEmptyEntries);
            if (head.Length < 2)
                throw new CanvasException("Method requires: returnType methodName [parameters]");

            returnType = head[0].Trim();
            methodName = head[1].Trim();
            string paramText = (head.Length == 3) ? head[2].Trim() : string.Empty;

            /// <summary>
            /// Reads parameters split by commas, each in the form: "type name".
            /// </summary>
            // Parse parameters by comma: "int one" , "int two"
            if (!string.IsNullOrWhiteSpace(paramText))
            {
                string[] paramChunks = paramText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string chunk in paramChunks)
                {
                    string c = chunk.Trim();
                    if (string.IsNullOrWhiteSpace(c)) continue;

                    string[] pair = c.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (pair.Length < 2)
                        throw new CanvasException($"Bad parameter in method '{methodName}': '{chunk}' (need: type name)");

                    string pType = pair[0].Trim();
                    string pName = pair[1].Trim();

                    parameterTypes.Add(pType);
                    parameterNames.Add(pName);
                }
            }

            /// <summary>
            /// Creates a return variable with the same name as the method (if missing).
            /// </summary>
            // Create return variable (same name as method)
            if (!this.Program.VariableExists(methodName))
            {
                if (returnType.Equals("int", StringComparison.OrdinalIgnoreCase))
                {
                    AppInteg rv = new AppInteg();
                    rv.Set(this.Program, methodName + " = 0");
                    rv.Compile();
                    rv.Execute();
                }
                else if (returnType.Equals("real", StringComparison.OrdinalIgnoreCase))
                {
                    AppReal rv = new AppReal();
                    rv.Set(this.Program, methodName + " = 0.0");
                    rv.Compile();
                    rv.Execute();
                }
                else
                {
                    throw new CanvasException($"Unknown method return type: {returnType}");
                }
            }

            /// <summary>
            /// Pushes METHOD so "end method" can match it later.
            /// </summary>
            // Push so "end method" can match it
            this.Program.Push(this);
        }

        /// <summary>
        /// Skips over the method body during normal run.
        /// The body only runs when a CALL jumps into it.
        /// </summary>
        public override void Execute()
        {
            // When running normally, skip method body
            if (returnLineNumber == -1)
            {
                this.Program.PC = this.EndLineNumber;
            }
        }
    }
}
