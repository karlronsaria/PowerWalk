/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/7/2016
 * Time: 11:39 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace PowerWalk.Tests
{
    [TestFixture]
    public class TestParserMethods
    {
        SortedStringSet[] binops    =  Operators.GetBinaryOperators();
        SortedStringSet   assignops =  Operators.GetAssignmentOperators();
		
        public static readonly string[] testGroups =
        {
            "(1)", " (14) ", "( 0.004 )", "(  \"Sample Text\" )  ", "'c'", "((( 14 + (2) )))", "(-(-(-(-(-1)))))"
        };
        
        public static readonly object[] resultGroups = 
        {
            1L, 14L, 0.004, "Sample Text", null, 16L, -1L
        };
        
        public static readonly string[] testLists =
        {
            "1, 2, 5, 7, 8, 14, 97, 1445",
            "1.003, 14, 90001.2, 17, 0.0",
            "17, 13.2, \"Sample Text\", 'c', null",
            "((17)), 3 + 7 * 2, (3 + 7) * 2, 3 > 4 or 4 = 8 / 2",
            "(null), (null), null, null, null",
            "(null)",
            "null"
        };
        
        public static readonly object[][] resultLists =
        {
            new object[]{1L, 2L, 5L, 7L, 8L, 14L, 97, 1445},
            new object[]{1.003, 14L, 90001.2, 17L, 0.0},
            new object[]{17L, 13.2, "Sample Text", 'c', null},
            new object[]{17L, 17L, 20L, true},
            new object[]{null, null, null, null, null},
            new object[]{null},
            new object[]{null}
        };
        
        public static readonly string[] testChains =
        {
            "\"Sample Text\"[0]",
            "\"Sample Text\"[10]",
            "{0, 1, 2, 3, 4, 5}[2]",
            "{ {2, 4, 6}, {3, 5, 7}, {11, 13, 17} }[1][2]",
            "{ {\"ABC\", \"DEF\"}, {\"GHI\", \"JKL\"}, {\"MNO\", \"PQS\", \"TUV\"} }[1][0][1]"
        };
        
        public static readonly object[] resultChains =
        {
            'S', 't', 2L, 7L, 'H'
        };
        
        public static readonly string[] testExpressions =
        {
            "'\n'",
            "\"what\" plus '\n'",
            "12 < 11 and \"Sample Text\"[12] = 'z'",
            "5 < 11 or \"Sample Text\"[12] = 'z'",
            "500 / 0.0"
        };
        
        public static readonly object[] resultExpressions =
        {
            '\n', "what\n", false, true, Double.PositiveInfinity
        };
        
        [Test]
        public void TestParseGroup()
        {
            var main = new Interpreter.Sequence();
            
            for (int i = 0; i < testGroups.Length; ++i)
                Assert.AreEqual(resultGroups[i], main.ParseGroup(testGroups[i]));
        }
        
        [Test]
        public void TestParseList()
        {
            var main = new Interpreter.Sequence();
            ArrayList list;
            
            for (int i = 0; i < testLists.Length; ++i)
            {
                list = main.ParseList(testLists[i]);
                
                for (int j = 0; j < list.Count; ++j)
                {
                    Assert.AreEqual(resultLists[i][j], list[j]);
                }
            }
        }
        
        [Test]
        public void TestParseMixedGroupExpression()
        {
            var main = new Interpreter.Sequence();
            
            for (int i = 0; i < testChains.Length; ++i)
            {
                Assert.AreEqual(resultChains[i], main.ParseMixedGroupExpression(testChains[i]));
            }
        }
        
        [Test]
        public void TestParseExpression()
        {
            var main = new Interpreter.Sequence();
            
            for (int i = 0; i < testExpressions.Length; ++i)
                Assert.AreEqual(resultExpressions[i], main.Expression(testExpressions[i]));
        }
    }
}
