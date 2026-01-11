using BOOSE;
using BOOSEapp1;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Custom integer variable class that replaces BOOSE's Int class.
    /// Removes restrictions on variable names and variable count limits.
    /// </summary>
    public class AppInteg : Evaluation, ICommand
    {
        /// <summary>
        /// Creates a new integer variable command.
        /// </summary>
        public AppInteg() : base()
        {
        }

        /// <summary>
        /// Reads the variable declaration text and saves the name and start value.
        /// Supports: "int x" or "int x = 50".
        /// </summary>
        public override void Compile()
        {
            /// <summary>
            /// The full text after the command (name and optional "= value").
            /// </summary>
            string fullExpression = this.ParameterList?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(fullExpression))
            {
                throw new CanvasException("Variable declaration requires a name");
            }

            /// <summary>
            /// If there is "=", split into the variable name and the expression.
            /// Otherwise default the value to 0.
            /// </summary>
            if (fullExpression.Contains("="))
            {
                // Declaration with initialization: int radius = 50
                string[] parts = fullExpression.Split('=');
                this.varName = parts[0].Trim();
                this.expression = parts[1].Trim();
            }
            else
            {
                // Declaration only: int width
                this.varName = fullExpression;
                this.expression = "0";
            }

            /// <summary>
            /// Registers this variable in the program's variable storage.
            /// </summary>
            // Add variable to program's variable table (your replacement storage)
            this.Program.AddVariable(this);
        }

        /// <summary>
        /// Evaluates the expression and stores the final integer value.
        /// </summary>
        public override void Execute()
        {
            // IMPORTANT:
            // This must call YOUR evaluation (AppEvaluation), not BOOSE Evaluation.
            base.Execute();

            /// <summary>
            /// Converts the evaluated result to an int and updates the program value.
            /// </summary>
            // Keep behaviour identical: parse evaluatedExpression as int only
            if (int.TryParse(this.evaluatedExpression, out int result))
            {
                this.value = result;
                this.Program.UpdateVariable(this.varName, result);
            }
            else
            {
                throw new CanvasException(
                    $"Cannot convert '{this.evaluatedExpression}' to integer for variable '{this.varName}'"
                );
            }
        }
    }
}
