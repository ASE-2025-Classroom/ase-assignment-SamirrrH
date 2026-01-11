#nullable disable

using BOOSE;
using System;
using System.Globalization;

namespace BOOSEapp1
{
    /// <summary>
    /// Custom peek command that gets values from arrays.
    /// Syntax:
    ///   peek varName = arrayName row
    ///   peek varName = arrayName row col
    /// </summary>
    public class AppPeek : AppArray, ICommand
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppPeek() : base() { }

        /// <summary>
        /// Checks parameters (not used here).
        /// </summary>
        /// <param name="parameter">List of parameters passed to the command.</param>
        public override void CheckParameters(string[] parameter)
        {
            // no validation
        }

        /// <summary>
        /// Reads the peek syntax and stores:
        /// target variable, array name, row, and column text.
        /// </summary>
        public override void Compile()
        {
            // Parses ParameterList and sets:
            //   peekVar, varName (array name), rowS, columnS
            ProcessArrayParametersCompile(PEEK);

            /// <summary>
            /// Trims stored names/values to avoid space issues.
            /// </summary>
            // Safety: normalise names
            if (peekVar != null) peekVar = peekVar.Trim();
            if (varName != null) varName = varName.Trim();
            if (rowS != null) rowS = rowS.Trim();
            if (columnS != null) columnS = columnS.Trim();
        }

        /// <summary>
        /// Reads a value from the array and puts it into the target variable.
        /// </summary>
        public override void Execute()
        {
            /// <summary>
            /// Evaluates row/column indexes for the array access.
            /// </summary>
            ProcessArrayParametersExecute(PEEK);

            /// <summary>
            /// Gets clean names for the array and the target variable.
            /// </summary>
            string arrayName = (this.varName ?? string.Empty).Trim();
            string targetName = (this.peekVar ?? string.Empty).Trim();

            /// <summary>
            /// Basic checks to make sure the array name is valid and exists.
            /// </summary>
            if (string.IsNullOrWhiteSpace(arrayName))
                throw new CanvasException("Peek error: array name missing.");

            if (!this.Program.VariableExists(arrayName))
                throw new CanvasException($"Peek error: No such variable '{arrayName}'");

            /// <summary>
            /// Gets the variable and confirms it is an array.
            /// </summary>
            Evaluation ev = this.Program.GetVariable(arrayName);

            if (ev is not AppArray array)
                throw new CanvasException($"Peek error: '{arrayName}' is not an array.");

            /// <summary>
            /// If the array is int, read an int and store it into the target.
            /// </summary>
            if (array.type == "int")
            {
                int v = array.GetIntArray(this.row, this.column);

                // int target: UpdateVariable is correct
                this.Program.UpdateVariable(targetName, v);
                return;
            }

            /// <summary>
            /// If the array is real, read a double and store it into the target.
            /// </summary>
            if (array.type == "real")
            {
                double v = array.GetRealArray(this.row, this.column);

                if (!this.Program.VariableExists(targetName))
                    throw new CanvasException($"Peek error: target '{targetName}' does not exist.");

                Evaluation target = this.Program.GetVariable(targetName);

                /// <summary>
                /// If the target is AppReal, set its Value directly.
                /// </summary>
                // If target is your custom AppReal -> set directly
                if (target is AppReal ar)
                {
                    ar.Value = v;
                    return;
                }

                /// <summary>
                /// If the target is BOOSE.Real, set its Value directly.
                /// </summary>
                // If target is BOOSE.Real -> set directly
                if (target is Real br)
                {
                    br.Value = v;
                    return;
                }

                /// <summary>
                /// Otherwise, update using the program update method.
                /// </summary>
                // Fallback: try UpdateVariable with a double
                this.Program.UpdateVariable(targetName, v);
                return;
            }

            /// <summary>
            /// If the type is not supported, throw an error.
            /// </summary>
            throw new CanvasException($"Peek error: Unknown array type '{array.type}'");
        }
    }
}
