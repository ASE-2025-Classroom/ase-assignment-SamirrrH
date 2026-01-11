#nullable disable

using BOOSE;
using System;
using System.Globalization;

namespace BOOSEapp1
{
    /// <summary>
    /// Write command that draws text onto the canvas.
    /// It supports normal text in quotes and also values from variables or maths.
    /// </summary>
    public class AppWrite : Evaluation, ICommand
    {
        /// <summary>
        /// A shared canvas reference used by this command to draw the text.
        /// </summary>
        private static AppCanvas staticCanvas;

        /// <summary>
        /// Sets which canvas the write command will draw onto.
        /// </summary>
        /// <param name="canvas">The canvas that will receive the text.</param>
        public static void SetCanvas(AppCanvas canvas)
        {
            staticCanvas = canvas;
        }

        /// <summary>
        /// Default constructor used by the factory.
        /// </summary>
        public AppWrite() : base() { }

        /// <summary>
        /// No parameter validation is needed for this command.
        /// </summary>
        /// <param name="parameter">Parameters passed in (not used).</param>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Stores the text after "write" so it can be evaluated later.
        /// </summary>
        public override void Compile()
        {
            this.expression = (this.ParameterList ?? string.Empty).Trim();
        }

        /// <summary>
        /// Works out the final output string and writes it to the canvas.
        /// </summary>
        public override void Execute()
        {
            /// <summary>
            /// Gets the saved expression and removes extra spaces.
            /// </summary>
            string expr = (this.expression ?? string.Empty).Trim();

            /// <summary>
            /// If the expression contains quotes, treat it as a string expression.
            /// Otherwise treat it as a numeric expression.
            /// </summary>
            string output = expr.Contains("\"")
                ? EvaluateStringExpression(expr)
                : EvaluateNumericExpression(expr);

            /// <summary>
            /// Draws the final output text at the current canvas position.
            /// </summary>
            staticCanvas?.WriteText(output);
        }

        /// <summary>
        /// Evaluates a string expression.
        /// Example: "X = " + x + ", Y = " + y
        /// </summary>
        /// <param name="expr">The text expression to evaluate.</param>
        /// <returns>The combined text result.</returns>
        private string EvaluateStringExpression(string expr)
        {
            /// <summary>
            /// Splits by '+' so each part can be processed and joined.
            /// </summary>
            var parts = expr.Split('+', StringSplitOptions.RemoveEmptyEntries);

            /// <summary>
            /// Builds the final output text piece by piece.
            /// </summary>
            string result = "";

            foreach (var raw in parts)
            {
                /// <summary>
                /// Cleans each part (removes extra spaces).
                /// </summary>
                string part = raw.Trim();

                /// <summary>
                /// If the part is inside quotes, add it as plain text.
                /// </summary>
                if (part.StartsWith("\"") && part.EndsWith("\"") && part.Length >= 2)
                {
                    result += part.Substring(1, part.Length - 2);
                    continue;
                }

                /// <summary>
                /// If the part is a variable name, add the variable value.
                /// </summary>
                if (this.Program.VariableExists(part))
                {
                    Evaluation v = this.Program.GetVariable(part);

                    /// <summary>
                    /// Real variables are formatted carefully to keep decimals.
                    /// </summary>
                    if (v is AppReal appRealVar)
                        result += appRealVar.Value.ToString("0.0################", CultureInfo.InvariantCulture);
                    else if (v is Real booseRealVar)
                        result += booseRealVar.Value.ToString("0.0################", CultureInfo.InvariantCulture);
                    else
                        result += this.Program.GetVarValue(part);

                    continue;
                }

                /// <summary>
                /// If it is not a quoted string or variable, treat it like a number expression.
                /// </summary>
                result += EvaluateNumericExpression(part);
            }

            /// <summary>
            /// Returns the final joined string.
            /// </summary>
            return result;
        }

