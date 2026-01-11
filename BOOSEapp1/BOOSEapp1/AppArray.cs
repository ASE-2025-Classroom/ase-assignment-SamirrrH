#nullable disable

using BOOSE;
using System;

namespace BOOSEapp1
{
    /// <summary>
    /// Custom array class that replaces BOOSE's Array class.
    /// It supports both int arrays and real (double) arrays.
    /// </summary>
    public class AppArray : Evaluation, ICommand
    {
        /// <summary>
        /// Constant used to mean "read from the array".
        /// </summary>
        protected const bool PEEK = false;

        /// <summary>
        /// Constant used to mean "write into the array".
        /// </summary>
        protected const bool POKE = true;

        /// <summary>
        /// Stores what type of array this is: "int" or "real".
        /// </summary>
        public string type;

        /// <summary>
        /// The number of rows in the array.
        /// </summary>
        protected int rows;

        /// <summary>
        /// The number of columns in the array.
        /// If the user makes a 1D array, this stays as 1.
        /// </summary>
        protected int columns;

        /// <summary>
        /// The real storage for an int array.
        /// </summary>
        protected int[,] intArray;

        /// <summary>
        /// The real storage for a real (double) array.
        /// </summary>
        protected double[,] realArray;

        /// <summary>
        /// Stores the row text written in peek/poke.
        /// This can be a number or an expression.
        /// </summary>
        protected string rowS;

        /// <summary>
        /// Stores the column text written in peek/poke.
        /// This can be a number or an expression.
        /// </summary>
        protected string columnS;

        /// <summary>
        /// The final row index after evaluation.
        /// </summary>
        protected int row;

        /// <summary>
        /// The final column index after evaluation.
        /// </summary>
        protected int column;

        /// <summary>
        /// The final value to write when poking an int array.
        /// </summary>
        protected int valueInt;

        /// <summary>
        /// The final value to write when poking a real array.
        /// </summary>
        protected double valueReal;

        /// <summary>
        /// Stores the raw value text after "=" in a poke command.
        /// </summary>
        protected string pokeValue;

        /// <summary>
        /// Stores the target variable name on the left side of "=" in a peek command.
        /// </summary>
        protected string peekVar;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AppArray() : base()
        {
        }

        /// <summary>
        /// Checks parameters but no checks are needed here.
        /// </summary>
        /// <param name="parameterList">Parameters passed to the command.</param>
        public override void CheckParameters(string[] parameterList)
        {
            // No validation
        }

