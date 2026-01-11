using BOOSE;
using System;
using System.Globalization;

namespace BOOSEapp1
{
    /// <summary>
    /// Handles an IF statement and decides whether the block should run.
    /// </summary>
    public class AppIf : CompoundCommand, ICommand
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppIf() : base() { }

        /// <summary>
        /// Checks parameters (not used here).
        /// </summary>
        /// <param name="parameter">List of parameters passed to the command.</param>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Stores the condition text and pushes this IF so ELSE/END can match it later.
        /// </summary>
        public override void Compile()
        {
            /// <summary>
            /// Saves the condition expression (e.g. "x < 10").
            /// </summary>
            this.expression = (this.ParameterList ?? string.Empty).Trim();

            /// <summary>
            /// Saves where this IF is in the program.
            /// </summary>
            this.LineNumber = this.Program.Count;

            /// <summary>
            /// Pushes IF onto the stack for matching with ELSE/END.
            /// </summary>
            this.Program.Push(this);
        }

        /// <summary>
        /// Evaluates the condition and skips the block if it is false.
        /// </summary>
        public override void Execute()
        {
            /// <summary>
            /// Checks whether the IF condition is true or false.
            /// </summary>
            bool conditionResult = EvalCondition(this.Program, this.expression);
            this.Condition = conditionResult;

            /// <summary>
            /// If false, jump to after the IF/ELSE block.
            /// </summary>
            if (!conditionResult)
                this.Program.PC = this.EndLineNumber;
        }

        /// <summary>
        /// Evaluates a condition string like "a >= b" or "x != 0".
        /// If there is no operator, non-zero means true.
        /// </summary>
        /// <param name="program">The stored program (for variables and expressions).</param>
        /// <param name="expr">The condition text.</param>
        /// <returns>True if the condition is met, otherwise false.</returns>
        private static bool EvalCondition(StoredProgram program, string expr)
        {
            expr = (expr ?? string.Empty).Trim();

            /// <summary>
            /// Supported comparison operators (checked in this order).
            /// </summary>
            // comparisons (order matters)
            string[] ops = { ">=", "<=", "==", "!=", ">", "<" };

            foreach (string op in ops)
            {
                int idx = expr.IndexOf(op, StringComparison.Ordinal);
                if (idx < 0) continue;

                /// <summary>
                /// Splits into left side and right side around the operator.
                /// </summary>
                string leftExpr = expr.Substring(0, idx).Trim();
                string rightExpr = expr.Substring(idx + op.Length).Trim();

                /// <summary>
                /// Evaluates both sides as numbers.
                /// </summary>
                double left = EvalNumber(program, leftExpr);
                double right = EvalNumber(program, rightExpr);

                /// <summary>
                /// Compares the two numbers based on the operator.
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
            /// If there is no operator, treat any non-zero value as true.
            /// </summary>
            // Single expression: treat non-zero as true
            return Math.Abs(EvalNumber(program, expr)) >= 0.0001;
        }

        /// <summary>
        /// Evaluates a single expression into a number.
        /// Supports: literal numbers, variables, or full expressions.
        /// </summary>
        /// <param name="program">The stored program.</param>
        /// <param name="expr">The text to evaluate.</param>
        /// <returns>The numeric result.</returns>
        private static double EvalNumber(StoredProgram program, string expr)
        {
            expr = (expr ?? string.Empty).Trim();

            /// <summary>
            /// If it is already a number, just parse it.
            /// </summary>
            // 1) literal number -> parse directly (THIS fixes your <50> and <10> crash)
            if (double.TryParse(expr, NumberStyles.Any, CultureInfo.InvariantCulture, out double literal))
                return literal;

            /// <summary>
            /// If it matches a variable name, read its stored value.
            /// </summary>
            // 2) variable -> read stored value
            if (program.VariableExists(expr))
                return ToDouble(program.GetVarValue(expr));

            /// <summary>
            /// Otherwise, let BOOSE evaluate the expression, then convert the result.
            /// </summary>
            // 3) otherwise -> let BOOSE evaluate, then parse result
            return ToDouble(program.EvaluateExpression(expr));
        }

        /// <summary>
        /// Converts a string into a double using invariant culture.
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
