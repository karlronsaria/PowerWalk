/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/9/2016
 * Time: 1:21 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;

namespace PowerWalk.Tests
{
    [TestFixture]
    public class TestExecutionFunctions
    {
        public string[] testLines =
        {
            "for each letter in text",
    		"    if letter match whitespace",
    		"        if LengthOf(line) plus LengthOf(word) > len",
    		"            add line plus newline to result",
    		"            clear line",
    		"        add word plus ' ' to line",
    		"        clear word",
    		"    add letter to word"
        };
        
        public int[] resultLevels =
        {
            0, 1, 2, 3, 3, 2, 2, 1
        };
        
        public string[] resultVerbs =
        {
            "for", "if", "if", "add", "clear", "add", "clear", "add"
        };
        
        public string[] resultComplements =
        {
            "each letter in text",
    		"letter match whitespace",
    		"LengthOf(line) plus LengthOf(word) > len",
    		"line plus newline to result",
    		"line",
    		"word plus ' ' to line",
    		"word",
    		"letter to word"
        };
        
        [Test]
        public void TestSplitAndLevel()
        {
            var main = new Interpreter.Sequence();
            
            int lvl;
            string verb = "", complement = "";
            
            for (int i = 0; i < testLines.Length; ++i)
            {
                lvl = Interpreter.Sequence.GetLevel(testLines[i]);
                Verbs.SplitSentence(testLines[i], ref verb, ref complement);
                
                main.MoveScope(lvl);
                
                Assert.AreEqual(resultLevels[i], lvl);
                Assert.AreEqual(resultLevels[i], main.Levels);
                Assert.AreEqual(resultVerbs[i], verb);
                Assert.AreEqual(resultComplements[i], complement);
            }
        }
    }
}
