#nullable disable

using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Custom For command that removes loop restrictions.
    /// Syntax: for variable = from to to step step
    /// </summary>
    public class AppFor : ConditionalCommand, ICommand
    {
        /// <summary>
        /// The name of the loop variable (e.g. i).
        /// </summary>
        private string loopVariableName;

        /// <summary>
        /// The raw text for the start value (from), end value (to), and step.
        /// These can be numbers or expressions.
        /// </summary>
        private string fromExpr;
        private string toExpr;
        private string stepExpr;

        /// <summary>
        /// The evaluated numeric values used during execution.
        /// </summary>
        private int fromValue;
        private int toValue;
        private int stepValue;

        /// <summary>
        /// Tracks if the loop variable has been set to the start value yet.
        /// </summary>
        private bool initialized;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppFor() : base()
        {
            initialized = false;
        }

        /// <summary>
        /// Checks parameters (not used here).
        /// </summary>
        /// <param name="parameter">List of parameters passed to the command.</param>
        public override void CheckParameters(string[] parameter)
        {
            // No validation
        }

        /// <summary>
        /// Compiles the for statement.
        /// Parses: for variable = from to to [step step]
        /// </summary>
        public override void Compile()
        {
            /// <summary>
            /// Stores the line number of this FOR in the program.
            /// </summary>
            this.LineNumber = this.Program.Count;

            /// <summary>
            /// Splits the text by spaces and '=' to read the parts.
            /// </summary>
            string[] parts = this.ParameterList.Split(new[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4)
                throw new CanvasException("For loop requires: variable = from to to [step step]");

            /// <summary>
            /// Reads the variable name and the "from" expression.
            /// </summary>
            loopVariableName = parts[0].Trim();
            fromExpr = parts[1].Trim();

            /// <summary>
            /// Finds the position of the "to" keyword.
            /// </summary>
            // Find "to"
            int toIndex = -1;
            for (int i = 2; i < parts.Length; i++)
            {
                if (parts[i].Equals("to", StringComparison.OrdinalIgnoreCase))
                {
                    toIndex = i;
                    break;
                }
            }

            if (toIndex == -1 || toIndex + 1 >= parts.Length)
                throw new CanvasException("For loop requires 'to' keyword");

            /// <summary>
            /// Reads the "to" expression.
            /// </summary>
            toExpr = parts[toIndex + 1].Trim();

            /// <summary>
            /// Looks for an optional "step" value, otherwise uses 1.
            /// </summary>
            // Optional "step"
            int stepIndex = -1;
            for (int i = toIndex + 2; i < parts.Length; i++)
            {
                if (parts[i].Equals("step", StringComparison.OrdinalIgnoreCase))
                {
                    stepIndex = i;
                    break;
                }
            }

            stepExpr = (stepIndex != -1 && stepIndex + 1 < parts.Length)
                ? parts[stepIndex + 1].Trim()
                : "1";

            /// <summary>
            /// Pushes this FOR so END can match it later.
            /// </summary>
            this.Program.Push(this);
        }

        /// <summary>
        /// Runs the for loop:
        /// - creates the loop variable if missing
        /// - evaluates from/to/step
        /// - sets Condition to control whether the loop continues
        /// </summary>
        public override void Execute()
        {
            /// <summary>
            /// Creates the loop variable if it does not exist yet.
            /// </summary>
            // Create loop variable if it doesn't exist
            if (!this.Program.VariableExists(loopVariableName))
            {
                AppInteg loopVar = new AppInteg();
                loopVar.Set(this.Program, loopVariableName + " = 0");
                loopVar.Compile();
                loopVar.Execute();
            }

            /// <summary>
            /// Calculates the numeric values for from/to/step.
            /// </summary>
            // Evaluate from, to, step safely (handle literals without EvaluateExpression)
            fromValue = EvalInt(fromExpr, "from");
            toValue = EvalInt(toExpr, "to");
            stepValue = EvalInt(stepExpr, "step");

            /// <summary>
            /// On the first run only, set the loop variable to the start value.
            /// </summary>
            // Init at first run
            if (!initialized)
            {
                this.Program.UpdateVariable(loopVariableName, fromValue);
                initialized = true;
            }

            /// <summary>
            /// Reads the current loop variable value.
            /// </summary>
            // Current loop value
            int currentValue = int.Parse(this.Program.GetVarValue(loopVariableName));

            /// <summary>
            /// Decides if the loop should keep running (supports +step and -step).
            /// </summary>
            // Continue condition
            bool shouldContinue;
            if (stepValue > 0)
                shouldContinue = currentValue <= toValue;
            else if (stepValue < 0)
                shouldContinue = currentValue >= toValue;
            else
                throw new CanvasException("For loop step cannot be zero");

            /// <summary>
            /// Stores the result so END can use it for loop-back.
            /// </summary>
            this.Condition = shouldContinue;

            /// <summary>
            /// If the loop is finished, reset and jump to after END.
            /// </summary>
            if (!shouldContinue)
            {
                initialized = false;          // reset for next time
                this.Program.PC = this.EndLineNumber;
            }
        }

        /// <summary>
        /// Evaluates a string into an integer (number or expression).
        /// </summary>
        /// <param name="expr">Text to evaluate.</param>
        /// <param name="label">Used in error messages.</param>
        /// <returns>The integer result.</returns>
        private int EvalInt(string expr, string label)
        {
            expr = (expr ?? "").Trim();

            if (int.TryParse(expr, out int direct))
                return direct;

            string evaluated = this.Program.EvaluateExpression(expr);
            if (!int.TryParse(evaluated, out int result))
                throw new CanvasException($"For loop '{label}' value must be an integer, got: {evaluated}");

            return result;
        }

        /// <summary>
        /// The evaluated "from" value.
        /// </summary>
        public int From => fromValue;

        /// <summary>
        /// The evaluated "to" value.
        /// </summary>
        public int To => toValue;

        /// <summary>
        /// The evaluated step value.
        /// </summary>
        public int Step => stepValue;

        /// <summary>
        /// The loop variable name (e.g. i).
        /// </summary>
        public string LoopVariableName => loopVariableName;
    }
}
