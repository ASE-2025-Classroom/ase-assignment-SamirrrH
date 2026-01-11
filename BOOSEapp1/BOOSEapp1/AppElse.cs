using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Handles the "else" part of an if statement.
    /// </summary>
    public class AppElse : CompoundCommand, ICommand
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppElse() : base() { }

        /// <summary>
        /// Checks parameters (else has none).
        /// </summary>
        /// <param name="parameter">List of parameters passed to the command.</param>
        public override void CheckParameters(string[] parameter)
        {
            // No validation
        }

        /// <summary>
        /// Links this ELSE to the matching IF and sets jump locations.
        /// </summary>
        public override void Compile()
        {
            /// <summary>
            /// Stores where this ELSE line is in the program.
            /// </summary>
            this.LineNumber = this.Program.Count;

            /// <summary>
            /// Gets the last open conditional command (should be an IF).
            /// </summary>
            ConditionalCommand correspondingIf = this.Program.Pop();

            if (correspondingIf == null || !(correspondingIf is AppIf))
                throw new CanvasException("else without matching if");

            /// <summary>
            /// If the IF condition is false, it should jump to this ELSE line.
            /// </summary>
            correspondingIf.EndLineNumber = this.LineNumber;

            /// <summary>
            /// Keeps a reference to the matching IF.
            /// </summary>
            this.CorrespondingCommand = correspondingIf;

            /// <summary>
            /// Pushes ELSE so END can match and close it later.
            /// </summary>
            this.Program.Push(this);
        }

        /// <summary>
        /// Runs when execution reaches ELSE.
        /// If the IF part already ran (condition was true), skip to END.
        /// </summary>
        public override void Execute()
        {
            // If IF was true, execution reaches ELSE and we skip to END
            this.Program.PC = this.EndLineNumber;
        }
    }
}
