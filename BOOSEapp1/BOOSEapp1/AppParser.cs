using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Custom parser that swaps in your own commands for key keywords.
    /// This avoids BOOSE limits and also cleans hidden characters from input lines.
    /// </summary>
    public class AppParser : Parser
    {
        /// <summary>
        /// The program that stores commands and variables while parsing.
        /// </summary>
        private readonly StoredProgram storedProgram;

        /// <summary>
        /// Creates a parser that uses your command factory and program.
        /// </summary>
        /// <param name="factory">Creates command objects from keywords.</param>
        /// <param name="program">Stores the parsed commands and variables.</param>
        public AppParser(CommandFactory factory, StoredProgram program)
            : base(factory, program)
        {
            storedProgram = program;
        }

        /// <summary>
        /// Turns one line of script into an ICommand object.
        /// Some keywords are handled here so your custom versions are used.
        /// </summary>
        /// <param name="line">A single script line.</param>
        /// <returns>A command object, or null for a blank line.</returns>
        public override ICommand ParseCommand(string line)
        {
            /// <summary>
            /// Trims spaces and removes hidden characters that can break keywords.
            /// </summary>
            string trimmedLine = (line ?? string.Empty)
                .Trim()
                .Trim('\uFEFF', '\u200B', '\u200C', '\u200D');

            /// <summary>
            /// If the line is empty, return null so it is ignored.
            /// </summary>
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
#pragma warning disable CS8603
                return null;
#pragma warning restore CS8603
            }

            /// <summary>
            /// Splits the line into a keyword (first word) and the rest of the text.
            /// </summary>
            string[] split = trimmedLine.Split(new[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
            string keyword = split[0].Trim().ToLowerInvariant();
            string rest = split.Length > 1 ? split[1].Trim() : string.Empty;

            /// <summary>
            /// Handles: int name [= value]
            /// Uses AppInteg instead of BOOSE Int.
            /// </summary>
            if (keyword == "int")
            {
                var cmd = new AppInteg();
                cmd.Set(storedProgram, rest);
                cmd.Compile();
                return cmd;
            }

            /// <summary>
            /// Handles: real name [= value]
            /// Uses AppReal instead of BOOSE Real.
            /// </summary>
            if (keyword == "real")
            {
                var cmd = new AppReal();
                cmd.Set(storedProgram, rest);
                cmd.Compile();
                return cmd;
            }

            /// <summary>
            /// Handles: array type name size
            /// Uses AppArray for custom array support.
            /// </summary>
            if (keyword == "array")
            {
                var cmd = new AppArray();
                cmd.Set(storedProgram, rest);
                cmd.Compile();
                return cmd;
            }

            /// <summary>
            /// Handles: if condition
            /// Uses AppIf so your condition rules are used.
            /// </summary>
            if (keyword == "if")
            {
                var cmd = new AppIf();
                cmd.Set(storedProgram, rest);
                cmd.Compile();
                return cmd;
            }

            /// <summary>
            /// Handles: else
            /// Uses AppElse to link to the matching if.
            /// </summary>
            if (keyword == "else")
            {
                var cmd = new AppElse();
                cmd.Set(storedProgram, rest);
                cmd.Compile();
                return cmd;
            }

            /// <summary>
            /// Handles: while condition
            /// Uses AppWhile so your condition rules are used.
            /// </summary>
            if (keyword == "while")
            {
                var cmd = new AppWhile();
                cmd.Set(storedProgram, rest);
                cmd.Compile();
                return cmd;
            }

            /// <summary>
            /// Handles: for variable = from to to [step step]
            /// Uses AppFor so your loop rules are used.
            /// </summary>
            if (keyword == "for")
            {
                var cmd = new AppFor();
                cmd.Set(storedProgram, rest);
                cmd.Compile();
                return cmd;
            }

            /// <summary>
            /// Handles: end ...
            /// Uses AppEnd to close if/else/while/for/method blocks.
            /// </summary>
            if (keyword == "end")
            {
                var cmd = new AppEnd();
                cmd.Set(storedProgram, rest);
                cmd.Compile();
                return cmd;
            }

            /// <summary>
            /// Handles: method returnType methodName ...
            /// Uses AppMethod to define a method block.
            /// </summary>
            if (keyword == "method")
            {
                var cmd = new AppMethod();
                cmd.Set(storedProgram, rest);
                cmd.Compile();
                return cmd;
            }

            /// <summary>
            /// Handles: call methodName ...
            /// Uses AppCall to jump into a method and pass parameters.
            /// </summary>
            if (keyword == "call")
            {
                var cmd = new AppCall();
                cmd.Set(storedProgram, rest);
                cmd.Compile();
                return cmd;
            }

            /// <summary>
            /// If a line looks like "x = something", treat it as an assignment command.
            /// This only happens if the variable already exists.
            /// </summary>
            if (IsStandaloneAssignment(trimmedLine))
            {
                var eval = new AppEvaluation();
                eval.Set(storedProgram, trimmedLine);
                eval.Compile();
                return eval;
            }

            /// <summary>
            /// For other commands (circle, moveto, rect, etc.) use the normal BOOSE parser.
            /// </summary>
            return base.ParseCommand(trimmedLine);
        }

        /// <summary>
        /// Checks if a line should be treated as a normal assignment like "x = 2*y".
        /// It returns false for keywords like "int", "if", "while", and so on.
        /// </summary>
        /// <param name="line">The full line text.</param>
        /// <returns>True if it is a valid standalone assignment line.</returns>
        private bool IsStandaloneAssignment(string line)
        {
            /// <summary>
            /// If there is no "=", it cannot be an assignment.
            /// </summary>
            if (!line.Contains("=")) return false;

            /// <summary>
            /// Used to check the start of the line against keywords.
            /// </summary>
            string start = line.TrimStart();

            /// <summary>
            /// Keywords that should not be treated as assignments.
            /// </summary>
            string[] keywords =
            {
                "int","real","array","poke","peek",
                "if","else","end","while","for",
                "method","call"
            };

            /// <summary>
            /// If the line starts with any keyword, do not treat it as an assignment.
            /// </summary>
            foreach (string kw in keywords)
            {
                if (start.Equals(kw, StringComparison.OrdinalIgnoreCase) ||
                    start.StartsWith(kw + " ", StringComparison.OrdinalIgnoreCase) ||
                    start.StartsWith(kw + "\t", StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            /// <summary>
            /// Gets the left side of the "=" and reads the variable name.
            /// </summary>
            string left = line.Split('=')[0].Trim();
            if (string.IsNullOrWhiteSpace(left)) return false;

            string varName = left.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)[0];

            /// <summary>
            /// Only treat it as an assignment if the variable exists already.
            /// </summary>
            return storedProgram.VariableExists(varName);
        }
    }
}
