/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 9/29/2016
 * Time: 8:43 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Collections.Generic;

namespace PowerWalk
{
    /// <summary>
    /// Description of Transcript.
    /// </summary>
    
    public static class Transcript
    {
        private static readonly Dictionary<string, List<string>> _table = new Dictionary<string, List<string>>();
        
        public static Dictionary<string, List<string>> Table
        {
            get { return _table; }
        }
        
        private static bool Push(string path, List<string> page)
        {
            if (!_table.ContainsKey(path))
            {
                _table.Add(path, page);
                
                return Definition.Push(path, page);
            }
            
            return false;
        }
        
        public static bool PushFile(string path)
        {
            if (path.EndsWith(Preferences.PAGE_EXTENSION, StringComparison.Ordinal))
            {
                var newPage = new List<string>();
                
                newPage.AddRange(System.IO.File.ReadAllLines(path));
                
                CleanPage(ref newPage);
                
                return Push(path, newPage);
            }
            
            return false;
        }
        
//        public static bool Push(string path)
//        {
//            bool isTranscriptPage = false;
//            
//            foreach (var file in System.IO.Directory.GetFiles(path))
//            {
//                if (!isTranscriptPage && PushFile(file))
//                {
//                    isTranscriptPage = true;
//                }
//                else
//                {
//                    PushFile(file);
//                }
//            }
//            
//            return isTranscriptPage;
//        }
        
        public static void CleanPage(ref List<string> lines)
        {
            for (int index = 0; index < lines.Count; ++index)
            {
                RemoveAnnotation(ref lines, index);
                
                ContinueNextLine(ref lines, ref index);
            }
        }
        
        public static void RemoveAnnotation(ref List<string> lines, int index)
        {
            MicroRegex.Match match = 
                
                MicroRegex.Match.NextMatch
                (
                    lines[index], Operators.comment,
                    MicroRegex.SearchTypes.STRING,
                    MicroRegex.MatchTypes.NonLiteralSequence,
                    MicroRegex.Match.ANYQUOTE
                );
            
            if (match.Success)
            {
                lines[index] = lines[index].Substring(0, match.Index);
            }
        }
        
		public static void ContinueNextLine(ref List<string> lines, ref int index)
		{
		    MicroRegex.Match match = MicroRegex.Match.LastMatch
		                             (
		                                 lines[index],
		                                 Operators.linecontinue,
		                                 MicroRegex.SearchTypes.STRING,
		                                 MicroRegex.MatchTypes.NonLiteralSequence
		                             );
		    
		    int count = 1;
            
            while (match.Success)
            {
                if (String.IsNullOrWhiteSpace(lines[index].Substring(match.Index + match.Length)))
                {
                    if (index + count < lines.Count)
                    {
                        lines[index] = lines[index].Substring(0, match.Index);
                        
                        if (!lines[index].EndsWith(" ", StringComparison.Ordinal))
                        {
                            lines[index] += " ";
                        }
                        
                        lines[index] += lines[index + count].TrimStart();
                        
                        lines[index + count] = "";
                        
                        count = count + 1;
                    }
                    else
                    {
                        throw new Exception("Line " + (index + count) + ": Syntax Error: Line continuation not followed by another line.");
                    }
                }
                else
                {
                    lines[index] = lines[index].Remove(match.Index, match.Length);
                }
                
			    match = MicroRegex.Match.LastMatch
			            (
			                lines[index],
			                Operators.linecontinue,
			                MicroRegex.SearchTypes.STRING,
			                MicroRegex.MatchTypes.NonLiteralSequence
			            );
            }
		}
		
		public static string TableToString()
		{
		    return Algorithms.Collections.ToString(_table);
		}
		
		public static void Clear()
		{
		    _table.Clear();
		}
    }
}



















