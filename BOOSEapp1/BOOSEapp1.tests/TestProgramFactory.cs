using BOOSE;
using BOOSEapp1;

namespace BOOSEapp1.tests
{
    internal static class TestProgramFactory
    {
        public static StoredProgram CreateProgram()
        {
            // Your BOOSE StoredProgram constructor expects an ICanvas
            var canvas = new AppCanvas(800, 600);
            var program = new StoredProgram(canvas);

            return program;
        }
    }
}
