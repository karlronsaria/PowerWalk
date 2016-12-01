/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/7/2016
 * Time: 5:51 AM
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
    public class TestSplitMethods
    {
        readonly SortedStringSet[] binops = Operators.GetBinaryOperators();
        
        readonly string[][] testWords =
        {
            new string[]{ "left + right", "left ", "+", " right" },
            new string[]{ "what [] .. plus {list, of, values}(param) ", "what [] .. ", "plus", " {list, of, values}(param) " },
            new string[]{ " {list, of, values}(param)minus{list, of, values}(param) ", "", "", "" }
        };
        
        readonly string[][][] testLists =
        {
            new string[][]
            {
                new string[]{"asdf, (aa text) , <wicket> , fdsa, 1234, 4321.ff, ff432 . 6, &&&;;^"},
                new string[]{"asdf", " (aa text) ", " <wicket> ", " fdsa", " 1234", " 4321.ff", " ff432 . 6", " &&&;;^"},
                new string[]{"asdf", "(aa text)", "<wicket>", "fdsa", "1234", "4321.ff", "ff432 . 6", "&&&;;^"}
            }
        };
        
        const string delimiter = ",";
        const string delimiterPattern = "(?<=\\s*),(?=\\s*)";
    
        [Test]
        public void TestSplitTrinomial()
        {
            foreach (string[] arr in testWords)
            {
                string left = "", op = "", right = "";
                
                Interpreter.Sequence.SplitTrinomial(ref left, ref op, ref right, arr[0],
                                                   binops[(int) Operators.OperatorLevel.ADDITIVE],
                                                   Patterns.getbinaryop);
                
                Assert.AreEqual(arr[1], left,  "Compare :  [" + arr[1] + "] - [" + left  + "]");
                Assert.AreEqual(arr[2], op,    "Compare :  [" + arr[2] + "] - [" + op    + "]");
                Assert.AreEqual(arr[3], right, "Compare :  [" + arr[3] + "] - [" + right + "]");
            }
        }
        
        [Test]
        public void TestSplit()
        {
            List<string> list;
            
            foreach (string[][] arr in testLists)
            {
                list = MicroRegex.Match.Split(arr[0][0], delimiterPattern, MicroRegex.SearchTypes.REGEX);
                
                for (int i = 0; i < arr[1].Length && i < list.Count; ++i)
                {
                    Assert.AreEqual(arr[1][i], list[i]);
                }
                
                list = MicroRegex.Match.Split(arr[0][0], delimiterPattern, MicroRegex.SearchTypes.REGEX, trim:true);
                
                for (int i = 0; i < arr[2].Length && i < list.Count; ++i)
                {
                    Assert.AreEqual(arr[2][i], list[i]);
                }
                
                list = MicroRegex.Match.Split(arr[0][0], delimiter, MicroRegex.SearchTypes.STRING);
                
                for (int i = 0; i < arr[1].Length && i < list.Count; ++i)
                {
                    Assert.AreEqual(arr[1][i], list[i]);
                }
                
                list = MicroRegex.Match.Split(arr[0][0], delimiter, MicroRegex.SearchTypes.REGEX, trim:true);
                
                for (int i = 0; i < arr[2].Length && i < list.Count; ++i)
                {
                    Assert.AreEqual(arr[2][i], list[i]);
                }
            }
        }
    }
}
