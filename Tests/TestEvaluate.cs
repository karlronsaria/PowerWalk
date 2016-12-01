/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/7/2016
 * Time: 4:05 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;

namespace PowerWalk.Tests
{
	[TestFixture]
	public class TestEvaluate
	{
		const double EPSILON = 0.00000001;
		
		readonly Int64[] testIntegers =
		{
			1, 23, 13, 53415, 527, 488, 0, 2, 14,
			-1 -23, -13, -53415, -527, -448, -0, -2, -14
		};
		
		readonly Double[] testDoubles =
		{
			1.337, 23.42, 13.008, 53415.991, 527.24, 488.19, 0.0, 2.019, 14.14,
			-1.337 -23.42, -13.008, -53415.991, -527.24, -448.19, -0.0, -2.019, -14.14
		};
		
		readonly Int64[][] testIntegerPairs =
		{
			new Int64[]{1, 2},
			new Int64[]{3, 4},
			new Int64[]{0, 15},
			new Int64[]{-15, 0},
			new Int64[]{-10, -10},
			new Int64[]{22, 300},
			new Int64[]{34, 34}
		};
		
		readonly Double[][] testDoublePairs =
		{
			new Double[]{1.1234, 2.4324},
			new Double[]{33.221, 14.778},
			new Double[]{0, 15.19},
			new Double[]{-15.19, 0},
			new Double[]{-10.0007, -10.1},
			new Double[]{22.0003, 312.19},
			new Double[]{0.0007, 1238.2},
			new double[]{12.34, 12.34}
		};
		
		readonly string[][] testWordPairs =
		{
			new string[]{"Hello ", "World"},
			new string[]{"sample", ""},
			new string[]{"",       "text"},
			new string[]{"what", "what"}
		};
		
		readonly Boolean[][] deMorganTable =
		{
			new bool[]{true,  true},
			new bool[]{true,  false},
			new bool[]{false, true},
			new bool[]{false, false}
		};
		
		[Test]
		public void TestBinaryEvaluate()
		{
		    foreach (Int64 item in testIntegers)
		        Assert.AreEqual(-item, (Int64) Interpreter.Sequence.Evaluate("-", item));
		    
		    foreach (Double item in testDoubles)
		        Assert.AreEqual(-item, (Double) Interpreter.Sequence.Evaluate("-", item));
		    
		    Assert.AreEqual(false, (Boolean) Interpreter.Sequence.Evaluate("not", true));
		    Assert.AreEqual(true,  (Boolean) Interpreter.Sequence.Evaluate("not", false));
		}
		
