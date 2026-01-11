#nullable disable

using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Custom poke command that sets values in arrays.
    /// Syntax:
    ///   poke arrayName row = value
    ///   poke arrayName row col = value
    /// </summary>
    public class AppPoke : AppArray, ICommand
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppPoke() : base() { }

        /// <summary>
        /// Checks parameters (not used here).
        /// </summary>
        /// <param name="parameter">List of parameters passed to the command.</param>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Reads the poke syntax and stores:
        /// array name, row/column text, and value text.
        /// </summary>
        public override void Compile()
        {
            ProcessArrayParametersCompile(POKE);
        }

        /// <summary>
        /// Evaluates indexes/value and writes the value into the array.
        /// </summary>
        public override void Execute()
        {
            /// <summary>
            /// Evaluates row/column and converts the value to int/real based on the array type.
            /// </summary>
            ProcessArrayParametersExecute(POKE);

            /// <summary>
            /// Gets the target array variable from the program.
            /// </summary>
            AppArray array = (AppArray)this.Program.GetVariable(this.varName);

            /// <summary>
            /// Writes an integer value into the array.
            /// </summary>
            if (array.type == "int")
            {
                array.SetIntArray(this.valueInt, this.row, this.column);
                return;
            }

            /// <summary>
            /// Writes a real (double) value into the array.
            /// </summary>
            if (array.type == "real")
            {
                array.SetRealArray(this.valueReal, this.row, this.column);
                return;
            }

            /// <summary>
            /// If the array type is not supported, throw an error.
            /// </summary>
            throw new CanvasException($"Unknown array type '{array.type}'");
        }
    }
}
