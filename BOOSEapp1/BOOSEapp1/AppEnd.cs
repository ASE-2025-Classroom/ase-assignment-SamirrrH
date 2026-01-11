using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Ends a block such as if, else, while, for, or method.
    /// It also controls where the program should jump next.
    /// </summary>
    public class AppEnd : CompoundCommand, ICommand
    {
        /// <summary>
        /// Stores the text written after the word "end".
        /// This is mainly used to show helpful error messages.
        /// </summary>
        private string endType = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppEnd() : base() { }

        /// <summary>
        /// Checks parameters but no checks are needed here.
        /// </summary>
        /// <param name="parameter">Parameters passed to the command.</param>
        public override void CheckParameters(string[] parameter) { }

        /// <summary>
        /// Links this END to the block it is closing
        /// and sets where execution should jump.
        /// </summary>
        public override void Compile()
        {
            /// <summary>
            /// Reads the word after "end" such as "if" or "while".
            /// </summary>
            endType = (this.ParameterList ?? string.Empty).Trim().ToLowerInvariant();

            /// <summary>
            /// Stores the line number where this END appears in the program.
            /// </summary>
            this.LineNumber = this.Program.Count - 1;

            /// <summary>
            /// Takes the last open block from the stack.
            /// This should be an if, else, while, for, or method.
            /// </summary>
            ConditionalCommand popped = this.Program.Pop();
            if (popped == null)
                throw new CanvasException($"end {endType} without matching statement");

            /// <summary>
            /// If this END closes an ELSE block,
            /// the ELSE must jump to the line after this END.
            /// </summary>
            if (popped is AppElse elseCmd)
            {
                elseCmd.EndLineNumber = this.LineNumber + 1;
                this.CorrespondingCommand = elseCmd;
                return;
            }

            /// <summary>
            /// If this END closes an IF, WHILE, FOR, or METHOD,
            /// the false or skip jump must go to the line after this END.
            /// </summary>
            if (popped is AppIf || popped is AppWhile || popped is AppFor || popped is AppMethod)
            {
                popped.EndLineNumber = this.LineNumber + 1;
                this.CorrespondingCommand = popped;
                return;
            }

            /// <summary>
            /// If the block type is not recognised, show an error.
            /// </summary>
            throw new CanvasException($"end {endType} does not match supported block type");
        }

        /// <summary>
        /// Runs when the END line is reached during execution.
        /// It handles looping back or returning from a method.
        /// </summary>
        public override void Execute()
        {
            /// <summary>
            /// If this END belongs to a WHILE loop and the condition is still true,
            /// the program jumps back to the WHILE line.
            /// </summary>
            if (this.CorrespondingCommand is AppWhile whileCmd)
            {
                if (whileCmd.Condition)
                {
                    this.Program.PC = whileCmd.LineNumber - 1;
                }
                return;
            }

            /// <summary>
            /// If this END belongs to a FOR loop and the condition is still true,
            /// the loop variable is updated and the program jumps back to the FOR line.
            /// </summary>
            if (this.CorrespondingCommand is AppFor forCmd)
            {
                if (forCmd.Condition)
                {
                    int current = int.Parse(this.Program.GetVarValue(forCmd.LoopVariableName));
                    current += forCmd.Step;
                    this.Program.UpdateVariable(forCmd.LoopVariableName, current);

                    this.Program.PC = forCmd.LineNumber - 1;
                }
                return;
            }

            /// <summary>
            /// If this END belongs to a METHOD,
            /// the program jumps back to the line where the method was called.
            /// </summary>
            if (this.CorrespondingCommand is AppMethod methodCmd)
            {
                if (methodCmd.ReturnLine != -1)
                {
                    this.Program.PC = methodCmd.ReturnLine;
                    methodCmd.ReturnLine = -1;
                }
                return;
            }

            /// <summary>
            /// If this END belongs to an IF or ELSE block,
            /// nothing special is needed at runtime here.
            /// </summary>
        }
    }
}
