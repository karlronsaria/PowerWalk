/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/9/2016
 * Time: 3:36 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;

namespace PowerWalk.Tests
{
    [TestFixture]
    public class TestVerbs
    {
        public readonly string[] testAssignments =
        {
            "var := \"Sample Text\"",
            "var1 := 3 * 2 + 1",
            "var_2 := var1 + 3",
            "\"More \" + var =: _var3",
            "13.5->var_4_",
            "\"\"->var5"
        };
        
        public readonly string[] resultAssignNames =
        {
            "var", "var1", "var_2", "_var3", "var_4_", "var5"
        };
        
        public readonly object[] resultAssignValues =
        {
            "Sample Text", 7L, 10L, "More Sample Text", 13.5, ""
        };
        
        public readonly string[] testSums =
        {
            "var := \" And More\"",
            "var1 := 13",
            "var_2 := var1 - 11",
            "\": Let me tell you about it.\" =: _var3",
            "-2.2->var_4_",
            "\"foobar\" plus var_2->var5"
        };
        
        public readonly object[] resultSums =
        {
            "Sample Text And More", 20L, 19L,
            "More Sample Text: Let me tell you about it.", 11.3,
            "foobar19"
        };
        
        public readonly string[] numerics =
        {
            "var1", "var_2", "var_4_"
        };
        
        public readonly string[] testDifferences =
        {
            "var1 := 13",
            "var_2 := var1 - 11",
            "-2.2->var_4_"
        };
        
        public readonly object[] resultDifferences =
        {
            -6L, 27L, 15.7
        };
        
        public readonly string[] testProducts =
        {
            "var1 := 5.2",
            "var_2 := var1 - 1.1",
            "-2.2->var_4_"
        };
        
        public readonly object[] resultProducts =
        {
            36.4, 353, -29.7
        };
        
        public readonly string[] testQuotients =
        {
            "var1 := 2.1",
            "var_2 := var1 - 1.1",
            "-0.92->var_4_"
        };
        
        public readonly object[] resultQuotients =
        {
            3.3333, 4.4776, -14.6739
        };
        
        public readonly string[] assignHighComposites =
        {
            "var1 := 300",
            "var_2 := var1 - 90",
            "-2000->var_4_"
        };
        
        public readonly string[] testRemainders =
        {
            "var1 := 40",
            "var_2 := var1 - 220",
            "-12->var_4_"
        };
        
        public readonly object[] resultRemainders =
        {
            20, 10, -8
        };
        
        public readonly object[] resultIncrements =
        {
            8L, 11L, 14.5
        };
        
        public readonly object[] resultDecrements =
        {
            6L, 9L, 12.5
        };
        
        public Interpreter.Sequence TestAssignTo()
        {
            var main = new Interpreter.Sequence();
            
            for (int i = 0; i < testAssignments.Length; ++i)
            {
                Verbs.AssignTo(main, testAssignments[i]);
                
                Assert.AreEqual(resultAssignValues[i], main.GetNamedValue(resultAssignNames[i]));
            }
            
            return main;
        }
        
        [Test]
        public void TestAddTo()
        {
            var main = TestAssignTo();
            
            for (int i = 0; i < testSums.Length; ++i)
            {
                Verbs.AddTo(main, testSums[i]);
                
                Assert.AreEqual(resultSums[i], main.GetNamedValue(resultAssignNames[i]));
            }
        }
        
        [Test]
        public void TestSubtractFrom()
        {
            var main = TestAssignTo();
            
            for (int i = 0; i < testDifferences.Length; ++i)
            {
                Verbs.SubtractFrom(main, testDifferences[i]);
                
                Assert.AreEqual(resultDifferences[i], main.GetNamedValue(numerics[i]));
            }
        }
        
        [Test]
        public void TestMultiplyBy()
        {
            var main = TestAssignTo();
            
            for (int i = 0; i < testProducts.Length; ++i)
            {
                Verbs.MultiplyBy(main, testProducts[i]);
                
                Assert.AreEqual(Convert.ToDouble(resultProducts[i]),
                                Convert.ToDouble(main.GetNamedValue(numerics[i])), .0001);
            }
        }
        
        [Test]
        public void TestDivideBy()
        {
            var main = TestAssignTo();
            
            for (int i = 0; i < testQuotients.Length; ++i)
            {
                Verbs.DivideBy(main, testQuotients[i]);
                
                Assert.AreEqual(Convert.ToDouble(resultQuotients[i]),
                                Convert.ToDouble(main.GetNamedValue(numerics[i])), .0001);
            }
        }
        
        [Test]
        public void TestRemainderFrom()
        {
            var main = new Interpreter.Sequence();
            
            for (int i = 0; i < assignHighComposites.Length; ++i)
            {
                Verbs.AssignTo(main, assignHighComposites[i]);
            }
            
            for (int i = 0; i < testRemainders.Length; ++i)
            {
                Verbs.RemainderFrom(main, testRemainders[i]);
                
                Assert.AreEqual(resultRemainders[i], main.GetNamedValue(numerics[i]));
            }
        }
        
        [Test]
        public void TestClear()
        {
            var main = TestAssignTo();
            
            for (int i = 0; i < testAssignments.Length; ++i)
            {
                Verbs.Clear(main, resultAssignNames[i]);
                
                Assert.AreEqual(null, main.GetNamedValue(resultAssignNames[i]));
            }
        }
        
        [Test]
        public void TestIncrement()
        {
            var main = TestAssignTo();
            
            for (int i = 0; i < numerics.Length; ++i)
            {
                Verbs.Increment(main, numerics[i]);
                
                Assert.AreEqual(resultIncrements[i], main.GetNamedValue(numerics[i]));
            }
        }
        
        [Test]
        public void TestDecrement()
        {
            var main = TestAssignTo();
            
            for (int i = 0; i < numerics.Length; ++i)
            {
                Verbs.Decrement(main, numerics[i]);
                
                Assert.AreEqual(resultDecrements[i], main.GetNamedValue(numerics[i]));
            }
        }
    }
}
