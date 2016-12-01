/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/7/2016
 * Time: 6:46 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;

namespace PowerWalk.Tests
{
    [TestFixture]
    public class TestSmartMatch
    {
        readonly string[][] testWordPairs =
        {
            new string[]{ "asdf ()[]{}\"\"", "asdf ", "()", "[]", "{}", "\"\"" },
            new string[]{ "\"\"{}[]() fdsa", "\"\"", "{}", "[]", "()", " fdsa" },
            new string[]{ "(1234f ) cndkl988&* [a;dkjvh] (&*LKNKJKH&] 20a.6%",
                          "(1234f )", " cndkl988&* ", "[a;dkjvh]", " ", "(&*LKNKJKH&]", " 20a.6%" }
        };
        
        const string line    = "( text ) text [ text )text ";
        const string pattern = "(?<=\\s)t.*(?=\\s)";
        
        [Test]
        public void TestNextOuterGroup()
        {
            int index;
            MicroRegex.Match match;
            
            foreach (string[] arr in testWordPairs)
            {
                index = 0;
                
                for (int i = 1; i < arr.Length; ++i)
                {
                    match = MicroRegex.Match.NextOuterGroup(arr[0], index);
                    
                    Assert.AreEqual(arr[i], match.Value);
                    
                    index = match.Index + match.Length;
                }
            }
        }
        
        [Test]
        public void TestLastOuterGroup()
        {
            int index;
            MicroRegex.Match match;
            
            foreach (string[] arr in testWordPairs)
            {
                index = arr[0].Length - 1;
                
                for (int i = arr.Length - 1; i > 0; --i)
                {
                    match = MicroRegex.Match.LastOuterGroup(arr[0], index);
                    
                    Assert.AreEqual(arr[i], match.Value);
                    
                    index = match.Index - 1;
                }
            }
        }
        
        [Test]
        public void TestNextMatch()
        {
            Assert.AreEqual(9, MicroRegex.Match.NextMatch(line, pattern, MicroRegex.SearchTypes.REGEX).Index);
        }
        
        [Test]
        public void TestLastMatch()
        {
            Assert.AreEqual(9, MicroRegex.Match.LastMatch(line, pattern, MicroRegex.SearchTypes.REGEX).Index);
        }
    }
}
