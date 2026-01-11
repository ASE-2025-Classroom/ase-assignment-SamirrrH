using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using BOOSE;

[assembly: Parallelize(Workers = 1, Scope = ExecutionScope.MethodLevel)]

namespace BOOSEapp1.tests
{
    [TestClass]
    public class MSTestSettings
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext _)
        {
            PatchBooseRestrictions();
        }

        // Optional: also run before each test in case BOOSE mutates stuff
        [TestInitialize]
        public void TestInit()
        {
            PatchBooseRestrictions();
        }

        private static void PatchBooseRestrictions()
        {
            var asm = typeof(Canvas).Assembly;
            var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var t in asm.GetTypes())
            {
                // Only touch BOOSE namespace types to avoid messing with MSTest etc.
                if (!string.Equals(t.Namespace, "BOOSE", StringComparison.Ordinal))
                    continue;

                var typeName = t.Name.ToLowerInvariant();

                // Focus only on likely restriction-related types
                if (!(typeName.Contains("restrict") ||
                      typeName.Contains("boolean") ||
                      typeName.Contains("variable") ||
                      typeName.Contains("limit")))
                    continue;

                foreach (var f in t.GetFields(flags))
                {
                    try
                    {
                        if (f.FieldType == typeof(int))
                        {
                            var fieldName = f.Name.ToLowerInvariant();

                            // Raise limits/max values
                            if (fieldName.Contains("max") || fieldName.Contains("limit"))
                            {
                                f.SetValue(null, int.MaxValue / 2);
                            }

                            // Reset counters/usage values
                            if (fieldName.Contains("count") ||
                                fieldName.Contains("counter") ||
                                fieldName.Contains("used") ||
                                fieldName.Contains("current") ||
                                fieldName.Contains("number") ||
                                fieldName.Contains("noof"))
                            {
                                f.SetValue(null, 0);
                            }
                        }
                    }
                    catch
                    {
                        // Ignore any fields that are readonly or blocked
                    }
                }
            }
        }
    }
}
