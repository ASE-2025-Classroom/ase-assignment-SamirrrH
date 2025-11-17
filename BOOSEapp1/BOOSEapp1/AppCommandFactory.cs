using BOOSE;
using BOOSEapp1;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Factory that creates the correct command based on the given name.
    /// </summary>
    internal class AppCommandFactory : CommandFactory
    {
        /// <summary>
        /// Returns a command object that matches the requested command name.
        /// </summary>
        /// <param name="commandType">The command name typed by the user.</param>
        /// <returns>A matching command, or the base factory’s version if not found.</returns>
        public override ICommand MakeCommand(string commandType)
        {
            commandType = commandType.ToLowerInvariant();

            if (commandType == "circle")
            {
                return new AppCircle();
            }

            if (commandType == "moveto")
            {
                return new AppMoveTo();
            }

            if (commandType == "rect")
            {
                return new AppRect();
            }

            return base.MakeCommand(commandType);
        }
    }
}