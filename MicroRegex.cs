/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/7/2016
 * Time: 7:56 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MicroRegex
{
	public enum SearchTypes { STRING, REGEX };
	public enum MatchTypes  { NonLiteralSequence, NonGroup, WordOnly };
    
    public class Match
    {
        public const string STRINGQUOTE = "\"";
        public const string CHARQUOTE   = "'";
        public const string ANYQUOTE    = "\"'";
        
		const string opener = "([{";  // NOT SUPPORTED: <
		const string closer = ")]}";  // NOT SUPPORTED: >
		
        private string value  = "";
        private Int32  index  = -1;
        private Int32  length =  0;
        
        private GroupCollection groups = null;

        private Match() {}

        private Match(string str, Int32 i, Int32 len, GroupCollection coll = null)
        {
            value  = str;
            index  = i;
            length = len;
            groups = coll;
        }

        public string Value
        {
            get { return value; }
        }

        public Boolean Success
        {
			get { return index >= 0; }
        }

        public Int32 Index
        {
            get { return index; }
        }

        public Int32 Length
        {
            get { return length; }
        }
        
        public GroupCollection Groups
        {
            get { return groups; }
        }
		
        private static Match Fail() { return new Match(); }
        
		private delegate int CodeStub(int start, int len);
		
		private static int SubstringForward(int start, int len)
		{
		    return start;
		}
		
		private static int SubstringBackward(int start, int len)
		{
		    return start - len + 1;
		}
		
		private static int StopIncrementingAt(int start, int len)
		{
		    return len - start;
		}
		
		private static int StopDecrementingAt(int start, int len)
		{
		    return start + 1;
		}
		
        private static int NextLiteralSequenceLength(string line, int start, int end, int shiftFactor, string seqdelim)
        {
            int count;
            
            for(count = 1; count < end && !seqdelim.Contains(line[start + shiftFactor * count]); ++count);
            
            return count + 1;
        }
        
		private static int NextGroupLength(string line, int start, int end, int shiftFactor, string pushSymbols, string popSymbols)
		{
			var groupStack = new Stack<int>();
			
			groupStack.Push(pushSymbols.IndexOf(line[start]));

			int count = 1;
			
			while (groupStack.Any() && count < end)
			{
				if (pushSymbols.Contains(line[start + shiftFactor * count]))
				{
					groupStack.Push(pushSymbols.IndexOf(line[start + shiftFactor * count]));
				}
				else if (popSymbols.Contains(line[start + shiftFactor * count]))
				{
					groupStack.Pop();
				}
				
				count = count + 1;
			}
			
			return count;
		}
		
		private static int NextWordLength( string line, int start, int end, int shiftFactor,
		                                   string groupLimits = opener + closer + "\"" )
		{
		    int count;
		    
		    for (count = 1; count < end && !groupLimits.Contains(line[start + shiftFactor * count]); ++count) {}
		    
		    return count;
		}
		
		private static Match NextOuterGroup( string line, int start, int shiftFactor,
		                                     string pushSymbols, string popSymbols,
		                                     CodeStub StopAt, CodeStub NextStart,
		                                     string seqdelim = STRINGQUOTE )
		{
		    if (String.IsNullOrEmpty(line)) return Fail();
		    
			int len;
			
			if (seqdelim.Contains(line[start]))
			{
			    len = NextLiteralSequenceLength(line, start, StopAt(start, line.Length), shiftFactor, seqdelim);
			}
			else if (pushSymbols.Contains(line[start]))
			{
				len = NextGroupLength(line, start, StopAt(start, line.Length), shiftFactor, pushSymbols, popSymbols);
			}
			else
			{
				len = NextWordLength(line, start, StopAt(start, line.Length), shiftFactor);
			}
			
			return new Match
			(
			    line.Substring(NextStart(start, len), len), NextStart(start, len), len
			);
		}
		
		public static Match NextOuterGroup(string line, int start = 0, string seqdelim = STRINGQUOTE)
		{
		    return NextOuterGroup(line, start, 1, opener, closer, StopIncrementingAt, SubstringForward, seqdelim);
		}
		
		public static Match LastOuterGroup(string line, int start)
		{
		    return NextOuterGroup(line, start, -1, closer, opener, StopDecrementingAt, SubstringBackward);
		}
		
		public static Match LastOuterGroup(string line)
		{
		    return LastOuterGroup(line, line.Length - 1);
		}
		
		private static Match MatchGroup( string line, string pattern, ref int tracker, ref int len, SearchTypes search,
		                                 int shiftFactor, CodeStub NextStart, RegexOptions orientation )
		{
			int index = -1;
			string value = "";
			GroupCollection groups = null;
			
			switch (search)
			{
				case SearchTypes.STRING:
				{
					index = line.Substring(NextStart(tracker, len), len).IndexOf(pattern, StringComparison.Ordinal);
					
					if (index >= 0)
					{
						value = pattern;
					}
					break;
				}
				case SearchTypes.REGEX:
				{
			        System.Text.RegularExpressions.Match match =
			            
			            Regex.Match(line.Substring(NextStart(tracker, len), len), pattern, orientation | RegexOptions.Multiline);
					
					if (match.Success)
					{
						value  = match.Value;
						index  = match.Index;
						groups = match.Groups;
					}
					break;
				}
				default:
				{
					throw new ArgumentException();
				}
			}
			
			if (index >= 0)
			{
			    return new Match(value, NextStart(tracker, len) + index, value.Length, groups);
			}
			
			tracker += shiftFactor * len;
			
			return Fail();
		}
		
		private static Match NextMatchNonLiteralSequence( string line, string pattern, int start, SearchTypes search,
		                                                  int shiftFactor, CodeStub StopAt, CodeStub NextStart,
		                                                  RegexOptions orientation,
		                                                  string seqdelim = STRINGQUOTE )
		{
		    Match match = Fail();
		    
		    int tracker = start;
		    int len;
		    
		    while (!match.Success && tracker >= 0 && tracker < line.Length)
		    {
		        if (seqdelim.Contains(line[tracker]))  // HERE'S A PROBLEM
		        {
		            tracker += shiftFactor * NextLiteralSequenceLength(line, tracker, StopAt(tracker, line.Length), shiftFactor, seqdelim);
		            
		            continue;
		        }
		        
				len = NextWordLength(line, tracker, StopAt(tracker, line.Length), shiftFactor, seqdelim);
				
				match = MatchGroup(line, pattern, ref tracker, ref len, search, shiftFactor, NextStart, orientation);
		    }
		    
		    return match;
		}
		
		private static Match NextMatchNonGroup( string line, string pattern, int start, SearchTypes search,
		                                        int shiftFactor, CodeStub StopAt, CodeStub NextStart,
		                                        string pushSymbols, string popSymbols,
		                                        RegexOptions orientation )
		{
		    Match match = Fail();
		    
		    int tracker = start;
		    int len;
		    
		    while (!match.Success && tracker >= 0 && tracker < line.Length)
		    {
				if(pushSymbols.Contains(line[tracker]))
				{
					tracker += shiftFactor * NextGroupLength(line, tracker, StopAt(tracker, line.Length), shiftFactor, pushSymbols, popSymbols);
					
					continue;
				}
				
				len = NextWordLength(line, tracker, StopAt(tracker, line.Length), shiftFactor, pushSymbols + popSymbols);
				
				match = MatchGroup(line, pattern, ref tracker, ref len, search, shiftFactor, NextStart, orientation);
		    }
		    
		    return match;
		}
		
		private static Match NextMatchWordOnly( string line, string pattern, int start, SearchTypes search,
                                                int shiftFactor, CodeStub StopAt, CodeStub NextStart,
                                                string pushSymbols, string popSymbols,
                                                RegexOptions orientation, string seqdelim = STRINGQUOTE )
		{
		    Match match = Fail();
		    
		    int tracker = start;
		    int len;
		    
		    while (!match.Success && tracker >= 0 && tracker < line.Length)
		    {
		        if (seqdelim.Contains(line[tracker]))
		        {
		            tracker += shiftFactor * NextLiteralSequenceLength(line, tracker, StopAt(tracker, line.Length), shiftFactor, seqdelim);
		            
		            continue;
		        }
		        
				if(pushSymbols.Contains(line[tracker]))
				{
					tracker += shiftFactor * NextGroupLength(line, tracker, StopAt(tracker, line.Length), shiftFactor, pushSymbols, popSymbols);
					
					continue;
				}
				
				len = NextWordLength(line, tracker, StopAt(tracker, line.Length), shiftFactor, pushSymbols + popSymbols + seqdelim);
				
				match = MatchGroup(line, pattern, ref tracker, ref len, search, shiftFactor, NextStart, orientation);
		    }
		    
		    return match;
		}
		
		public static Match NextMatchNonLiteralSequence(string line, string pattern, int start, SearchTypes search, string seqdelim = STRINGQUOTE)
		{
		    return NextMatchNonLiteralSequence(line, pattern, start, search, 1, StopIncrementingAt, SubstringForward, RegexOptions.None, seqdelim);
		}
		
		public static Match NextMatchNonGroup(string line, string pattern, int start, SearchTypes search)
		{
		    return NextMatchNonGroup(line, pattern, start, search, 1, StopIncrementingAt, SubstringForward, opener, closer, RegexOptions.None);
		}
		
		public static Match NextMatchWordOnly(string line, string pattern, int start, SearchTypes search, string seqdelim = STRINGQUOTE)
		{
		    return NextMatchWordOnly(line, pattern, start, search, 1, StopIncrementingAt, SubstringForward, opener, closer, RegexOptions.None, seqdelim);
		}
		
		public static Match LastMatchNonLiteralSequence(string line, string pattern, int start, SearchTypes search, string seqdelim = STRINGQUOTE)
		{
		    return NextMatchNonLiteralSequence(line, pattern, start, search, -1, StopDecrementingAt, SubstringBackward, RegexOptions.RightToLeft, seqdelim);
		}
		
		public static Match LastMatchNonGroup(string line, string pattern, int start, SearchTypes search)
		{
		    return NextMatchNonGroup(line, pattern, start, search, -1, StopDecrementingAt, SubstringBackward, closer, opener, RegexOptions.RightToLeft);
		}
		
		public static Match LastMatchWordOnly(string line, string pattern, int start, SearchTypes search, string seqdelim = STRINGQUOTE)
		{
		    return NextMatchWordOnly(line, pattern, start, search, -1, StopDecrementingAt, SubstringBackward, closer, opener, RegexOptions.RightToLeft, seqdelim);
		}
		
		
//		public static Match NextMatchNonLiteralSequence(string line, string pattern, SearchTypes search = SearchTypes.REGEX, string seqdelim = "\"")
//		{
//		    return NextMatchNonLiteralSequence(line, pattern, 0, search, seqdelim);
//		}
//		
//		public static Match LastMatchNonLiteralSequence(string line, string pattern, SearchTypes search = SearchTypes.REGEX, string seqdelim = "\"")
//		{
//		    return LastMatchNonLiteralSequence(line, pattern, line.Length - 1, search, seqdelim);
//		}
		
		
		public static Match NextMatch(string line, string pattern, int start, SearchTypes search, MatchTypes validMatch = MatchTypes.WordOnly, string seqdelim = STRINGQUOTE)
		{
		    switch (validMatch)
		    {
		        case MatchTypes.NonLiteralSequence:
		            return NextMatchNonLiteralSequence(line, pattern, start, search, seqdelim);
		        case MatchTypes.NonGroup:
		            return NextMatchNonGroup(line, pattern, start, search);
		        case MatchTypes.WordOnly:
		            return NextMatchWordOnly(line, pattern, start, search, seqdelim);
		        default:
		            return Fail();
		    }
		}
		
		public static Match LastMatch(string line, string pattern, int start, SearchTypes search, MatchTypes validMatch = MatchTypes.WordOnly, string seqdelim = STRINGQUOTE)
		{
		    switch (validMatch)
		    {
		        case MatchTypes.NonLiteralSequence:
		            return LastMatchNonLiteralSequence(line, pattern, start, search, seqdelim);
		        case MatchTypes.NonGroup:
		            return LastMatchNonGroup(line, pattern, start, search);
		        case MatchTypes.WordOnly:
		            return LastMatchWordOnly(line, pattern, start, search, seqdelim);
		        default:
		            return Fail();
		    }
		}
		
		
		public static Match NextMatch(string line, string pattern, SearchTypes search, MatchTypes validMatch = MatchTypes.WordOnly, string seqdelim = STRINGQUOTE)
		{
		    return NextMatch(line, pattern, 0, search, validMatch, seqdelim);
		}
		
		public static Match NextMatch(string line, string pattern, MatchTypes validMatch = MatchTypes.WordOnly, string seqdelim = STRINGQUOTE)
		{
		    return NextMatch(line, pattern, 0, SearchTypes.REGEX, validMatch, seqdelim);
		}
		
		public static Match LastMatch(string line, string pattern, SearchTypes search, MatchTypes validMatch = MatchTypes.WordOnly, string seqdelim = STRINGQUOTE)
		{
		    return LastMatch(line, pattern, line.Length - 1, search, validMatch, seqdelim);
		}
		
		public static Match LastMatch(string line, string pattern, MatchTypes validMatch = MatchTypes.WordOnly, string seqdelim = STRINGQUOTE)
		{
		    return LastMatch(line, pattern, line.Length - 1, SearchTypes.REGEX, validMatch, seqdelim);
		}
		
        public static List<Match> Matches(string line, string pattern, SearchTypes search, MatchTypes validMatch = MatchTypes.WordOnly, string seqdelim = STRINGQUOTE)
        {
            var matches = new List<Match>();

            Match match = NextMatch(line, pattern, search, validMatch, seqdelim);

            while (match.Success)
            {
                matches.Add(match);

                match = NextMatch(line, pattern, match.Index + match.Length, search, validMatch, seqdelim);
            }

            return matches;
        }
        
        public static List<Match> Matches(string line, string pattern, MatchTypes validMatch = MatchTypes.WordOnly, string seqdelim = STRINGQUOTE)
        {
            return Matches(line, pattern, SearchTypes.REGEX, validMatch, seqdelim);
        }
        
		public static List<string> Split(string line, string delimiter, SearchTypes search, MatchTypes validMatch = MatchTypes.WordOnly, string seqdelim = STRINGQUOTE, bool trim = false)
		{
			List<Match> matches = Matches(line, delimiter, search, validMatch, seqdelim);
			var words = new List<string>();
		
			if (matches.Any())
			{
				int prev = 0;

				foreach (Match match in matches)
				{
				    words.Add(trim ? line.Substring(prev, match.Index - prev).Trim()
				                   : line.Substring(prev, match.Index - prev));

					prev = match.Index + match.Length;
				}
				
				words.Add(trim ? line.Substring(prev, line.Length - prev).Trim()
				               : line.Substring(prev, line.Length - prev));
			}
			else
			{
			    words.Add(trim ? line.Trim() : line);
			}
		
			return words;
		}
		
        public static List<string> Split(string line, string delimiter, SearchTypes search, bool trim)
        {
            return Split(line, delimiter, search, MatchTypes.WordOnly, STRINGQUOTE, trim);
        }
        
//		public static List<string> Split(string line, string delimiter, SearchTypes search, MatchTypes validMatch)
//		{
//		    return Split(line, delimiter, search, validMatch);
//		}
//		
//		public static List<string> Split(string line, string delimiter, SearchTypes search)
//		{
//		    return Split(line, delimiter, search, MatchTypes.WordOnly);
//		}
//		
//		public static List<string> Split(string line, string delimiter, MatchTypes validMatch)
//		{
//		    return Split(line, delimiter, SearchTypes.STRING, validMatch);
//		}
//		
//		public static List<string> Split(string line, string delimiter, SearchTypes search, bool trim)
//		{
//		    return Split(line, delimiter, search, MatchTypes.WordOnly, trim);
//		}
//		
//		public static List<string> Split(string line, string delimiter, MatchTypes validMatch, bool trim)
//		{
//		    return Split(line, delimiter, SearchTypes.STRING, validMatch, trim);
//		}
//		
//		public static List<string> Split(string line, string delimiter, bool trim)
//		{
//		    return Split(line, delimiter, SearchTypes.STRING, MatchTypes.WordOnly, trim);
//		}
//		
//		public static List<string> Split(string line, string delimiter)
//		{
//		    return Split(line, delimiter, SearchTypes.STRING, MatchTypes.WordOnly);
//		}
    }
}
