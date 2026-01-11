#nullable disable

using BOOSE;
using System;
using System.Globalization;

namespace BOOSEapp1
{
    /// <summary>
    /// Custom real variable class (MyReal-style).
    /// </summary>
    public class AppReal : Evaluation, ICommand
    {
        /// <summary>
        /// Stores the real (double) value for this variable.
        /// </summary>
        private double doubleValue;

        /// <summary>
        /// Gets or sets the current real value.
        /// </summary>
        public new double Value
        {
            get => doubleValue;
            set => doubleValue = value;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppReal() : base() { }

        /// <summary>
        /// Checks parameters (not used here).
        /// </summary>
        /// <param name="parameter">List of parameters passed to the command.</param>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Reads the variable name and optional start expression.
        /// Supports: "real x" or "real x = 2.5".
        /// </summary>
        public override void Compile()
        {
            /// <summary>
            /// The full text after the command (name and optional "= value").
            /// </summary>
            string fullExpression = (this.ParameterList ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(fullExpression))
                throw new CanvasException("Variable declaration requires a name");

            /// <summary>
            /// Splits into variable name and expression, or defaults to 0.0.
            /// </summary>
            if (fullExpression.Contains("="))
            {
                string[] parts = fullExpression.Split('=');
                this.varName = parts[0].Trim();
                this.expression = parts[1].Trim();
            }
            else
            {
                this.varName = fullExpression.Trim();
                this.expression = "0.0";
            }

            /// <summary>
            /// Registers this variable in the program's variable storage.
            /// </summary>
            this.Program.AddVariable(this);
        }

        /// <summary>
        /// Evaluates the expression and stores the result into Value.
        /// </summary>
        public override void Execute()
        {
            /// <summary>
            /// Starts with the saved expression text.
            /// </summary>
            string evaluated = (this.expression ?? "0.0").Trim();

            /// <summary>
            /// Splits the expression to find variable names inside it.
            /// </summary>
            string[] tokens = evaluated.Split(new[] { ' ', '+', '-', '*', '/', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries);

            /// <summary>
            /// Replaces variable tokens with their current numeric values.
            /// </summary>
            foreach (string token in tokens)
            {
                if (this.Program.VariableExists(token))
                {
                    Evaluation v = this.Program.GetVariable(token);
                    string valueStr;

                    if (v is AppReal ar)
                        valueStr = ar.Value.ToString(CultureInfo.InvariantCulture);
                    else if (v is Real br)
                        valueStr = br.Value.ToString(CultureInfo.InvariantCulture);
                    else
                        valueStr = this.Program.GetVarValue(token);

                    evaluated = evaluated.Replace(token, valueStr);
                }
            }

            /// <summary>
            /// If the result is already a number, store it directly.
            /// </summary>
            if (double.TryParse(evaluated, NumberStyles.Any, CultureInfo.InvariantCulture, out double direct))
            {
                this.Value = direct;
                return;
            }

            /// <summary>
            /// Otherwise, try to calculate the expression using DataTable.Compute.
            /// </summary>
            try
            {
                var dt = new System.Data.DataTable();
                var result = dt.Compute(evaluated, "");
                this.Value = Convert.ToDouble(result, CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new CanvasException($"Cannot convert '{evaluated}' to real for variable '{this.varName}'");
            }
        }
    }
}
