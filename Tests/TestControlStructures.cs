/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/10/2016
 * Time: 4:40 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace PowerWalk.Tests
{
    [TestFixture]
    public class TestControlStructures
    {
        public static readonly string testIfProcess =
            
            "    if var1 mod 3 = 0\n"             +
            "        init var2 := 2\n"            +
            "        add var1 <- var2 by var1\n"  +
            "    else if var1 mod 5 = 0\n"        +
            "        init var3 := 3\n"            +
            "        add var1 <- var3 by var1\n"  +
            "    else if var1 mod 7 = 0\n"        +
            "        init var4 := 4\n"            +
            "        add var1 <- var4 by var1\n"  +
            "    else if var1 mod 11 = 0\n"       +
            "        init var5 := 5\n"            +
            "        add var1 <- var5 by var1\n"  +
            "    writeline \"Result: \" + var1";
        
        public static readonly string[] testIfDeclarations =
        {
            "    declare var1 :=   9",
            "    declare var1 :=  25",
            "    declare var1 :=  49",
            "    declare var1 := 121"
        };
        
        public static readonly Int64[] resultIfRuns =
        {
            27L, 100L, 245L, 726L
        };
        
        public static readonly string testWhileProcess =
            
            "    writeline \"\"\n"                     +
            "    writeline \"\"\n"                     +
            "    declare counter := 0\n"               +
            "    while var1 >= 0\n"                    +
            "        init var2 := 2\n"                 +
            "        increment counter\n"              +
            "        assign var1 := var1 minus var2\n" +
            "    writeline \"Iterations: \" + counter";
        
        public static readonly string[] testWhileDeclarations =
        {
            "    declare var1 :=   9",
            "    declare var1 :=  25",
            "    declare var1 :=  49",
            "    declare var1 := 121"
        };
        
        public static readonly Int64[] resultWhileRuns =
        {
            5L, 13L, 25L, 61L
        };
        
        public static readonly string[][] testForStatements =
        {
            new string[]
            {
                "    init sum := 0  \n",
                "    for each n in [1, 10]  \n",
                "        assign sum := sum + n  \n"
            },
            
            new string[]
            {
                "    init sum := 0  \n",
                "    for each var in [2, 17)  \n",
                "        assign sum := sum + var  \n"
            },
            
            new string[]
            {
                "    init sum := 0.0  \n",
                "    for every 0.1 f in [5.0, 0.0]  \n",
                "        assign sum := sum + f  \n"
            },
            
            new string[]
            {
                "    init sum := 0  \n",
                "    for every (12/4) number in {2, 14, 3, 15, 17, 21, 50, 89}  \n",
                "        assign sum := sum + number  \n"
            },
            
            new string[]
            {
                "    init sum := \"\"  \n",
                "    for every 2 character in {'a', 'b', 'c', 'd', 'e', 'f'}  \n",
                "        assign sum := sum + character  \n"
            },
            
            new string[]
            {
                "    init sum := \"\"  \n",
                "    for each character in ['z', 'b')  \n",
                "        assign sum := sum + character  \n"
            },
            
            new string[]
            {
                "    init sum := \"\"  \n",
                "    for every 1 str in { \"It's \",   \"all \",\"I \", \"have to bring \",   \"today\"  }",
                "        assign sum := sum + str"
            }
        };
        
        public static readonly object[] resultForRuns =
        {
            55L, 135L, 127.5, 67L, "ace", "zyxwvutsrqponmlkjihgfedc", "It's all I have to bring today"
        };
        
        [Test]
        public void TestIf()
        {
            var queue = new List<string>();
            
            queue.AddRange(testIfProcess.Split('\n'));
            
            for (int i = 0; i < testIfDeclarations.Length; ++i)
            {
                var main      = new Interpreter.Sequence();
                var testQueue = new List<string>();
                
                testQueue.Add(testIfDeclarations[i]);
                testQueue.AddRange(queue);
                
                int line = 0;
                
                while (line < testQueue.Count)
                {
                    line = main.Execute(testQueue, line);
                }
                
                Assert.AreEqual(resultIfRuns[i], main.GetNamedValue("var1"));
            }
        }
        
        [Test]
        public void TestWhile()
        {
            var queue = new List<string>();
            
            queue.AddRange(testWhileProcess.Split('\n'));
            
            for (int i = 0; i < testWhileDeclarations.Length; ++i)
            {
                var main      = new Interpreter.Sequence();
                var testQueue = new List<string>();
                
                testQueue.Add(testWhileDeclarations[i]);
                testQueue.AddRange(queue);
                
                int line = 0;
                
                while (line < testQueue.Count)
                {
                    line = main.Execute(testQueue, line);
                }
                
                Assert.AreEqual(resultWhileRuns[i], main.GetNamedValue("counter"));
            }
        }
        
        [Test]
        public void TestFor()
        {
            for (int i = 0; i < testForStatements.Length; ++i)
            {
                var main  = new Interpreter.Sequence();
                var queue = new List<string>();
                
                queue.AddRange(testForStatements[i]);
                
                int line = 0;
                
                while (line < queue.Count)
                {
                    line = main.Execute(queue, line);
                }
                
                switch (resultForRuns[i].GetType().Name)
                {
                    case "Double" : Assert.AreEqual((Double) resultForRuns[i], (Double) main.GetNamedValue("sum"), 0.05);
                                    break;
                    default       : Assert.AreEqual(resultForRuns[i], main.GetNamedValue("sum"));
                                    break;
                }
            }
        }
    }
}
