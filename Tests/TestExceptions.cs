/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/17/2016
 * Time: 3:19 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;

namespace PowerWalk.Tests
{
    [TestFixture]
    public class TestExceptions
    {
        public static string[] formatTests = new string[]
        {
            "    declare",
            "    declare 13num := 13",
            "    asdf what",
            "    declare v := \n",
            "    var := (3 /)",
            "    for every 3 v in ",
            "    declare newVar\n    assign newVar := {(), (), , , }[2]",
            "    declare newVar\n    assign newVar := 3 * ()"
        };
        
        public static string[] formatResults = new string[]
        {
            "Line 1: Syntax Error: No valid variable name in the phrase: \"\".",
            "Line 1: Syntax Error: No valid variable name in the phrase: \"13num\".",
            "Line 1: Syntax Error: No valid variable name in the phrase: \"asdf what\".",
            "Line 1: Syntax Error: Not a valid expression.",
            "Line 1: Syntax Error: Not a valid expression.",
            "Line 1: Syntax Error: Not a valid expression.",
            "Line 2: Syntax Error: Not a valid expression.",
            "Line 2: Syntax Error: Not a valid expression."
        };
        
        public static string[] indexTests = new string[]
        {
            "    writeline (5 < 11 and \"Sample Text\"[12] = 'z')",
            "    writeline (12 < 11 or \"Sample Text\"[12] = 'z')"
        };
        
        public static string[] indexResults = new string[]
        {
            "Index Out Of Range: Index was outside the bounds of the array.",
            "Index Out Of Range: Index was outside the bounds of the array."
        };
        
        public static string[] castTests = new string[]
        {
            "    declare what := 3 < 4 and \"ersatz\""
        };
        
        public static string[] castResults = new string[]
        {
            "Invalid Cast Exception: The 'and' operator could not be applied to the given types: Boolean, String."
        };
        
        public static string[] constTests = new string[]
        {
            "    declare const WHAT := 4\n    assign WHAT := 5",
            "    const WHAT := 4\n    WHAT := 5",
            "    const WHAT := 4\n    declare WHAT := 5",
            "    declare const WHAT := 4\n    clear WHAT",
            "    declare const WHAT := 4\n    release WHAT",
            "    declare const WHAT := 4\n    assign 5 =: WHAT"
        };
        
        public static string constResult =
            "Line 2: Permission denied: Cannot overwrite value.";
        
        public static string[] overZeroTests = new string[]
        {
            "    write 4/0"
        };
        
        public static string overZeroResult =
            "Line 1: Divide By Zero: Attempted to divide by zero.";
            
        [Test]
        public void TestFormatExceptions()
        {
            var main = new Interpreter.Sequence();
            
            for (int i = 0; i < formatTests.Length; ++i)
            {
                try
                {
                    main.Run(formatTests[i]);
                }
                catch (Exception e)
                {
                    Assert.AreEqual(formatResults[i], e.Message);
                }
            }
        }
        
        [Test]
        public void TestIndexOutOfBoundsExceptions()
        {
            var main = new Interpreter.Sequence();
            
            for (int i = 0; i < indexTests.Length; ++i)
            {
                try
                {
                    main.Run(indexTests[i]);
                }
                catch (Exception e)
                {
                    Assert.AreEqual(indexResults[i], e.Message);
                }
            }
        }
        
        [Test]
        public void TestInvalidCastExceptions()
        {
            var main = new Interpreter.Sequence();
            
            for (int i = 0; i < castTests.Length; ++i)
            {
                try
                {
                    main.Run(castTests[i]);
                }
                catch (Exception e)
                {
                    Assert.AreEqual(castResults[i], e.Message);
                }
            }
        }
        
        [Test]
        public void TestPermissionDeniedExceptions()
        {
            var main = new Interpreter.Sequence();
            
            for (int i = 0; i < constTests.Length; ++i)
            {
                try
                {
                    main.Run(constTests[i]);
                }
                catch (Exception e)
                {
                    Assert.AreEqual(constResult, e.Message);
                }
            }
        }
        
        [Test]
        public void TestDivideByZeroExceptions()
        {
            var main = new Interpreter.Sequence();
            
            for (int i = 0; i < overZeroTests.Length; ++i)
            {
                try
                {
                    main.Run(overZeroTests[i]);
                }
                catch (Exception e)
                {
                    Assert.AreEqual(overZeroResult, e.Message);
                }
            }
        }
    }
}
