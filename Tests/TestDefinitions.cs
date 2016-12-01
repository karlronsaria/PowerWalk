/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 9/27/2016
 * Time: 12:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace PowerWalk.Tests
{
    [TestFixture]
    public class TestDefinitions
    {
        static readonly string[][] defHeadings = new string[][]
        {
            new string[]
            {
                "def DefinitionName(in first : String, out secnd : Int64): String",
                "    return \"Unfinished.\""
            },
            
            new string[]
            {
                "define SecondDef : Int64\n",
                "    in var1 : String := \"What.\"",
                "    out var2 : String",
                "    in var3, const var4: Int64, readonly var5 :Float :=0.0,",
                "       var6 := \"huh\"",
                "    return \"Unfinished.\""
            },
            
            new string[]
            {
                "def What() : Float",
                "    return \"Unfinished.\""
            }
        };
        
        static readonly object[] defNames = new object[]
        {
            "DefinitionName",
            "SecondDef",
            "What"
        };
        
        static readonly object[] defReturnTypes = new string[]
        {
            "String",
            "Int64",
            "Float"
        };
        
        static readonly object[][][] defParameters = new object[][][]
        {
            new object[][]
            {
                new object[]
                {
                    "first", true, true, false, "String", ""
                },
                
                new object[]
                {
                    "secnd", true, true, true, "Int64", ""
                }
            },
            
            new object[][]
            {
                new object[]{"var1", true, true, false, "String", "\"What.\""},
                new object[]{"var2", true, true, true, "String", ""},
                new object[]{"var3", true, true, false, "Object", ""},
                new object[]{"var4", true, false, false, "Int64", ""},
                new object[]{"var5", true, false, false, "Float", "0.0"},
                new object[]{"var6", true, true, false, "Object", "\"huh\""}
            },
            
            new object[][]{}
        };
        
        [Test]
        public void TestParseHeading()
        {
            List<string> headings;
            string name = "", returnType = "";
            int line;
            
            for (int i = 0; i < defHeadings.Length; ++i)
            {
                headings = new List<string>();
                headings.AddRange(defHeadings[i]);
                
                line = 0;
                var typenames = Definition.ParseHeading(defHeadings[i], ref line, ref name, ref returnType);
                line = 0;
                var parameters = Definition.ParseHeading(defHeadings[i], ref line);
                
                Assert.AreEqual(defNames[i], name);
                Assert.AreEqual(defReturnTypes[i], returnType);
                
                for (int j = 0; j < defParameters[i].Length; ++j)
                {
                    Assert.AreEqual(defParameters[i][j][0], parameters[j].key);
                    Assert.AreEqual(defParameters[i][j][1], parameters[j].readable);
                    Assert.AreEqual(defParameters[i][j][2], parameters[j].writeable);
                    Assert.AreEqual(defParameters[i][j][3], parameters[j].referential);
                    Assert.AreEqual(defParameters[i][j][4], parameters[j].typename);
                    Assert.AreEqual(defParameters[i][j][5], parameters[j].expression);
                    
                    Assert.AreEqual(defParameters[i][j][4], typenames[j]);
                }
            }
        }
    }
}
