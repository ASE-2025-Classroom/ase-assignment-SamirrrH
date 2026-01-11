using BOOSE;
using System;
using System.Globalization;

namespace BOOSEapp1
{
    /// <summary>
    /// Custom WHILE loop command that runs a block while a condition is true.
    /// </summary>
    public class AppWhile : CompoundCommand, ICommand
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppWhile() : base() { }

        /// <summary>
        /// Checks parameters (not used here).
        /// </summary>
        /// <param name="parameter">List of parameters passed to the command.</param>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Stores the condition and pushes this WHILE so END can match it later.
        /// </summary>
        public override void Compile()
        {
            /// <summary>
            /// Saves the loop condition text (e.g. "x < 10").
            /// </summary>
            this.expression = (this.ParameterList ?? string.Empty).Trim();

            /// <summary>
            /// Stores where this WHILE is in the program.
            /// </summary>
            this.LineNumber = this.Program.Count;

            /// <summary>
            /// Pushes this WHILE onto the stack for matching with END.
            /// </summary>
            this.Program.Push(this);
        }

        /// <summary>
        /// Evaluates the condition and decides if the loop should run.
        /// </summary>
        public override void Execute()
        {
            /// <summary>
            /// Checks whether the WHILE condition is true or false.
            /// </summary>
            bool conditionResult = EvalCondition(this.Program, this.expression);
            this.Condition = conditionResult;

            /// <summary>
            /// If the condition is false, jump to after the loop.
            /// </summary>
            if (!conditionResult)
                this.Program.PC = this.EndLineNumber;
        }

        /// <summary>
        /// Evaluates a condition like "a >= b" or "x != 0".
        /// </summary>
        /// <param name="program">The stored program.</param>
        /// <param name="expr">The condition text.</param>
        /// <returns>True if the condition is met.</returns>
        private static bool EvalCondition(StoredProgram program, string expr)
        {
            expr = (expr ?? string.Empty).Trim();

            /// <summary>
            /// Supported comparison operators.
            /// </summary>
            string[] ops = { ">=", "<=", "==", "!=", ">", "<" };

            foreach (string op in ops)
            {
                int idx = expr.IndexOf(op, StringComparison.Ordinal);
                if (idx < 0) continue;

                /// <summary>
                /// Splits the condition into left and right sides.
                /// </summary>
                string leftExpr = expr.Substring(0, idx).Trim();
                string rightExpr = expr.Substring(idx + op.Length).Trim();

                /// <summary>
                /// Evaluates both sides as numbers.
                /// </summary>
                double left = EvalNumber(program, leftExpr);
                double right = EvalNumber(program, rightExpr);

                /// <summary>
                /// Compares the two values using the operator.
                /// </summary>
                return op switch
                {
                    ">" => left > right,
                    "<" => left < right,
                    ">=" => left >= right,
                    "<=" => left <= right,
                    "==" => Math.Abs(left - right) < 0.0001,
                    "!=" => Math.Abs(left - right) >= 0.0001,
                    _ => false
                };
            }

            /// <summary>
            /// If no operator is found, treat non-zero as true.
            /// </summary>
            return Math.Abs(EvalNumber(program, expr)) >= 0.0001;
        }

        /// <summary>
        /// Evaluates a single value or expression into a number.
        /// </summary>
        /// <param name="program">The stored program.</param>
        /// <param name="expr">The text to evaluate.</param>
        /// <returns>The numeric result.</returns>
        private static double EvalNumber(StoredProgram program, string expr)
        {
            expr = (expr ?? string.Empty).Trim();

            /// <summary>
            /// If it is a number, parse it directly.
            /// </summary>
            if (double.TryParse(expr, NumberStyles.Any, CultureInfo.InvariantCulture, out double literal))
                return literal;

            /// <summary>
            /// If it is a variable, read its value.
            /// </summary>
            if (program.VariableExists(expr))
                return ToDouble(program.GetVarValue(expr));

            /// <summary>
            /// Otherwise, evaluate it as an expression.
            /// </summary>
            return ToDouble(program.EvaluateExpression(expr));
        }

        /// <summary>
        /// Converts a string to a double safely.
        /// </summary>
        /// <param name="s">Text to convert.</param>
        /// <returns>The double value.</returns>
        private static double ToDouble(string s)
        {
            s = (s ?? "").Trim();
            if (!double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
                throw new CanvasException($"Could not convert '{s}' to a number.");
            return d;
        }
    }
}
