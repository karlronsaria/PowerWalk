/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/18/2016
 * Time: 12:48 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PowerWalk
{
    /// <summary>
    /// Description of Verbs.
    /// </summary>

    public static partial class Verbs
    {
        private static string GetWord(string line, string pattern)
        {
            return Regex.Match(line, pattern).Value;
        }
        
		public static string GetVerb(string line)
		{
			return Regex.Match(line, Patterns.getverb).Value;
		}
		
		public static void SplitSentence(string line, ref string verb, ref string complement, string pattern = Patterns.getverb)
		{
		    line = line.Trim();
		    
		    verb = Verbs.GetWord(line, pattern);
		    
		    complement = verb.Length < line.Length ? line.Substring(verb.Length).Trim() : "";
		}
		
		public delegate void   Method          (Interpreter.Sequence r, string s);
		public delegate object BinaryOperation (object left, object right);
		
		public static Dictionary<string, Method> verb_table = GetVerbsTable();
        
		public static Dictionary<string, Method> GetVerbsTable()
		{
			var verbs = new Dictionary<string, Method>();
			
			foreach (string word in Operators.Verbs.declare.ToList())
				verbs.Add(word, new Method(Declare));
			
			foreach (string word in Operators.Verbs.assign.ToList())
				verbs.Add(word, new Method(AssignTo));
			
			foreach (string word in Operators.Verbs.clear.ToList())
				verbs.Add(word, new Method(Clear));
			
			foreach (string word in Operators.Verbs.release.ToList())
				verbs.Add(word, new Method(Release));
			
			foreach (string word in Operators.Verbs.increment.ToList())
				verbs.Add(word, new Method(Increment));
			
			foreach (string word in Operators.Verbs.decrement.ToList())
				verbs.Add(word, new Method(Decrement));
			
			foreach (string word in Operators.Verbs.addto.ToList())
				verbs.Add(word, new Method(AddTo));
			
			foreach (string word in Operators.Verbs.subtractfrom.ToList())
				verbs.Add(word, new Method(SubtractFrom));
			
			foreach (string word in Operators.Verbs.multiplyby.ToList())
				verbs.Add(word, new Method(MultiplyBy));
			
			foreach (string word in Operators.Verbs.divideby.ToList())
				verbs.Add(word, new Method(DivideBy));
			
			foreach (string word in Operators.Verbs.remainderfrom.ToList())
				verbs.Add(word, new Method(RemainderFrom));
			
			foreach (string word in Operators.Verbs.input.ToList())
				verbs.Add(word, new Method(Input));
			
			foreach (string word in Operators.Verbs.inputline.ToList())
				verbs.Add(word, new Method(InputLine));
			
			foreach (string word in Operators.Verbs.output.ToList())
				verbs.Add(word, new Method(Output));
			
			foreach (string word in Operators.Verbs.outputline.ToList())
				verbs.Add(word, new Method(OutputLine));
			
			foreach (string word in Operators.Verbs.error.ToList())
				verbs.Add(word, new Method(Error));
			
			foreach (string word in Operators.Verbs.errorline.ToList())
				verbs.Add(word, new Method(ErrorLine));
			
			foreach (string word in Operators.Verbs.returnvalue.ToList())
			    verbs.Add(word, new Method(ReturnValue));
			
			foreach (string word in Operators.Verbs.callprocess.ToList())
			    verbs.Add(word, new Method(CallProcess));
			
			return verbs;
		}
		
		public static void SpecifyPermissions(string op, ref bool read, ref bool write)
		{
		    if (Operators.readonlyvalue.Contains(op))
		    {
		        read  = true;
		        write = false;
		    }
		    else if (Operators.writeonlyvalue.Contains(op))
		    {
		        read  = false;
		        write = true;
		    }
		    else if (Operators.readwritevalue.Contains(op))
		    {
		        read  = true;
		        write = true;
		    }
		}
		
		public static void ParseDeclaration
		    
        		    ( string line,
        		      ref string key,
        		      ref bool read,
        		      ref bool write,
        		      ref bool referential,
        		      ref string typename,
        		      ref string op,
        		      ref string expression )
		{
		    string left = "", right = "";
		    
			// SET  : <nil>
			// line : "const ref name of type string := \"Adam\""
			
			Declarations.GetPermissionProperties(ref line, ref left, ref right, ref read, ref write);
			
			// SET  :  read, write
			// line : "ref name of type string := \"Adam\""
			
			Declarations.GetOrientationProperty(ref line, ref left, ref right, ref referential);
			
			// SET  :  read, write, referential
			// line : "name of type string := \"Adam\""
			
			Declarations.GetPayloadProperty(ref line, ref left, ref op, ref right, ref expression);
			
			// SET  :  read, write, referential, payload
			// line : "name of type string"
			
			string temp = "";
			
			Declarations.GetTypenameProperty(ref line, ref left, ref temp, ref right, ref typename);
			
			// SET  :  read, write, referential, payload, typename
			// line : "name"
			
			key = line.TrimEnd();
			
			// SET  : read, write, referential, payload, typename, key
		}
		
		public static class Declarations
		{
    		public static void GetPermissionProperties
    		    
    		            ( ref string line,
    		              ref string left,
    		              ref string right,
    		              ref bool read,
    		              ref bool write )
    		{
    			Verbs.SplitSentence(line, ref left, ref right);
    			
    			if (Operators.permissionops.Contains(left))
    			{
    			    SpecifyPermissions(left, ref read, ref write);
    			    
    			    line = right;
    			}
    			else
    			{
    			    read = write = true;
    			}
    		}
    		
    		public static void GetOrientationProperty
    		    
            		    ( ref string line,
            		      ref string left,
            		      ref string right,
            		      ref bool referential )
    		{
    			Verbs.SplitSentence(line, ref left, ref right);
    			
    			if (referential = Operators.bidirectional.Contains(left))
    			{
    			    line = right;
    			}
    		}
    		
    		public static void GetPayloadProperty
    		    
            		    ( ref string line,
            		      ref string left,
            		      ref string op,
            		      ref string right,
            		      ref string expression )
    		{
    			if (Interpreter.Sequence.SplitTrinomial(ref left, ref op, ref right, line, Operators.assignops, Patterns.getbinaryop))
    			{
    			    expression = right.Trim();
    			    
    			    line = left;
    			}
    			else
    			{
    			    expression = "";
    			}
    		}
    		
    		public static void GetTypenameProperty
    		    
            		    ( ref string line,
            		      ref string left,
            		      ref string op,
            		      ref string right,
            		      ref string typename )
    		{
    			if (Interpreter.Sequence.SplitTrinomial(ref left, ref op, ref right, line, Operators.statictype, Patterns.getbinaryop))
    			{
    			    typename = right.Trim();
    			    
    			    line = left;
    			}
    			else
    			{
    			    typename = Definition.DEFAULT_TYPENAME;
    			}
    		}
		}
		
//    	public static void Declare
//    	    
//        		    ( Interpreter.Sequence receiver,
//    	              string line,
//    	              string key,
//    	              bool read,
//    	              bool write,
//    	              Dictionary<string, Named> table = null,
//    	              string typename = "",
//    	              string expression = "" )
//    	{
//    	    object payload = String.IsNullOrWhiteSpace(expression) ? Value.NOVALUE : receiver.Expression(expression);
//    	    
//    	    if (String.IsNullOrWhiteSpace(typename))
//    	    {
//    	        if (table == null)
//    	        {
//    	            AddDynamicNamedValue(receiver.name_table, key, new Named.Value(payload, read, write), line);
//    	        }
//    	        else
//    	        {
//    	            AddNonStandardNamedValue(receiver.name_table, key, new Named.Reference(table, payload, read, write), line);
//    	        }
//    	    }
//    	    else
//    	    {
//    	        if (table == null)
//    	        {
//    	            AddNonStandardNamedValue(receiver.name_table, key, new Named.Static.Value(typename, payload, read, write), line);
//    	        }
//    	        else
//    	        {
//    	            AddNonStandardNamedValue(receiver.name_table, key, new Named.Static.Reference(typename, table, payload, read, write), line);
//    	        }
//    	    }
//    	}
		
		public static void Declare
		    
        		    ( Interpreter.Sequence receiver,
		              string line,
		              string key,
		              bool read,
		              bool write,
		              Dictionary<string, Named> table = null,
		              string typename = "",
		              string expression = "" )
		{
		    if (String.IsNullOrWhiteSpace(typename))
		    {
		        if (table == null)
		        {
        		    object payload = String.IsNullOrWhiteSpace(expression) ? Value.NOVALUE : receiver.Expression(expression);
        		    
		            AddDynamicNamedValue(receiver.Names, key, new Named.Value(payload, read, write), line);
		        }
		        else
		        {
		            AddNonStandardNamedValue(receiver.Names, key, new Named.Reference(table, expression, read, write), line);
		        }
		    }
		    else
		    {
		        if (table == null)
		        {
        		    object payload = String.IsNullOrWhiteSpace(expression) ? Value.NOVALUE : receiver.Expression(expression);
        		    
		            AddNonStandardNamedValue(receiver.Names, key, new Named.Static.Value(typename, payload, read, write), line);
		        }
		        else
		        {
		            AddNonStandardNamedValue(receiver.Names, key, new Named.Static.Reference(typename, table, expression, read, write), line);
		        }
		    }
		}
		
		public static void Declare
		    
        		    ( Interpreter.Sequence receiver,
		              string line,
		              string key,
		              bool read,
		              bool write,
		              object payload,
		              Dictionary<string, Named> table = null,
		              string typename = "" )
		{
		    if (String.IsNullOrWhiteSpace(typename))
		    {
		        if (table == null)
		        {
		            AddDynamicNamedValue(receiver.Names, key, new Named.Value(payload, read, write), line);
		        }
		        else
		        {
		            AddNonStandardNamedValue(receiver.Names, key, new Named.Reference(table, (string) payload, read, write), line);
		        }
		    }
		    else
		    {
		        if (table == null)
		        {
		            AddNonStandardNamedValue(receiver.Names, key, new Named.Static.Value(typename, payload, read, write), line);
		        }
		        else
		        {
		            AddNonStandardNamedValue(receiver.Names, key, new Named.Static.Reference(typename, table, (string) payload, read, write), line);
		        }
		    }
		}
		
		public static void Declare(Interpreter.Sequence receiver, string line)
		{
			string op = "", key = "", typename = "", expression = "";
			bool read = true, write = true, referential = false;
			
			ParseDeclaration(line, ref key, ref read, ref write, ref referential, ref typename, ref op, ref expression);
			
			if (op.Equals(Operators.novelty_declare))
			{
				if (key.ToLower().Equals("war"))
				{
					if (Novelty.Tables.Sovereigns.Contains(expression))
					{
						Console.WriteLine("War were declared.");
					}
					else
					{
						Console.WriteLine("The expression \"" + expression + "\" does not name a sovereign state, you twat.");
					}
				}
			}
			
			VerifyRightToLeftOperator(op);
			
			Declare(receiver, line, key, read, write, referential ? receiver.Names.GetTable(expression) : null, typename, expression);
		}
		
		public static void VerifyRightToLeftOperator(string op)
		{
			if (!String.IsNullOrWhiteSpace(op) && !Operators.right_to_left.Contains(op))
			    
			    throw new FormatException("Left-to-right operators do not apply to declarations.");
		}
		
		public static void AddDynamicNamedValue(NameTable names, string key, Named.Value value, string line)
		{
		    Match match = Regex.Match(key, Patterns.getnewname);
		    
		    if (!match.Success)
		        
		        throw new FormatException("No valid variable name in the phrase: \"" + line.Trim() + "\".");
		    
		    if (!names.AddDynamicValue(match.Value, value))
		        
		        throw new Exception( "Name Conflict: The name \"" + key
		                              + "\" already exists in the variable list." );
		}
		
		public static void AddNonStandardNamedValue(NameTable names, string key, Named value, string line)
		{
		    Match match = Regex.Match(key, Patterns.getnewname);
		    
		    if (!match.Success)
		        
		        throw new FormatException("No valid variable name in the phrase: \"" + line.Trim() + "\".");
		    
		    if (!names.AddNonStandardValue(match.Value, value))
		        
		        throw new Exception( "Name Conflict: The name \"" + key
		                              + "\" already exists in the variable list." );
		}
		
		public static void AddNamedValue(NameTable names, string key, bool read = true, bool write = true)
		{
		    AddDynamicNamedValue(names, key, new Named.Value(null, read, write), key);
		}
		
		public static void AssignTo(Interpreter.Sequence receiver, string line)
		{
			string left = "", op = "", right = "";
			
			if (Interpreter.Sequence.SplitTrinomial(ref left, ref op, ref right, line, Operators.assignops, Patterns.getbinaryop))
			{
				if (Operators.right_to_left.Contains(op))
				{
					receiver.ParseLeftValueExpression(left, receiver.Expression(right));
				}
				else if (Operators.left_to_right.Contains(op))
				{
					receiver.ParseLeftValueExpression(right, receiver.Expression(left));
				}
			}
			else
			{
			    throw new FormatException("Not a valid assign-statement.");
			}
		}
		
		public static void ThrowFormatException(Interpreter.Sequence receiver, string line)
		{
		    throw new FormatException("Verb missing. Not a runnable statement.");
		}
		
		public static void GoToAssignmentOrDeclaration(Interpreter.Sequence receiver, string line)
		{
		    try
		    {
		        if (Operators.permissionops.Contains(Verbs.GetVerb(line)))
		            throw new Exception();
		        
		        AssignTo(receiver, line);
		    }
		    catch
		    {
		        Declare(receiver, line);
		    }
		}
		
		public static void CombineWith(Interpreter.Sequence receiver, string line, BinaryOperation Combine)
		{
		    string left = "", op = "", right = "";
		    
		    if (Interpreter.Sequence.SplitTrinomial(ref left, ref op, ref right, line, Operators.assignops, Patterns.getbinaryop))
		    {
		        if (Operators.right_to_left.Contains(op))
		        {
		            receiver.ParseLeftValueExpression(left, Combine(receiver.Expression(left), receiver.Expression(right)));
		        }
		        else if (Operators.left_to_right.Contains(op))
		        {
		            receiver.ParseLeftValueExpression(right, Combine(receiver.Expression(right), receiver.Expression(left)));
		        }
		    }
		    else
		    {
		        throw new FormatException("Not a valid assign-statement.");
		    }
		}
		
		public static void AddTo(Interpreter.Sequence receiver, string line)
		{
		    CombineWith(receiver, line, Algorithms.Arithmetic.Sum);
		}
		
		public static void SubtractFrom(Interpreter.Sequence receiver, string line)
		{
		    CombineWith(receiver, line, Algorithms.Arithmetic.Difference);
		}
		
		public static void MultiplyBy(Interpreter.Sequence receiver, string line)
		{
		    CombineWith(receiver, line, Algorithms.Arithmetic.Product);
		}
		
		public static void DivideBy(Interpreter.Sequence receiver, string line)
		{
		    CombineWith(receiver, line, Algorithms.Arithmetic.Quotient);
		}
		
		public static void RemainderFrom(Interpreter.Sequence receiver, string line)
		{
		    CombineWith(receiver, line, Algorithms.Arithmetic.Remainder);
		}
		
		public static void Clear(Interpreter.Sequence receiver, string line)
		{
			receiver.ParseLeftValueExpression(line, null);
		}
		
		public static void Release(Interpreter.Sequence receiver, string line)
		{
		    receiver.Names.Remove(line.Trim());
		}
		
		public static void Increment(Interpreter.Sequence receiver, string line)
		{
		    receiver.ParseLeftValueExpression(line, Algorithms.Arithmetic.Sum(receiver.Expression(line), 1L));
		}
		
		public static void Decrement(Interpreter.Sequence receiver, string line)
		{
		    receiver.ParseLeftValueExpression(line, Algorithms.Arithmetic.Difference(receiver.Expression(line), 1L));
		}
		
		public static void Input(Interpreter.Sequence receiver, string line)
		{
		    receiver.ParseLeftValueExpression(line, Console.Read());
		}
		
		public static void InputLine(Interpreter.Sequence receiver, string line)
		{
		    receiver.ParseLeftValueExpression(line, Console.ReadLine());
		}
		
		public static void Output(Interpreter.Sequence receiver, string line)
		{
		    Console.Write(receiver.Expression(line));
		}
		
		public static void OutputLine(Interpreter.Sequence receiver, string line)
		{
		    Console.WriteLine(String.IsNullOrWhiteSpace(line) ? "" : receiver.Expression(line));
		}
		
		public static void Error(Interpreter.Sequence receiver, string line)
		{
		    Console.Error.Write(receiver.Expression(line));
		}
		
		public static void ErrorLine(Interpreter.Sequence receiver, string line)
		{
		    Console.Error.WriteLine(receiver.Expression(line));
		}
		
		public static void ReturnValue(Interpreter.Sequence receiver, string line)
		{
		    throw new Return(receiver.Expression(line));
		}
		
		public static void CallProcess(Interpreter.Sequence receiver, string line)
		{
			line = line.Trim();

			if (String.IsNullOrEmpty(line))
			    throw new FormatException("Not a valid expression.");

			// "FunctionName(arg1, arg2, arg3)"
			
			string first = MicroRegex.Match.NextOuterGroup(line).Value;
			
			// first : "FunctionName"
			// secnd : ""
			
			if (Regex.IsMatch(first, Patterns.getname))
			{
				string secnd = MicroRegex.Match.NextOuterGroup(line.Substring(first.Length)).Value;
				
				// secnd : "(arg1, arg2, arg3)"
				
				Match argsCapture = Regex.Match(secnd, Patterns.getargs);
				
				if (argsCapture.Success)
				{
				    // argsCapture.Value : "arg1, arg2, arg3"
				    
				    receiver.LookUpFunction(first.TrimEnd(), receiver.ParseList(argsCapture.Value.Trim()));
				}
				else
				{
					throw new Exception("Syntax Error: Not a valid process call.");
				}
			}
		}
		
		public static class Commands
		{
		    public static string GetCommand(string line)
		    {
		        return GetWord(line, Patterns.getcommand);
		    }
		    
		    public static void SplitImperative(string line, ref string command, ref string complement)
		    {
		        SplitSentence(line, ref command, ref complement, Patterns.getcommand);
		    }
		    
		    public delegate void Function(string s);
		    
    		public static Dictionary<string, Function> command_table = GetCommandsTable();
            
    		public static Dictionary<string, Function> GetCommandsTable()
    		{
    			var commands = new Dictionary<string, Function>();
    			
    			commands.Add(Operators.Verbs.Commands.show, new Function(ShowInfo));
    			
    			commands.Add(Operators.Verbs.Commands.load, new Function(LoadTranscript));
    			
    			commands.Add(Operators.Verbs.Commands.reset, new Function(ClearTranscript));
    			
    			commands.Add(Operators.Verbs.Commands.reload, new Function(ReloadTranscript));
    			
    			commands.Add(Operators.Verbs.Commands.terminate, new Function(TerminateCommandLineInterface));
    			
    			commands.Add(Operators.Verbs.Commands.push, new Function(PushLevelOntoNameTable));
    			
    			commands.Add(Operators.Verbs.Commands.pop, new Function(PopLevelFromNameTable));
    			
    			commands.Add("what", new Function(ProcessWhat));
    			
    			commands.Add("fail", new Function(DisruptCommandLineInterface));
    			
    			foreach (var word in Operators.Verbs.Commands.gethelp.ToList())
    			    commands.Add(word, new Function(GetHelp));
    			
    			foreach (var word in Operators.Verbs.Commands.clearscreen.ToList())
    				commands.Add(word, new Function(ClearScreen));
    			
    			return commands;
    		}
    		
    		public static bool LoadFileIntoTranscript(string line)
    		{
    		    if (Transcript.PushFile(line))
    		    {
    		        Console.ForegroundColor = ConsoleColor.Green;
    		        Console.WriteLine(Preferences.FIRST_INDENTATION + "Added :  " + line);
    		        Console.ResetColor();
    		        
    		        return true;
    		    }
    		    
    		    return false;
    		}
    		
		    public static bool LoadFilesIntoTranscript(string line)
		    {
		        bool loadedFile = false;
		        
	            foreach (var path in MicroRegex.Match.Split(line, Patterns.getlistdelimiter, MicroRegex.SearchTypes.REGEX, true))
	            {
                    loadedFile |= LoadFileIntoTranscript(path);
	            }
	            
	            return loadedFile;
		    }
		    
            public static bool LoadFromDirectoryToTranscript(string path)
            {
                bool loadedFile = false;
                
                foreach (var file in System.IO.Directory.GetFiles(path))
                {
                    loadedFile |= LoadFileIntoTranscript(file);
                }
                
                return loadedFile;
            }
            
		    public static void LoadTranscript(string line)
		    {
		        bool loadedFile;
		        
		        string command = null, complement = null;
		        
		        SplitImperative(line, ref command, ref complement);
		        
		        try
		        {
    		        switch (command)
    		        {
    		            case "dir"   : 
    		            {
    		                SplitImperative(complement, ref command, ref complement);
    		                
                            loadedFile = command.Equals(Operators.Verbs.Commands.current) ?
                                    
                                    LoadFromDirectoryToTranscript(Preferences.default_directory) :
                                    LoadFromDirectoryToTranscript(complement.Trim());
    		                
    		                break;
    		            }
    		            case "file"  : loadedFile = LoadFileIntoTranscript(complement.Trim());
    		                           break;
    		            case "files" : loadedFile = LoadFilesIntoTranscript(complement);
    		                           break;
    		            default      : 
    		            {
    		                loadedFile = String.IsNullOrWhiteSpace(complement) ?
    		                        
    		                        LoadFromDirectoryToTranscript(Preferences.default_directory) :
    		                        LoadFilesIntoTranscript(complement);
    		                
    		                break;
    		            }
    		        }
		        }
		        catch (Exception e)
		        {
		            CommandLineInterface.ShowError(Preferences.FIRST_INDENTATION + "Error :  " + e.Message);
		            
		            loadedFile = false;
		        }
		        
		        if (!loadedFile)
		        {
		            Console.WriteLine("\nNone of the pages specified were loaded successfully.");
		        }
		    }
		    
		    public static void ClearTranscript(string line)
		    {
		        if (
		              GetVerb(line).Equals(Operators.Verbs.Commands.force) ||
    		         
		                CommandLineInterface.AwaitConfirmationKeyPress
    		            (
    		                "This will erase all definitions currently loaded in the transcript.\nContinue? (Y/N) "
    		            )
		           )
		        {
		    		Definition.Clear();
		            Transcript.Clear();
		        }
		    }
		    
		    public static void ReloadTranscript(string line)
		    {
		        ClearTranscript(line);
		        
		        LoadTranscript("dir current");
		    }
		    
		    public static void ShowInfo(string line)
		    {
		        string command = null, complement = null;
		        
		        SplitImperative(line, ref command, ref complement);
		        
		        switch (command)
		        {
		            case "transcript"  : Console.Write(Transcript.TableToString());
		                                 break;
		            case "defs"        : 
		            case "defines"     : 
		            case "definitions" : Console.Write(Definition.TableToString());
		                                 break;
		            case "names"       : 
		            case "table"       : 
                    case "nametable"   : Console.Write(CommandLineInterface.Sequencer.Names.ToString());
                                         break;
                    case "commands"    : Console.Write(Algorithms.Collections.ToString(command_table));
                                         break;
                    case "verbs"       : Console.Write(Algorithms.Collections.ToString(verb_table));
                                         break;
	                default :
	                {
                        if (Operators.Verbs.Commands.gethelp.Contains(command))
	                    {
	                        GetHelp(complement);
	                    }
	                    
	                    break;
	                }
		        }
		    }
		    
		    public static void GetHelp(string line)
		    {
		        throw new NotImplementedException();
		    }
		    
		    public static void TerminateCommandLineInterface(string line)
		    {
		        throw new NotImplementedException();
		    }
		    
		    public static void ClearScreen(string line)
		    {
		    	Console.Clear();
		    }
		    
		    public static void PushLevelOntoNameTable(string line)
		    {
		    	CommandLineInterface.Sequencer.Names.Push();
		    }
		    
		    public static void PopLevelFromNameTable(string line)
		    {
		    	CommandLineInterface.Sequencer.Names.Pop();
		    }
		    
		    public static void ProcessWhat(string line)
		    {
		    	if (CommandLineInterface.AwaitConfirmationKeyPress("Is there a problem? Don't say yes. (Y/N) "))
		    	{
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.WriteLine("\n\nThis is unacceptible.");
					Console.ResetColor();
					
					System.Threading.Thread.Sleep(5000);
		    	}
		    	else
		    	{
		    		Console.WriteLine();
		    		
		    		for (int i = 1; i <= 3; ++i)
		    		{
		    			System.Threading.Thread.Sleep(1000);
		    			Console.Write(". ");
		    		}
		    		
		    		System.Threading.Thread.Sleep(1000);
		    		
		    		Console.WriteLine("Good.");
		    		
		    		System.Threading.Thread.Sleep(2000);
		    	}
		    }
		    
		    public static void DisruptCommandLineInterface(string line)
		    {
				const int soICameUpWithThisAmazingIdea     = 1;
				const int idPretendIWasOneOfThoseDeafMutes = 3;
				
				switch (new Random().Next() % 2)
				{
					case 0 : 
					{
						Console.WriteLine("\n\nGoodbye, cruel world!\n");
						System.Threading.Thread.Sleep(1000);
						
						while(true)
						{
							System.Threading.Thread.Sleep(soICameUpWithThisAmazingIdea);
							Console.Write("HA" + new string(' ', new Random().Next() % idPretendIWasOneOfThoseDeafMutes));
						}
					}
					
					case 1 :
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("\n\nBlood for the blood god!\n");
						System.Threading.Thread.Sleep(1000);
						
						while(true)
						{
							System.Threading.Thread.Sleep(soICameUpWithThisAmazingIdea);
							
							switch (new Random().Next() % 4)
							{
								case 0 : Console.ForegroundColor = ConsoleColor.DarkRed;
								         Console.Write("DEATH ");
								         break;
								case 1 : Console.ForegroundColor = ConsoleColor.Red;
								         Console.Write("DYING ");
								         break;
								case 2 : Console.ForegroundColor = ConsoleColor.DarkRed;
								         Console.Write("HELP ");
								         break;
								case 3 : Console.ForegroundColor = ConsoleColor.Red;
								         Console.Write("BARF ");
								         break;
							}
							
							switch (new Random().Next() % 10)
							{
								case 0 : System.Threading.Thread.Sleep(soICameUpWithThisAmazingIdea);
										 Console.ForegroundColor = ConsoleColor.DarkRed;
								         Console.Write("NO ONE CAN STOP THE PAIN ");
								         break;
								case 1 : System.Threading.Thread.Sleep(soICameUpWithThisAmazingIdea);
										 Console.ForegroundColor = ConsoleColor.Red;
								         Console.Write("PLEASE CAN YOU STOP THE PAIN ");
								         break;
							}
						}
					}
				}
		    }
		}
    }
}
