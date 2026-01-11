#nullable disable

using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Custom evaluation class for handling standalone variable assignments.
    /// Allows assignments like "width = 2*radius" after variable declaration.
    /// </summary>
    public class AppEvaluation : Evaluation, ICommand
    {
        /// <summary>
        /// Creates a new evaluation command.
        /// </summary>
        public AppEvaluation() : base()
        {
        }

        /// <summary>
        /// Checks parameters (not used here).
        /// </summary>
        /// <param name="parameter">List of parameters passed to the command.</param>
        public override void CheckParameters(string[] parameter)
        {
            // Intentionally no validation
        }

        /// <summary>
        /// Reads and splits the assignment (var = expression).
        /// Also checks the variable already exists.
        /// </summary>
        public override void Compile()
        {
            /// <summary>
            /// The full text after the command, e.g. "width = 2*radius".
            /// </summary>
            string fullExpression = this.ParameterList.Trim();

            if (string.IsNullOrWhiteSpace(fullExpression))
            {
                throw new CanvasException("Assignment requires an expression");
            }

            /// <summary>
            /// Splits the input into left side (variable name) and right side (expression).
            /// </summary>
            if (fullExpression.Contains("="))
            {
                string[] parts = fullExpression.Split('=');
                this.varName = parts[0].Trim();
                this.expression = parts[1].Trim();
            }
            else
            {
                throw new CanvasException("Assignment must contain '='");
            }

            /// <summary>
            /// Makes sure the variable was declared before we try to update it.
            /// </summary>
            if (!this.Program.VariableExists(this.varName))
            {
                throw new CanvasException($"Variable '{this.varName}' must be declared before assignment");
            }
        }

        /// <summary>
        /// Evaluates the right side and updates the variable value.
        /// </summary>
        public override void Execute()
        {
            /// <summary>
            /// Calculates the expression into a final number string.
            /// </summary>
            string evaluated = EvaluateManually(this.expression);

            /// <summary>
            /// Gets the current variable so we can keep its type behaviour.
            /// </summary>
            Evaluation existingVar = this.Program.GetVariable(this.varName);

            // Keep behaviour identical: treat Real/AppReal as real variable targets
            if (existingVar is Real || existingVar is AppReal)
            {
                if (double.TryParse(evaluated, out double doubleResult))
                {
                    this.Program.UpdateVariable(this.varName, doubleResult);
                }
                else
                {
                    throw new CanvasException($"Cannot evaluate '{this.expression}' as real for variable '{this.varName}'");
                }
            }
            else if (int.TryParse(evaluated, out int intResult))
            {
                this.value = intResult;
                this.Program.UpdateVariable(this.varName, intResult);
            }
            else if (double.TryParse(evaluated, out double dblResult))
            {
                this.Program.UpdateVariable(this.varName, dblResult);
            }
            else
            {
                throw new CanvasException($"Cannot evaluate '{this.expression}' for variable '{this.varName}'");
            }
        }

        /// <summary>
        /// Evaluates an expression by replacing variable names with values,
        /// then calculating the final result.
        /// </summary>
        /// <param name="expr">The expression text to evaluate.</param>
        /// <returns>The result as text.</returns>
        private string EvaluateManually(string expr)
        {
            /// <summary>
            /// Starts with the raw expression text.
            /// </summary>
            string result = expr.Trim();

            /// <summary>
            /// Finds possible variable names by splitting on operators and brackets.
            /// </summary>
            string[] tokens = result.Split(new[] { ' ', '+', '-', '*', '/', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries);

            /// <summary>
            /// Replaces any variable tokens with their current values.
            /// </summary>
            foreach (string token in tokens)
            {
                if (this.Program.VariableExists(token))
                {
                    string value = this.Program.GetVarValue(token);
                    result = result.Replace(token, value);
                }
            }

            /// <summary>
            /// Uses DataTable.Compute to calculate the final numeric result.
            /// </summary>
            try
            {
                var dataTable = new System.Data.DataTable();
                var evalResult = dataTable.Compute(result, "");
                return evalResult.ToString();
            }
            catch
            {
                throw new CanvasException($"Cannot evaluate expression: {result}");
            }
        }
    }
}