        /// <summary>
        /// Evaluates numeric output.
        /// It can be a number, a variable name, or a maths expression.
        /// </summary>
        /// <param name="expr">The numeric expression.</param>
        /// <returns>The final numeric result as text.</returns>
        private string EvaluateNumericExpression(string expr)
        {
            /// <summary>
            /// Keeps the original text and also a working version we can replace tokens in.
            /// </summary>
            string originalExpr = (expr ?? string.Empty).Trim();
            string evaluated = originalExpr;

            /// <summary>
            /// Checks if the expression includes + - * /, meaning it is maths.
            /// </summary>
            bool hasMathOps =
                originalExpr.Contains("+") || originalExpr.Contains("-") ||
                originalExpr.Contains("*") || originalExpr.Contains("/");

            /// <summary>
            /// If it is only a single variable name, return it using the correct format.
            /// </summary>
            if (!hasMathOps && this.Program.VariableExists(originalExpr))
            {
                Evaluation vVar = this.Program.GetVariable(originalExpr);

                /// <summary>
                /// Real values should show .0 if they are whole numbers.
                /// </summary>
                if (vVar is AppReal arVar)
                {
                    double dv = arVar.Value;
                    if (Math.Abs(dv % 1) < 0.0000001)
                        return dv.ToString("0.0", CultureInfo.InvariantCulture);
                    return dv.ToString(CultureInfo.InvariantCulture);
                }
                if (vVar is Real brVar)
                {
                    double dv = brVar.Value;
                    if (Math.Abs(dv % 1) < 0.0000001)
                        return dv.ToString("0.0", CultureInfo.InvariantCulture);
                    return dv.ToString(CultureInfo.InvariantCulture);
                }

                /// <summary>
                /// Int variables keep the old style (no .0).
                /// </summary>
                return this.Program.GetVarValue(originalExpr);
            }

            /// <summary>
            /// Splits the expression into tokens so we can find variables inside it.
            /// </summary>
            string[] tokens = evaluated.Split(new[] { ' ', '+', '-', '*', '/', '(', ')' },
                StringSplitOptions.RemoveEmptyEntries);

            /// <summary>
            /// Replaces variable names in the expression with their current values.
            /// </summary>
            foreach (string token in tokens)
            {
                if (!this.Program.VariableExists(token)) continue;

                Evaluation v = this.Program.GetVariable(token);
                string value;

                /// <summary>
                /// Real values are converted using invariant culture for safe maths.
                /// </summary>
                if (v is AppReal appRealVar)
                    value = appRealVar.Value.ToString("0.0################", CultureInfo.InvariantCulture);
                else if (v is Real booseRealVar)
                    value = booseRealVar.Value.ToString("0.0################", CultureInfo.InvariantCulture);
                else
                    value = this.Program.GetVarValue(token);

                evaluated = evaluated.Replace(token, value);
            }

            /// <summary>
            /// If the replaced expression is now just a number, format and return it.
            /// </summary>
            if (double.TryParse(evaluated, NumberStyles.Any, CultureInfo.InvariantCulture, out double direct))
            {
                /// <summary>
                /// Whole numbers may be returned as "155.0" (maths) or "2" (no maths).
                /// </summary>
                if (Math.Abs(direct % 1) < 0.0000001)
                {
                    if (hasMathOps)
                        return direct.ToString("0.0", CultureInfo.InvariantCulture);

                    return ((int)direct).ToString(CultureInfo.InvariantCulture);
                }

                /// <summary>
                /// Decimal numbers are returned normally.
                /// </summary>
                return direct.ToString(CultureInfo.InvariantCulture);
            }

            /// <summary>
            /// If it is still not a simple number, try to compute it using DataTable.
            /// </summary>
            try
            {
                var dt = new System.Data.DataTable();
                var result = dt.Compute(evaluated, "");
                double v = Convert.ToDouble(result, CultureInfo.InvariantCulture);

                /// <summary>
                /// Applies the same whole-number formatting rules after calculation.
                /// </summary>
                if (Math.Abs(v % 1) < 0.0000001)
                {
                    if (hasMathOps)
                        return v.ToString("0.0", CultureInfo.InvariantCulture);

                    return ((int)v).ToString(CultureInfo.InvariantCulture);
                }

                /// <summary>
                /// Returns the calculated decimal value.
                /// </summary>
                return v.ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                /// <summary>
                /// If calculation fails, return the expression text that we ended with.
                /// </summary>
                return evaluated;
            }
        }
    }
}