		[Test]
		public void TestTernaryEvaluate()
		{
			foreach (Int64[] arr in testIntegerPairs)
			{
				Assert.AreEqual(arr[0] + arr[1], (Int64) Interpreter.Sequence.Evaluate(arr[0], "plus", arr[1]));
				Assert.AreEqual(arr[1] + arr[0], (Int64) Interpreter.Sequence.Evaluate(arr[1], "plus", arr[0]));
			}
			
			foreach (Int64[] arr in testIntegerPairs)
			{
				Assert.AreEqual(arr[0] - arr[1], (Int64) Interpreter.Sequence.Evaluate(arr[0], "minus", arr[1]));
				Assert.AreEqual(arr[1] - arr[0], (Int64) Interpreter.Sequence.Evaluate(arr[1], "minus", arr[0]));
			}
			
			foreach (Int64[] arr in testIntegerPairs)
			{
				Assert.AreEqual(arr[0] * arr[1], (Int64) Interpreter.Sequence.Evaluate(arr[0], "by", arr[1]));
				Assert.AreEqual(arr[1] * arr[0], (Int64) Interpreter.Sequence.Evaluate(arr[1], "by", arr[0]));
			}
			
			foreach (Int64[] arr in testIntegerPairs)
			{
				if (arr[1] != 0) Assert.AreEqual(arr[0] / arr[1], (Int64) Interpreter.Sequence.Evaluate(arr[0], "over", arr[1]));
				if (arr[0] != 0) Assert.AreEqual(arr[1] / arr[0], (Int64) Interpreter.Sequence.Evaluate(arr[1], "over", arr[0]));
			}
			
			foreach (Double[] arr in testDoublePairs)
			{
				Assert.AreEqual(arr[0] + arr[1], (Double) Interpreter.Sequence.Evaluate(arr[0], "plus", arr[1]));
				Assert.AreEqual(arr[1] + arr[0], (Double) Interpreter.Sequence.Evaluate(arr[1], "plus", arr[0]));
			}
			
			foreach (Double[] arr in testDoublePairs)
			{
				Assert.AreEqual(arr[0] - arr[1], (Double) Interpreter.Sequence.Evaluate(arr[0], "minus", arr[1]));
				Assert.AreEqual(arr[1] - arr[0], (Double) Interpreter.Sequence.Evaluate(arr[1], "minus", arr[0]));
			}
			
			foreach (Double[] arr in testDoublePairs)
			{
				Assert.AreEqual(arr[0] * arr[1], (Double) Interpreter.Sequence.Evaluate(arr[0], "by", arr[1]));
				Assert.AreEqual(arr[1] * arr[0], (Double) Interpreter.Sequence.Evaluate(arr[1], "by", arr[0]));
			}
			
			foreach (Double[] arr in testDoublePairs)
			{
				if (Math.Abs(arr[1]) > EPSILON)
					Assert.AreEqual(arr[0] / arr[1], (Double)Interpreter.Sequence.Evaluate(arr[0], "over", arr[1]));
				if (Math.Abs(arr[0]) > EPSILON)
					Assert.AreEqual(arr[1] / arr[0], (Double)Interpreter.Sequence.Evaluate(arr[1], "over", arr[0]));
			}
			
			foreach (string[] arr in testWordPairs)
			{
				Assert.AreEqual(arr[0] + arr[1], (string) Interpreter.Sequence.Evaluate(arr[0], "plus", arr[1]));
				Assert.AreEqual(arr[1] + arr[0], (string) Interpreter.Sequence.Evaluate(arr[1], "plus", arr[0]));
			}
			
			foreach (bool[] arr in deMorganTable)
			{
				Assert.AreEqual(arr[0] || arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], "or", arr[1]));
				Assert.AreEqual(arr[1] || arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], "or", arr[0]));
			}
			
			foreach (bool[] arr in deMorganTable)
			{
				Assert.AreEqual(arr[0] && arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], "and", arr[1]));
				Assert.AreEqual(arr[1] && arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], "and", arr[0]));
			}
		
			foreach (Int64[] arr in testIntegerPairs)
			{
				Assert.AreEqual(arr[0] > arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], ">", arr[1]));
				Assert.AreEqual(arr[1] > arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], ">", arr[0]));
			}
			
			foreach (Double[] arr in testDoublePairs)
			{
				Assert.AreEqual(arr[0] > arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], ">", arr[1]));
				Assert.AreEqual(arr[1] > arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], ">", arr[0]));
			}
			
			foreach (string[] arr in testWordPairs)
			{
				Assert.AreEqual(String.Compare(arr[0], arr[1], StringComparison.Ordinal) > 0, (Boolean) Interpreter.Sequence.Evaluate(arr[0], ">", arr[1]));
				Assert.AreEqual(String.Compare(arr[1], arr[0], StringComparison.Ordinal) > 0, (Boolean) Interpreter.Sequence.Evaluate(arr[1], ">", arr[0]));
			}
		
			foreach (Int64[] arr in testIntegerPairs)
			{
				Assert.AreEqual(arr[0] < arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], "<", arr[1]));
				Assert.AreEqual(arr[1] < arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], "<", arr[0]));
			}
			
			foreach (Double[] arr in testDoublePairs)
			{
				Assert.AreEqual(arr[0] < arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], "<", arr[1]));
				Assert.AreEqual(arr[1] < arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], "<", arr[0]));
			}
			
			foreach (string[] arr in testWordPairs)
			{
				Assert.AreEqual(String.Compare(arr[0], arr[1], StringComparison.Ordinal) < 0, (Boolean) Interpreter.Sequence.Evaluate(arr[0], "<", arr[1]));
				Assert.AreEqual(String.Compare(arr[1], arr[0], StringComparison.Ordinal) < 0, (Boolean) Interpreter.Sequence.Evaluate(arr[1], "<", arr[0]));
			}
		
			foreach (Int64[] arr in testIntegerPairs)
			{
				Assert.AreEqual(arr[0] <= arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], "<=", arr[1]));
				Assert.AreEqual(arr[1] <= arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], "<=", arr[0]));
			}
			
			foreach (Double[] arr in testDoublePairs)
			{
				Assert.AreEqual(arr[0] <= arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], "<=", arr[1]));
				Assert.AreEqual(arr[1] <= arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], "<=", arr[0]));
			}
			
			foreach (string[] arr in testWordPairs)
			{
				Assert.AreEqual(String.Compare(arr[0], arr[1], StringComparison.Ordinal) <= 0, (Boolean) Interpreter.Sequence.Evaluate(arr[0], "<=", arr[1]));
				Assert.AreEqual(String.Compare(arr[1], arr[0], StringComparison.Ordinal) <= 0, (Boolean) Interpreter.Sequence.Evaluate(arr[1], "<=", arr[0]));
			}
		
			foreach (Int64[] arr in testIntegerPairs)
			{
				Assert.AreEqual(arr[0] >= arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], ">=", arr[1]));
				Assert.AreEqual(arr[1] >= arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], ">=", arr[0]));
			}
			
			foreach (Double[] arr in testDoublePairs)
			{
				Assert.AreEqual(arr[0] >= arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], ">=", arr[1]));
				Assert.AreEqual(arr[1] >= arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], ">=", arr[0]));
			}
			
			foreach (string[] arr in testWordPairs)
			{
				Assert.AreEqual(String.Compare(arr[0], arr[1], StringComparison.Ordinal) >= 0, (Boolean) Interpreter.Sequence.Evaluate(arr[0], ">=", arr[1]));
				Assert.AreEqual(String.Compare(arr[1], arr[0], StringComparison.Ordinal) >= 0, (Boolean) Interpreter.Sequence.Evaluate(arr[1], ">=", arr[0]));
			}
		
			foreach (Int64[] arr in testIntegerPairs)
			{
				Assert.AreEqual(arr[0] == arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], "equal", arr[1]));
				Assert.AreEqual(arr[1] == arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], "equal", arr[0]));
			}
			
			foreach (Double[] arr in testDoublePairs)
			{
				Assert.AreEqual(Math.Abs(arr[0] - arr[1]) < EPSILON, (Boolean)Interpreter.Sequence.Evaluate(arr[0], "equal", arr[1]));
				Assert.AreEqual(Math.Abs(arr[1] - arr[0]) < EPSILON, (Boolean)Interpreter.Sequence.Evaluate(arr[1], "equal", arr[0]));
			}
			
			foreach (string[] arr in testWordPairs)
			{
				Assert.AreEqual(arr[0] == arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], "equal", arr[1]));
				Assert.AreEqual(arr[1] == arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], "equal", arr[0]));
			}
		
			foreach (Int64[] arr in testIntegerPairs)
			{
				Assert.AreEqual(arr[0] != arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], "not equal", arr[1]));
				Assert.AreEqual(arr[1] != arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], "not equal", arr[0]));
			}
			
			foreach (Double[] arr in testDoublePairs)
			{
				Assert.AreEqual(Math.Abs(arr[0] - arr[1]) > EPSILON, (Boolean)Interpreter.Sequence.Evaluate(arr[0], "not equal", arr[1]));
				Assert.AreEqual(Math.Abs(arr[1] - arr[0]) > EPSILON, (Boolean)Interpreter.Sequence.Evaluate(arr[1], "not equal", arr[0]));
			}
			
			foreach (string[] arr in testWordPairs)
			{
				Assert.AreEqual(arr[0] != arr[1], (Boolean) Interpreter.Sequence.Evaluate(arr[0], "not equal", arr[1]));
				Assert.AreEqual(arr[1] != arr[0], (Boolean) Interpreter.Sequence.Evaluate(arr[1], "not equal", arr[0]));
			}
		}
	}
}
