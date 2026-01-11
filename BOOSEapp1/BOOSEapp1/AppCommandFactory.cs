using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Creates the correct command object based on the command name.
    /// </summary>
    public class AppCommandFactory : CommandFactory
    {
        /// <summary>
        /// Builds and returns the right command for the given text.
        /// </summary>
        /// <param name="commandType">The command name from the script.</param>
        /// <returns>The matching command object.</returns>
        public override ICommand MakeCommand(string commandType)
        {
            /// <summary>
            /// Cleans the command text so it is safe to compare.
            /// </summary>
            commandType = (commandType ?? "").Trim().ToLowerInvariant();

            /// <summary>
            /// Drawing commands.
            /// </summary>
            if (commandType == "circle") return new AppCircle();
            if (commandType == "moveto") return new AppMoveTo();
            if (commandType == "rect") return new AppRect();
            if (commandType == "write") return new AppWrite();

            /// <summary>
            /// Variable commands.
            /// </summary>
            if (commandType == "int") return new AppInteg();
            if (commandType == "real") return new AppReal();

            /// <summary>
            /// Array commands.
            /// </summary>
            if (commandType == "array") return new AppArray();
            if (commandType == "poke") return new AppPoke();
            if (commandType == "peek") return new AppPeek();

            /// <summary>
            /// Flow control commands.
            /// </summary>
            if (commandType == "if") return new AppIf();
            if (commandType == "else") return new AppElse();
            if (commandType == "while") return new AppWhile();
            if (commandType == "for") return new AppFor();
            if (commandType == "end") return new AppEnd();

            /// <summary>
            /// Method commands.
            /// </summary>
            if (commandType == "method") return new AppMethod();
            if (commandType == "call") return new AppCall();

            /// <summary>
            /// Uses the default factory if no match is found.
            /// </summary>
            return base.MakeCommand(commandType);
        }
    }
}