        /// <summary>
        /// Reads an array declaration.
        /// Example: "int myArr 10" or "real myArr 5,5".
        /// </summary>
        public override void Compile()
        {
            /// <summary>
            /// Splits the declaration into type, name, and size text.
            /// </summary>
            string[] parts = this.ParameterList.Split(
                new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            /// <summary>
            /// Needs at least: type, name, and size.
            /// </summary>
            if (parts.Length < 3)
                throw new CanvasException("Array declaration requires: type name size");

            /// <summary>
            /// Stores the type and variable name.
            /// </summary>
            type = parts[0].ToLowerInvariant();
            varName = parts[1];

            /// <summary>
            /// Splits the size into rows and optional columns (rows,cols).
            /// </summary>
            string[] dims = parts[2].Split(',');

            /// <summary>
            /// Reads the rows value and makes sure it is valid.
            /// </summary>
            if (!int.TryParse(dims[0], out rows) || rows < 1)
                throw new CanvasException("Array rows must be a positive integer");

            /// <summary>
            /// Default columns to 1 for a one-dimensional array.
            /// </summary>
            columns = 1;

            /// <summary>
            /// If a second value is provided, treat it as columns.
            /// </summary>
            if (dims.Length > 1)
            {
                if (!int.TryParse(dims[1], out columns) || columns < 1)
                    throw new CanvasException("Array columns must be a positive integer");
            }

            /// <summary>
            /// Registers the array variable in the program.
            /// </summary>
            Program.AddVariable(this);
        }

        /// <summary>
        /// Creates the actual array storage based on the chosen type.
        /// </summary>
        public override void Execute()
        {
            /// <summary>
            /// Creates an int[,] array if the type is int.
            /// </summary>
            if (type == "int")
                intArray = new int[rows, columns];
            /// <summary>
            /// Creates a double[,] array if the type is real.
            /// </summary>
            else if (type == "real")
                realArray = new double[rows, columns];
            /// <summary>
            /// If the type is not recognised, stop with an error.
            /// </summary>
            else
                throw new CanvasException($"Unknown array type '{type}'");
        }

        /// <summary>
        /// Stores an integer value into the int array at the given position.
        /// </summary>
        /// <param name="val">The value to store.</param>
        /// <param name="r">Row index.</param>
        /// <param name="c">Column index.</param>
        public void SetIntArray(int val, int r, int c)
        {
            /// <summary>
            /// Checks the indexes are inside the array size.
            /// </summary>
            BoundsCheck(r, c);

            /// <summary>
            /// Stores the value into the array.
            /// </summary>
            intArray[r, c] = val;
        }

        /// <summary>
        /// Stores a real (double) value into the real array at the given position.
        /// </summary>
        /// <param name="val">The value to store.</param>
        /// <param name="r">Row index.</param>
        /// <param name="c">Column index.</param>
        public void SetRealArray(double val, int r, int c)
        {
            /// <summary>
            /// Checks the indexes are inside the array size.
            /// </summary>
            BoundsCheck(r, c);

            /// <summary>
            /// Stores the value into the array.
            /// </summary>
            realArray[r, c] = val;
        }

        /// <summary>
        /// Gets an integer value from the int array at the given position.
        /// </summary>
        /// <param name="r">Row index.</param>
        /// <param name="c">Column index.</param>
        /// <returns>The value stored at that location.</returns>
        public int GetIntArray(int r, int c)
        {
            /// <summary>
            /// Checks the indexes are inside the array size.
            /// </summary>
            BoundsCheck(r, c);

            /// <summary>
            /// Returns the value from the array.
            /// </summary>
            return intArray[r, c];
        }

        /// <summary>
        /// Gets a real (double) value from the real array at the given position.
        /// </summary>
        /// <param name="r">Row index.</param>
        /// <param name="c">Column index.</param>
        /// <returns>The value stored at that location.</returns>
        public double GetRealArray(int r, int c)
        {
            /// <summary>
            /// Checks the indexes are inside the array size.
            /// </summary>
            BoundsCheck(r, c);

            /// <summary>
            /// Returns the value from the array.
            /// </summary>
            return realArray[r, c];
        }

        /// <summary>
        /// Makes sure the row and column are valid for this array.
        /// </summary>
        /// <param name="r">Row index.</param>
        /// <param name="c">Column index.</param>
        private void BoundsCheck(int r, int c)
        {
            /// <summary>
            /// If row or column is outside the array bounds, stop with an error.
            /// </summary>
            if (r < 0 || r >= rows || c < 0 || c >= columns)
                throw new CanvasException($"Array index out of bounds [{r},{c}]");
        }

        /// <summary>
        /// Reads peek/poke text and stores the parts during compile time.
        /// This works for both "peek" and "poke" formats.
        /// </summary>
        /// <param name="poke">True for poke, false for peek.</param>
        protected void ProcessArrayParametersCompile(bool poke)
        {
            /// <summary>
            /// Gets the full text and finds the "=" sign.
            /// </summary>
            string text = ParameterList.Trim();
            int eq = text.IndexOf('=');

            /// <summary>
            /// Peek and poke must have "=" so we know left and right side.
            /// </summary>
            if (eq == -1)
                throw new CanvasException("Peek/Poke requires '='");

            /// <summary>
            /// Poke format: arrayName row [col] = value
            /// </summary>
            if (poke)
            {
                /// <summary>
                /// Stores the raw value text after "=".
                /// </summary>
                pokeValue = text[(eq + 1)..].Trim();

                /// <summary>
                /// Splits the left side into array name, row, and optional column.
                /// </summary>
                string[] left = text[..eq].Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                /// <summary>
                /// Stores the array name and index texts.
                /// Column defaults to 0 for one-dimensional use.
                /// </summary>
                varName = left[0];
                rowS = left[1];
                columnS = left.Length > 2 ? left[2] : "0";
            }
            /// <summary>
            /// Peek format: targetVar = arrayName row [col]
            /// </summary>
            else
            {
                /// <summary>
                /// Stores the target variable name on the left side.
                /// </summary>
                peekVar = text[..eq].Trim();

                /// <summary>
                /// Splits the right side into array name, row, and optional column.
                /// </summary>
                string[] right = text[(eq + 1)..].Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                /// <summary>
                /// Stores the array name and index texts.
                /// Column defaults to 0 for one-dimensional use.
                /// </summary>
                varName = right[0];
                rowS = right[1];
                columnS = right.Length > 2 ? right[2] : "0";
            }
        }

        /// <summary>
        /// Evaluates the row and column indexes.
        /// If this is a poke, it also evaluates the value to store.
        /// </summary>
        /// <param name="poke">True for poke, false for peek.</param>
        protected void ProcessArrayParametersExecute(bool poke)
        {
            /// <summary>
            /// Converts the row and column text into integer indexes.
            /// </summary>
            row = EvaluateIndex(rowS);
            column = EvaluateIndex(columnS);

            /// <summary>
            /// If this is a peek, we only needed the indexes.
            /// </summary>
            if (!poke) return;

            /// <summary>
            /// Reads the array so we know if we should store an int or a real.
            /// </summary>
            AppArray array = (AppArray)this.Program.GetVariable(this.varName);

            /// <summary>
            /// For int arrays, parse the value as int or evaluate it as an expression.
            /// </summary>
            if (array.type == "int")
            {
                if (int.TryParse(pokeValue, out int directInt))
                {
                    valueInt = directInt;
                }
                else
                {
                    string eval = Program.EvaluateExpression(pokeValue);
                    if (!int.TryParse(eval, out valueInt))
                        throw new CanvasException($"Cannot parse int value: {eval}");
                }
            }
            /// <summary>
            /// For real arrays, parse the value as double or evaluate it as an expression.
            /// </summary>
            else if (array.type == "real")
            {
                if (double.TryParse(pokeValue, out double directReal))
                {
                    valueReal = directReal;
                }
                else
                {
                    string eval = Program.EvaluateExpression(pokeValue);
                    if (!double.TryParse(eval, out valueReal))
                        throw new CanvasException($"Cannot parse real value: {eval}");
                }
            }
            /// <summary>
            /// If the array type is unknown, stop with an error.
            /// </summary>
            else
            {
                throw new CanvasException($"Unknown array type '{array.type}'");
            }
        }

        /// <summary>
        /// Converts an index text into an integer index.
        /// It accepts a number like "3" or an expression like "i+1".
        /// </summary>
        /// <param name="expr">The index text.</param>
        /// <returns>The integer index.</returns>
        private int EvaluateIndex(string expr)
        {
            /// <summary>
            /// If it is already a number, return it directly.
            /// </summary>
            if (int.TryParse(expr, out int v)) return v;

            /// <summary>
            /// Otherwise evaluate it as an expression and parse the result.
            /// </summary>
            string eval = Program.EvaluateExpression(expr);
            if (!int.TryParse(eval, out int result))
                throw new CanvasException($"Cannot parse index value: {eval}");

            return result;
        }
    }
}
