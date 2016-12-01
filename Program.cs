using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using NUnit.Framework;

namespace PowerWalk
{
    // TODO: Add Help to CLI.
    // TODO: Add internal file store for preferences.
    // TODO: Add Preferences to CLI.
    // TODO: Add descriptive error messages to transcript loader: File does not exist, No usable files in list
    // TODO: Add autocomplete for: commands, subcommands, verbs, subverbs, definitions, file directory.
    // TODO: Move classes CommandLineInterface and Preferences to their own files.
    // TODO: Rename project and namespace to something like PowerWalk.
    // TODO: Rename Interpreter and Sequence classes.
    // TODO: Possibly merge Interpreter and Sequence classes.
    // TODO: Rename SplitSentence to SplitPredicate or SplitClause maybe.
    // TODO: Remove unnecessary unboxing type-casts.
    // TODO: Replace all collections with generic collections.
    // TODO: Interval class is not Generic.
    // TODO: Add comprehensive list to table of static types.
    // TODO: Encapsulate indentation size.
    // TODO: Compile and encapsulate all keywords.
    // TODO: Compose a single table of all keywords.
    // TODO: Add color-coding to keywords.
    // TODO: Add 'please' to CLI.
    // TODO: Write custom Object class from which all other pseudocode data types should derive.
    // TODO: Add 'dir' or 'ls' command (or 'get dir' or 'get files').
    // TODO: Make name table ToString more verbose.
    // TODO: Make smarter ToString's for all tables.
    // TODO: Make error messages more consistent.
    // TODO: Maybe apply aggregate operators to Intervals on Intervals.
    
    // FIXME: error and errorline
    // FIXME: read and readline, maybe
    // FIXME: Currently not functional: Ctl+C, Shft+Tab, Up, Down, Left, Right
    
    // DONE: Replace sorted sets with ternary trees.
    // DONE: Replace all Strings with strings.
    // DONE: Add 'cls', 'clearscreen', or 'clear screen' command.
    
    public static class CommandLineInterface
    {
        private static Interpreter.Sequence _sequencer = new Interpreter.Sequence();
        private static int _prompt_left;
        private static int _prompt_top;
        private static int _cursor_left;
        private static int _cursor_top;
        private static System.Text.StringBuilder _buffer = new System.Text.StringBuilder();
        private static List<string> _suggestions = new List<string>();
        private static int _index;
        
        public static Interpreter.Sequence Sequencer
        {
            get { return _sequencer; }
        }
        
//        private static System.Diagnostics.ProcessStartInfo _win_process = new System.Diagnostics.ProcessStartInfo()
//        {
//        	UseShellExecute  = false,
//        	WorkingDirectory = @"C:\Windows\System32",
//        	WindowStyle      = System.Diagnostics.ProcessWindowStyle.Normal
//        };
//        
//        private static void RunCommandLine(string command, string sender, string receiver)
//        {
//        	if (String.IsNullOrWhiteSpace(sender))
//        	{
//        		
//        	}
//        	else
//        	{
//	        	MicroRegex.Match capture = MicroRegex.Match.NextMatch( receiver, "(?=~)", 0,
//	        	                                                       MicroRegex.SearchTypes.REGEX,
//	        	                                                       MicroRegex.MatchTypes.WordOnly,
//	        	                                                       MicroRegex.Match.ANYQUOTE );
////        		capture.Value;
//        		receiver = receiver.Substring(capture.Length);
//        	}
//        }
        
        public static void Main()
        {
            string commandLine, verb = null, complement = null;
            
            do
            {
                commandLine = GetCommandLine();
            }
            while (String.IsNullOrWhiteSpace(commandLine));
            
            Verbs.SplitSentence(commandLine, ref verb, ref complement);
            
            while (!verb.Equals(Operators.Verbs.Commands.terminate))
            {
                Console.WriteLine();
                
                RunCommandLine(verb, complement);
                
                Console.WriteLine();
                
                do
                {
                    commandLine = GetCommandLine();
                }
                while (String.IsNullOrWhiteSpace(commandLine));
                
                Verbs.SplitSentence(commandLine, ref verb, ref complement);
            }
        }
        
        public static void RunCommandLine(string verb, string complement)
        {
			try
			{
    			if (Verbs.verb_table.ContainsKey(verb))
    			{
    			    Verbs.verb_table[verb](_sequencer, complement);
    			}
    			else if (Verbs.Commands.command_table.ContainsKey(verb))
    			{
    			    Verbs.Commands.command_table[verb](complement);
    			}
    			else
    			{
    			    ShowError("The command '" + verb + "' could not be identified.");
    			}
			}
			catch (Exception e)
			{
			    ShowError(e);
			}
        }
        
        public static string GetCommandLine()
        {
            ShowPrompt();
//            return ReadLine();
			return Console.ReadLine();
        }
        
        public static string ReadLine()
        {
			_buffer.Clear();
			
			var input = Console.ReadKey();
			
			while (input.Key != ConsoleKey.Enter)
			{	
				switch (input.Key)
				{
					case ConsoleKey.Backspace:
						
						_cursor_left = Console.CursorLeft;
						_cursor_top  = Console.CursorTop;
						
						Backspace();
				        break;
					
					case ConsoleKey.Tab:
				        
				        ShowNextSuggestion();
				        
						_cursor_left = Console.CursorLeft;
						_cursor_top  = Console.CursorTop;
						
						break;
					
					case ConsoleKey.UpArrow:
					case ConsoleKey.DownArrow:
					case ConsoleKey.LeftArrow:
	                case ConsoleKey.RightArrow:
						
						_cursor_left = Console.CursorLeft;
						_cursor_top  = Console.CursorTop;
						
						Reverse(2);
					    break;
						  
                    default:
					    
						_cursor_left = Console.CursorLeft;
						_cursor_top  = Console.CursorTop;
						
					    _buffer.Append(input.KeyChar);
                        break;
				}
				
				input = Console.ReadKey();
				
				if (_cursor_left == 0 && _cursor_left == Console.CursorLeft)
					Console.SetCursorPosition(Console.WindowWidth - 1, Console.CursorTop - 1);
			}
			
			Console.SetCursorPosition(0, Console.CursorTop + 1);
			
			return _buffer.ToString();
        }
        
        private static void ShowNextSuggestion()
        {
        	string nextResult = "";
        	
        	string currentCommandLine = _buffer.ToString();
        	
        	if (_suggestions.Any() && _suggestions[_index].Equals(currentCommandLine))
        	{
        		_index = _index == _suggestions.Count - 1 ? 0 : _index + 1;
        		
        		nextResult = _suggestions[_index];
        	}
        	else
        	{
        		string verb = null, complement = null;
        		
        		Verbs.Commands.SplitImperative(currentCommandLine, ref verb, ref complement);
        		
        		if (Operators.Verbs.callprocess.Contains(verb))
        		{
		        	if (_suggestions.Any() && _suggestions[_index].Equals(complement))
		        	{
		        		_index = _index == _suggestions.Count - 1 ? 0 : _index + 1;
		        	}
		        	else
		        	{
        				_suggestions = Definition.KeyNames.GetSuggestions(complement);
        				
        				_index = 0;
		        	}
        			
		        	if (_suggestions.Any())
        				nextResult = verb + " " + _suggestions[_index];
        		}
        		else if (Operators.Verbs.Commands.load.Equals(verb))
        		{
        			string subcommand = null, subcomplement = null;
        			
        			Verbs.Commands.SplitImperative(complement, ref subcommand, ref subcomplement);
        			
        			switch (subcommand)
        			{
        				case "dir":
        					break;
        				case "file":
        					break;
        				case "files":
        					break;
        				default:
        					break;
        			}
        		}
        		else
        		{
        			_index = 0;
        			
        			_suggestions = Operators.Verbs.Commands.Set.GetSuggestions(currentCommandLine);
        			
        			if (!_suggestions.Any())
        				_suggestions = Operators.Verbs.Set.GetSuggestions(currentCommandLine);
        			
        			if (_suggestions.Any())
        				nextResult = _suggestions[_index];
        		}
        	}
        	
        	if (_suggestions.Any())
        	{
        		_buffer.Clear();
				_buffer.Append(nextResult);
	        	
	        	ClearCurrentCommandLine();
	        	Console.Write(_buffer);
//	        	Write(_buffer.ToString());
	        }
        	else
        	{
        		Cancel();
        	}
        }
        
        private static void Write(string commandLine)
        {
        	string command = null, complement = null;
        	
        	Verbs.Commands.SplitImperative(commandLine, ref command, ref complement);
        	
        	if (Operators.Verbs.Commands.Set.Contains(command))
        	{
        		Console.ForegroundColor = ConsoleColor.Cyan;
        	}
        	else if (Operators.Verbs.Set.Contains(command))
        	{
        		Console.ForegroundColor = ConsoleColor.Magenta;
        	}
        	
        	Console.Write(command);
        	Console.ResetColor();
        	
        	foreach (var word in complement.Split(' '))
        	{
	        	if (Operators.Verbs.Commands.Set.Contains(command))
	        	{
	        		Console.ForegroundColor = ConsoleColor.Cyan;
	        	}
	        	else if (Operators.Verbs.Set.Contains(command))
	        	{
	        		Console.ForegroundColor = ConsoleColor.Magenta;
	        	}
	        	
	        	Console.Write(" " + word);
	        	Console.ResetColor();
        	}
        }
        
        private static void ClearCurrentCommandLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(_prompt_left, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(_prompt_left, currentLineCursor);
        }
        
        private static void Reverse(int len)
        {
        	Console.SetCursorPosition(Console.CursorLeft - len + 1 >= _prompt_left ? Console.CursorLeft - len + 1 : _prompt_left, Console.CursorTop);
        }
        
        private static void Cancel()
        {
			Console.SetCursorPosition(_cursor_left, _cursor_top);
        }
        
        private static void PushCursorBack()
        {
        	Console.SetCursorPosition
        	(
        		Console.CursorTop == _prompt_top ?
        		
        			(Console.CursorLeft - 1 >= _prompt_left ? Console.CursorLeft - 1 : _prompt_left) :
        		
        			(Console.CursorLeft > 0 ? Console.CursorLeft - 1 : Console.WindowWidth - 1),
        		
        		Console.CursorTop > _prompt_top && Console.CursorLeft == 0 ? Console.CursorTop - 1 : Console.CursorTop
        	);
        }
        
        private static void Backspace()
        {
        	Console.Write(" ");
        	
        	PushCursorBack();
        	
        	if (_buffer.Length > 0)
        		_buffer.Remove(_buffer.Length - 1, 1);
        }
        
	    public static bool AwaitConfirmationKeyPress(string message)
	    {
	        Console.Write(message);
	        
	        ConsoleKeyInfo keyPress;
	        
	        do
	        {
	            keyPress = Console.ReadKey();
	        }
	        while (!keyPress.Key.Equals(ConsoleKey.Y) && !keyPress.Key.Equals(ConsoleKey.N));
	        
	        Console.WriteLine();
	        
	        switch (keyPress.Key)
	        {
	            case ConsoleKey.Y : return true;
	            case ConsoleKey.N : return false;
	            default           : throw new Exception("Bug: Confirming using a nonbinary value (not Y or N).");
	        }
	    }
	    
        public static void ShowPrompt()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(Preferences.GetPrompt());
            _prompt_left = Console.CursorLeft;
            _prompt_top  = Console.CursorTop;
            Console.ResetColor();
        }
        
        public static void ShowError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
        }
        
        public static void ShowError(Exception e)
        {
            ShowError(e.Message);
        }
        
		public static string GetErrorMessageWithLineNumberAndHeading(int lineNumber, string heading, Exception e)
		{
		    return GetMessageWithLineNumber(lineNumber, heading + ": " + e.Message);
		}
		
		public static string GetMessageWithLineNumber(int lineNumber, string message)
		{
		    return Regex.IsMatch(message, Patterns.linenumber) ? message : "Line " + (lineNumber + 1) + ": " + message;
		}
    }
    
    public static class Preferences
    {
        public const  string PROGRAM_NAME            = "PWalk";
        public const  string PAGE_EXTENSION          = ".walk";
		public const  string FIRST_INDENTATION       = "  ";
        
        public static bool   show_current_directory  = true;
        public static string default_directory       = ".";
		public static int    tab_size                = 4;
        
        public delegate string GetString();
        
		public static Verbs.Method NonverbalAction = Verbs.GoToAssignmentOrDeclaration;
		
        public static GetString GetPrompt = GetPromptWithCurrentDirectory;
        
        public static string GetPromptWithCurrentDirectory()
        {
            return PROGRAM_NAME + " " + System.IO.Directory.GetCurrentDirectory() + "> ";
        }
        
        public static string GetPromptWithoutCurrentDirectory()
        {
            return PROGRAM_NAME + "> ";
        }
        
        public static void ChangePromptType(bool showCurrentDirectory)
        {
            show_current_directory = showCurrentDirectory;
            
            GetPrompt = show_current_directory ? (GetString) GetPromptWithCurrentDirectory
                                               : (GetString) GetPromptWithoutCurrentDirectory;
        }
        
        public static void TogglePromptType()
        {
            if (show_current_directory)
            {
                show_current_directory = false;
                
                GetPrompt = GetPromptWithoutCurrentDirectory;
            }
            else
            {
                show_current_directory = true;
                
                GetPrompt = GetPromptWithCurrentDirectory;
            }
        }
    }
    
	public partial class Interpreter
	{
		/**********************
		 * --- ATTRIBUTES --- *
		 **********************/
		
//		// No longer needed.
//		public static void Start()
//		{
//		    
//		}
		
		public Interpreter() {}
		
		public partial class Sequence
		{
			private readonly NameTable _name_table;
			
			public NameTable Names
			{
			    get { return _name_table; }
			}
			
			public int Levels
			{
			    get { return _name_table.Count; }
			}
			
			public object GetNamedValue(string key)
			{
			    return _name_table.Get(key);
			}
			
			public Sequence(NameTable parameters)
			{
				_name_table = parameters;
			}
            
			public Sequence(): this(new NameTable()) {}
			
			public Sequence(Dictionary<string, Named> parameters): this(new NameTable(parameters)) {}
			
			
			/***********************
			 * --- Run Process --- *
			 ***********************/
			
			
			public object Run(string process)
			{
                var queue = new List<string>();
                
                MicroRegex.Match match;
                
                List<string> split = MicroRegex.Match.Split( process, "\n",
                                                             MicroRegex.SearchTypes.STRING,
                                                             MicroRegex.MatchTypes.NonLiteralSequence,
                                                             MicroRegex.Match.ANYQUOTE );
                
                int line;
                
                for (line = 0; line < split.Count; ++line)
                {
                    match = MicroRegex.Match.NextMatch
                            (
                                split[line], Operators.comment,
                                MicroRegex.SearchTypes.STRING,
                                MicroRegex.MatchTypes.NonLiteralSequence,
                                MicroRegex.Match.ANYQUOTE
                            );
                    
                    if (match.Success)
                    {
                        split[line] = split[line].Substring(0, match.Index);
                    }
                    
                    if (line < split.Count - 1)
                    {
                        Transcript.ContinueNextLine(ref split, ref line);
                    }
                    
                    queue.Add(split[line]);
                }
                
                line = 0;
                
                try
                {
                    while (line < queue.Count)
                    {
                        line = Execute(queue, line);
                    }
                }
                catch (Return r)
                {
                    return r.Payload;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
                
                return null;
			}
			
			
			/*****************
			 * --- Verbs --- *
			 *****************/
			
			
			public static int GetLevel(string line)
			{
				int lvl = 0;
				
				while (lvl < line.Length && line[lvl++] == ' ') {}
				
				return lvl / Preferences.tab_size;
			}
			
			public void MoveScope(int lvl)
			{
				if (lvl > _name_table.Count)
				{
					while (_name_table.Count < lvl)
						_name_table.Push();
				}
				else if (lvl < _name_table.Count)
				{
					while (_name_table.Count > lvl)
						_name_table.Pop();
				}
			}
			
			public int Execute(List<string> queue, int start, int extraLvl = 0)
			{
				MoveScope(GetLevel(queue[start]) + extraLvl);
				
				if (String.IsNullOrWhiteSpace(queue[start]))
				    return start + 1;
				
				string verb = "", complement = "";
				
				Verbs.SplitSentence(queue[start], ref verb, ref complement);
				
				if (String.IsNullOrWhiteSpace(verb))
				{
				    Preferences.NonverbalAction(this, complement);
				    return start + 1;
				}
				
			    try
			    {
    				switch (verb)
    				{
    					case "if"     : return If(queue, start);
    					case "else"   : return Bypass(queue, start);
    					case "while"  : return While(queue, start);
    					case "for"    : return For(queue, start);
    					case "switch" : return Switch(queue, start);
    					
    					default : 
    					{
    					    try
    					    {
        					    Verbs.verb_table[verb](this, complement);
        					    return start + 1;
    					    }
    					    catch (KeyNotFoundException)
    					    {
                                Preferences.NonverbalAction(this, queue[start]);
    					        return start + 1;
    					    }
    					}
    				}
			    }
			    catch (Return)
			    {
			        throw;
			    }
			    catch (ArgumentException e)
			    {
                    throw new Exception(CommandLineInterface.GetErrorMessageWithLineNumberAndHeading(start, "Invalid Argument Exception", e));
			    }
			    catch (InvalidCastException e)
			    {
                    throw new Exception(CommandLineInterface.GetErrorMessageWithLineNumberAndHeading(start, "Invalid Cast Exception", e));
			    }
                catch (FormatException e)
                {
                    throw new Exception(CommandLineInterface.GetErrorMessageWithLineNumberAndHeading(start, "Syntax Error", e));
                }
                catch (NullReferenceException e)
                {
                    throw new Exception(CommandLineInterface.GetErrorMessageWithLineNumberAndHeading(start, "Null Reference Exception", e));
                }
                catch (KeyNotFoundException e)
                {
                	throw new Exception(CommandLineInterface.GetErrorMessageWithLineNumberAndHeading(start, "Key Not Found", e));
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new Exception(CommandLineInterface.GetErrorMessageWithLineNumberAndHeading(start, "Index Out Of Range", e));
                }
                catch (DivideByZeroException e)
                {
                    throw new Exception(CommandLineInterface.GetErrorMessageWithLineNumberAndHeading(start, "Divide By Zero", e));
                }
                catch (Exception e)
                {
                    throw new Exception(CommandLineInterface.GetMessageWithLineNumber(start, e.Message));
                }
			}
			
			
			/******************************
			 * --- Control Structures --- *
			 ******************************/
			
			
			public static int Bypass(List<string> queue, int start)
			{
				int lvl = GetLevel(queue[start]);
				
				int cursor = start + 1;
				
				while (cursor < queue.Count && GetLevel(queue[cursor]) > lvl)
				{
    				cursor = cursor + 1;
				}
				
				return cursor;
			}
			
			public int Iterate(List<string> queue, int start, int currentLvl, int extraLvl = 0)
			{
				int cursor = start + 1;
				
				while (cursor < queue.Count && GetLevel(queue[cursor]) > currentLvl)
			        cursor = Execute(queue, cursor, extraLvl);
				
				return cursor;
			}
			
			public int While(List<string> queue, int start)
			{
				int lvl = GetLevel(queue[start]);
				
				int? cursor = null;
				
				while ((Boolean) Expression(Regex.Match(queue[start], Patterns.getwhile).Value))
				{
                    cursor = Iterate(queue, start, lvl);
                    
                    MoveScope(lvl);
				}
				
				return cursor == null ? Bypass(queue, start) : (int) cursor;
			}
			
			public int If(List<string> queue, int start)
			{
				int lvl = GetLevel(queue[start]);
				
				bool enterIf = (Boolean) Expression(Regex.Match(queue[start], Patterns.getif).Value);
				
				int cursor;
				
                cursor = enterIf ? Iterate(queue, start, lvl) : Bypass(queue, start);
				
				string nextWord = Verbs.GetVerb(queue[cursor]);
				
				switch (nextWord)
				{
					case "else" :
					{
				        if (enterIf)
				        {
				            cursor = Bypass(queue, cursor);
				        }
				        else
				        {
    				        nextWord = Verbs.GetVerb((queue[cursor]).TrimStart().Substring(nextWord.Length));
    				        
    				        switch (nextWord)
    						{
    							case "if":
    							{
    								cursor = If(queue, cursor);
    							}
    							break;
    							default:
    							{
        				            cursor = Iterate(queue, cursor, lvl);
    							}
    							break;
    						}
				        }
					}
					break;
				}
				
				return cursor;
			}
			
			public int Switch(List<string> queue, int start)
			{
				throw new NotImplementedException();
			}
			
			public bool ParseFor(string line, ref string name, ref IComparable step, ref IEnumerable coll)
			{
			    MicroRegex.Match capture;
			    
			    // [   for each c in "Hello world!"   ]
			    
			    capture = MicroRegex.Match.NextMatch(line, Patterns.getfor, MicroRegex.SearchTypes.REGEX);
			    
			    if (!capture.Success) return false;
			    
			    int len = capture.Length;
			    
			    // [   for each c in "Hello world!"   ]
			    //             ^
			    
			    string first = line.Substring(len);
			    
			    // [ c in "Hello world!"   ]
			    
			    capture = MicroRegex.Match.NextMatch(first, Patterns.getaggregateop, MicroRegex.SearchTypes.REGEX);
			    
			    if (!capture.Success) return false;
			    
			    int index = capture.Index;
			    len = index + capture.Length;
			    
			    // [ c in "Hello world!"   ]
			    //    ^   ^
			    
			    string secnd = first.Substring(len);
			    
			    // ["Hello world!"   ]
			    
			    first = first.Substring(0, index).TrimStart();
			    
			    // [c]
			    
			    capture = MicroRegex.Match.LastMatch(first, Patterns.getloopcontroller, MicroRegex.SearchTypes.REGEX);
			    
			    if (!capture.Success) return false;
			    
			    // Name captured.
			    name = capture.Value;
			    
			    // [c]
			    
			    first = first.Substring(0, capture.Index);
			    
			    // []
			    
			    // Step captured.
		        step = String.IsNullOrWhiteSpace(first) ?
		        	
		        	1L : (IComparable) Expression( Regex.Match( first, Patterns.getloopstep ).Value );  /* RegexOptions.RightToLeft */
		        
		        coll = (IEnumerable) Expression(secnd);  // TODO: Add error-checking for empty values.
		        
		        if (coll == null) return false;
		        
			    switch (coll.GetType().Name)
			    {
			        case "String":
			        case "ArrayList":
		                return true;
			        case "Interval":
		                ((Interval) coll).EnumeratorStepSize = step;
			            return true;
			        default : throw new ArgumentException("Bug: Attempting to enumerate an unenumerable type: " + coll.GetType().Name + ".");
			    }
			}
			
//			public static IEnumerable Enumerate(IEnumerable coll, object step)
//			{
//			    switch (coll.GetType().Name)
//			    {
//			        case "String":
//			        case "ArrayList":
//			            return coll;
//			        case "Interval":
//			            ((Interval) coll).DefaultStep = step;
//			            return coll;
//			        default : throw new ArgumentException("Bug: Attempting to enumerate an unenumerable type: " + coll.GetType().Name + ".");
//			    }
//			}
			
//			public static IEnumerator GetEnumerator(IEnumerable coll)
//			{
//			    var temp = coll.GetEnumerator();
//			    
//			    if (coll.GetType().Name == "Object[]")
//			    {
//			        temp.MoveNext();
//			    }
//			    
//			    return temp;
//			}
			
			public static bool MoveNext(ref IEnumerator e, IComparable step)
			{
			    if (e.GetType().Name != "Enumerator")
			    {
			        Int64 i;
			        
			        for (i = 1L; i.CompareTo(step) <= 0 && e.MoveNext(); ++i) {}
			        
			        return i.CompareTo(step) > 0;
			    }
			    
			    return e.MoveNext();
			}
			
			public int For(List<string> queue, int start)
			{
			    string      name = "";
			    IComparable step = null;
			    IEnumerable coll = null;
			    
			    if (ParseFor(queue[start], ref name, ref step, ref coll))
			    {
			        int lvl = GetLevel(queue[start]);
			        
			        var enumerator = coll.GetEnumerator();
			        
			        int cursor;
			        
			        if (enumerator.MoveNext())
			        {
			            MoveScope(lvl + 1);
        				
        				if (_name_table.Find(name) < 0)
                            Verbs.AddNamedValue(_name_table, name);
        				
        				_name_table[name] = enumerator.Current;
                        
        				cursor = Iterate(queue, start, lvl, 1);
        				
                        while (MoveNext(ref enumerator, step))
        				{
        				    MoveScope(lvl + 1);
            			    
        				    _name_table[name] = enumerator.Current;
                        
        				    cursor = Iterate(queue, start, lvl, 1);
        				}
			        }
			        else
			        {
			            cursor = Bypass(queue, start);
			        }
    				
    				return cursor;
			    }
			    
			    throw new FormatException("Not a valid for-statement.");
			}
			
			
			/***********************
			 * --- Expressions --- *
			 ***********************/


			public object Expression(string line)
			{
				return ParseTrinomialExpression(line);
			}
			
			public object ParseTrinomialExpression(string line)
			{
				line = line.Trim();
				
				if (String.IsNullOrEmpty(line))
				    throw new FormatException("Not a valid expression.");

				bool success = false;
				int lvl = 0;
				
				string left = "", op = "", right = "";
				
				while (!success && lvl < Operators.binops.Length)
				{
					success = SplitTrinomial(ref left, ref op, ref right, line, Operators.binops[lvl], Patterns.getbinaryop);
				
					if (!success) lvl = lvl + 1;
				}

				if (success)
				{
				    if (String.IsNullOrWhiteSpace(left))
				    {
				        return Evaluate(op, ParseTrinomialExpression(right));
				    }
				    
					return ShortCircuitEvaluate
					(
						ParseTrinomialExpression(left), op, right
					);
				}
				
				return ParseBinomialExpression(line);
			}
			
			public object ParseBinomialExpression(string line)
			{
				line = line.Trim();

				if (String.IsNullOrEmpty(line))
				    throw new FormatException("Not a valid expression.");
				
				MicroRegex.Match match = MicroRegex.Match.NextMatch(line, Patterns.getunaryop, MicroRegex.SearchTypes.REGEX);
				
				if (match.Success && Operators.unops.Contains(match.Value))
				{
				    return ShortCircuitEvaluate(match.Value, line.Substring(match.Index + match.Length));
				}
				
				return ParseMixedGroupExpression(line);
			}
			
			public object ParseMixedGroupExpression(string line)
			{
				line = line.Trim();

				if (String.IsNullOrEmpty(line))
//				    return null;
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
    				    
						// FUNCTION
						return ParseChainExpression
						(
						    LookUpFunction(first.TrimEnd(), ParseList(argsCapture.Value.Trim())),
						    line.Substring(first.Length + secnd.Length)
						);
					}
					
					try
					{
						// KEY VALUE
						return Operators.keyValues[first.TrimEnd()];
					}
					catch (KeyNotFoundException)
					{
						// VARIABLE
						return ParseChainExpression
						(
							LookUpVariable(first.TrimEnd()),
							line.Substring(first.Length)
						);
					}
				}
				
				Match match = Regex.Match(first, Patterns.getstringliteral);
				
				if (match.Success)
				{
					// STRING LITERAL
					return ParseChainExpression
					(
						match.Value,
						line.Substring(first.Length)
					);
				}
				
				if (Regex.IsMatch(first, Patterns.getgroup))
				{
					// GROUP
					return ParseChainExpression
					(
						ParseGroup(first),
						line.Substring(first.Length)
					);
				}
				
				match = Regex.Match(first, Patterns.getlist);
				
				if (match.Success)
				{
					// LIST
					return ParseChainExpression
					(
						ParseList(match.Value),
						line.Substring(first.Length)
					);
				}
				
				return ParseMonomialExpression(line);
			}
			
			public object ParseChainExpression(object baseObject, string line)
			{
				if (!String.IsNullOrWhiteSpace(line))
				{
					string first = "";
					
					if (line.StartsWith(".", StringComparison.Ordinal))
					{
						first = MicroRegex.Match.NextOuterGroup(line, 1).Value;
						
						string secnd = MicroRegex.Match.NextOuterGroup(line.Substring(first.Length + 1)).Value;
						
						// METHOD CALL
						return ParseChainExpression
						(
							LookUpMethod
							(
								/*(Instance)*/
								baseObject,
								first,
						        ParseList(Regex.Match(secnd, Patterns.getmethodargs).Value)
							),
							
							line.Substring(first.Length + secnd.Length + 1)
						);
					}
					
					first = MicroRegex.Match.NextOuterGroup(line).Value;
					
					Match match = Regex.Match(first, Patterns.getsubscript);
					
					if (match.Success)
					{
						// INDEXER
						return ParseChainExpression
						(
							IndexToElement(baseObject, ParseTrinomialExpression(match.Value)),
							line.Substring(first.Length)
						);
					}
				}
				
				return baseObject;
			}
			
			public static object ParseMonomialExpression(string line)
			{
				line = line.Trim();

				if (String.IsNullOrEmpty(line)) return null;

				Match match;

				// Character
				match = Regex.Match(line, Patterns.getcharacter);
				if (match.Success) return (Char) match.Value[0];

				// Integer
				match = Regex.Match(line, Patterns.getinteger);
				if (match.Success) return (Int64) Int64.Parse(match.Value);

				// Float
				match = Regex.Match(line, Patterns.getfloatpointval);
				if (match.Success) return (Double) Double.Parse(match.Value);

				throw new FormatException("The phrase \"" + line.Trim() + "\" does not evaluate.");
			}

			public object ParseGroup(string line)
			{
				line = line.Trim();

				if (String.IsNullOrEmpty(line) || line.Length <= 2)
				    throw new FormatException("Not a valid expression.");

				List<string> bounds = MicroRegex.Match.Split( line.Substring(1, line.Length - 2),
				                                              Patterns.getlistdelimiter,
				                                              MicroRegex.SearchTypes.REGEX );
				
				switch (bounds.Count)
				{
					case 1:
					{
						return ParseTrinomialExpression(bounds[0]);
					}
					
					case 2:
					{
						Boundary first = Boundary.OPEN;
						Boundary secnd = Boundary.OPEN;
						
						switch (line.First())
						{
							case '(' : first = Boundary.OPEN;
								break;
							case '[' : first = Boundary.CLOSED;
								break;
					        default  : throw new Exception("Bug: Attempting to parse a group with an opening " + line.First() + " symbol.");
						}
						
						switch (line.Last())
						{
							case ')' : secnd = Boundary.OPEN;
								break;
							case ']' : secnd = Boundary.CLOSED;
								break;
							default  : throw new Exception("Bug: Attempting to parse a group with a closing " + line.Last() + " symbol.");
						}
			
						return new Interval( (IComparable) ParseTrinomialExpression(bounds[0]), first,
						                     (IComparable) ParseTrinomialExpression(bounds[1]), secnd );
					}
					
					default: throw new FormatException("The phrase, \"" + line + "\", is not a valid group expression.");
				}
			}
			
			public ArrayList ParseList(string line)
			{
				var list = new ArrayList();
				
				foreach (string word in MicroRegex.Match.Split(line, Patterns.getlistdelimiter, MicroRegex.SearchTypes.REGEX))
				{
					if (!String.IsNullOrWhiteSpace(word))
					{
						list.Add(ParseTrinomialExpression(word));
					}
				}
				
				return list;
			}
			
			public object LookUpVariable(string variableName)
			{
				return _name_table[variableName];
			}
			
			public object LookUpFunction(string functionName, ArrayList args)
			{
			    var temp = new List<object>();
			    
			    temp.AddRange(args.ToArray());
			    
			    return Definition.Call(this, functionName, temp);
			}
            
			public object LookUpMethod(object objectName, string methodName, ArrayList args)
			{
				throw new NotImplementedException();
			}
			
			public static ArrayList NewRange(IComparable begin, Boundary lowerBound, 
			                                 IComparable end,   Boundary upperBound,
									         IComparable step)
			{
				if (Convert.ToDouble(step) <= 0.0) throw new ArgumentException("The range step expects an unsigned numerical value.");
				
				switch (begin.GetType().Name)
				{
					case "Char"   :
					case "Int32"  : 
					case "Int64"  : step = Convert.ToInt64(step);
					                step = begin.CompareTo(end) <= 0
					                       ? (Int64) step : -(Int64) step;
					break;
					case "Double" : step = begin.CompareTo(end) <= 0
					                       ? (Double) step : -(Double) step;
					break;
				}
				
				IComparable i = null;
				
				switch (lowerBound)
				{
					case Boundary.OPEN   : i = (IComparable) Algorithms.Arithmetic.Sum(begin, step);
					break;
					case Boundary.CLOSED : i = begin;
					break;
				}
				
				var rangeBounds = new Interval(begin, lowerBound, end, upperBound);
				var rangeList   = new ArrayList();
				
				while (rangeBounds.Contains(i))
				{
					rangeList.Add(i);
					
					i = (IComparable) Algorithms.Arithmetic.Sum(i, step);
				}
				
				return rangeList;
			}
			
			public static bool
			
				SplitTrinomial
				
					( ref string              left,
					  ref string              op,
					  ref string              right,
			          string                  line,
					  SortedStringSet         list,
					  string                  pattern,
					  MicroRegex.SearchTypes  type = MicroRegex.SearchTypes.REGEX )
			{
				MicroRegex.Match match   = MicroRegex.Match.LastMatch(line, pattern, type);
				bool             success = list.Contains(match.Value);
				
				while (!success && match.Index >= 1)
				{
					match   = MicroRegex.Match.LastMatch(line, pattern, match.Index - 1, type);
					success = list.Contains(match.Value);
				}

				if (success)
				{
					left  = line.Substring(0, match.Index);
					op    = match.Value;
					right = line.Substring(match.Index + match.Length);
				}
				
				return success;
			}

			public object ShortCircuitEvaluate(object left, string op, string right)
			{
			    try
			    {
			        if (Operators.or.Contains(op))              return  (Boolean) left || (Boolean) Expression(right);
    				if (Operators.and.Contains(op))             return  (Boolean) left && (Boolean) Expression(right);
    				if (Operators.equal.Contains(op))           return  ((IComparable) left).CompareTo   ((IComparable) Expression(right)) == 0; // left == right;
    				if (Operators.notequal.Contains(op))        return  ((IComparable) left).CompareTo   ((IComparable) Expression(right)) != 0; // left != right;
    				if (Operators.lessthan.Contains(op))        return  ((IComparable) left).CompareTo   ((IComparable) Expression(right)) <  0; // left <  right;
    				if (Operators.greaterthan.Contains(op))     return  ((IComparable) left).CompareTo   ((IComparable) Expression(right)) >  0; // left >  right;
    				if (Operators.lessthanOrEqual.Contains(op)) return  ((IComparable) left).CompareTo   ((IComparable) Expression(right)) <= 0; // left <= right;
    				if (Operators.grtrthanOrEqual.Contains(op)) return  ((IComparable) left).CompareTo   ((IComparable) Expression(right)) >= 0; // left >= right;
    				if (Operators.within.Contains(op))          return  Algorithms.Collections.Contains  (Expression(right), left);
    				if (Operators.without.Contains(op))         return !Algorithms.Collections.Contains  (Expression(right), left);
    				if (Operators.contain.Contains(op))         return  Algorithms.Collections.Contains  (left, Expression(right));
    				if (Operators.notcontain.Contains(op))      return !Algorithms.Collections.Contains  (left, Expression(right));
    				if (Operators.plus.Contains(op))            return  Algorithms.Arithmetic.Sum        (left, Expression(right));
    				if (Operators.minus.Contains(op))           return  Algorithms.Arithmetic.Difference (left, Expression(right));
    				if (Operators.by.Contains(op))              return  Algorithms.Arithmetic.Product    (left, Expression(right));
    				if (Operators.over.Contains(op))            return  Algorithms.Arithmetic.Quotient   (left, Expression(right));
    				if (Operators.mod.Contains(op))             return  Algorithms.Arithmetic.Remainder  (left, Expression(right));
    				if (Operators.match.Contains(op))           return  Regex.IsMatch((string) left, (string) Expression(right));
    				if (Operators.notmatch.Contains(op))        return !Regex.IsMatch((string) left, (string) Expression(right));
			    }
			    catch (InvalidCastException)
			    {
			        throw new InvalidCastException("The '" + op + "' operator could not be applied to the given types: "
			                                        + left.GetType().Name + ", " + right.GetType().Name + ".");
			    }
				
				throw new Exception("Bug: Attempting to evaluate using an undefined binary operator: " + op);
			}

			public static object Evaluate(object left, string op, object right)
			{
			    try
			    {
    				if (Operators.or.Contains(op))              return  (Boolean) left || (Boolean) right;
    				if (Operators.and.Contains(op))             return  (Boolean) left && (Boolean) right;
    				if (Operators.equal.Contains(op))           return  ((IComparable) left).CompareTo((IComparable) right) == 0;
    				if (Operators.notequal.Contains(op))        return  ((IComparable) left).CompareTo((IComparable) right) != 0;
    				if (Operators.lessthan.Contains(op))        return  ((IComparable) left).CompareTo((IComparable) right) <  0; // left < right;
    				if (Operators.greaterthan.Contains(op))     return  ((IComparable) left).CompareTo((IComparable) right) >  0; // left > right;
    				if (Operators.lessthanOrEqual.Contains(op)) return  ((IComparable) left).CompareTo((IComparable) right) <= 0; // left <= right;
    				if (Operators.grtrthanOrEqual.Contains(op)) return  ((IComparable) left).CompareTo((IComparable) right) >= 0; // left >= right;
    				if (Operators.within.Contains(op))          return  Algorithms.Collections.Contains(right, left);    // ((IList) right).Contains(left);
    				if (Operators.without.Contains(op))         return !Algorithms.Collections.Contains(right, left);    //!((IList) right).Contains(left);
    				if (Operators.contain.Contains(op))         return  Algorithms.Collections.Contains(left, right);    // ((IList) left).Contains(right);
    				if (Operators.notcontain.Contains(op))      return !Algorithms.Collections.Contains(left, right);    //!((IList) left).Contains(right);
    				if (Operators.match.Contains(op))           return  Regex.IsMatch((string) left, (string) right);
    				if (Operators.notmatch.Contains(op))        return !Regex.IsMatch((string) left, (string) right);
    				if (Operators.plus.Contains(op))            return  Algorithms.Arithmetic.Sum(left, right);
    				if (Operators.minus.Contains(op))           return  Algorithms.Arithmetic.Difference(left, right);
    				if (Operators.by.Contains(op))              return  Algorithms.Arithmetic.Product(left, right);
    				if (Operators.over.Contains(op))            return  Algorithms.Arithmetic.Quotient(left, right);
    				if (Operators.mod.Contains(op))             return  Algorithms.Arithmetic.Remainder(left, right);
			    }
			    catch (InvalidCastException)
			    {
			        throw new InvalidCastException("The " + op + " operator could not be applied to the given types: "
			                                        + left.GetType().Name + ", " + right.GetType().Name + ".");
			    }
			    
				throw new Exception("Bug: Attempting to evaluate using an undefined binary operator: " + op);
			}

			public object ShortCircuitEvaluate(string op, string right)
			{
				if (Operators.negative.Contains(op))
                    
				    return !(Boolean) ParseMixedGroupExpression(right);

				if (Operators.opposite.Contains(op))
				{
				    object temp = ParseMixedGroupExpression(right);
				    
                    switch (temp.GetType().Name)
                    {
                        case "Int64"  : return -(Int64)  temp;
                        case "Double" : return -(Double) temp;
                        default       : throw new ArgumentException( "The " + op + " operator could not be applied to the given type: "
				                                                      + temp.GetType().Name + "." );
                    }
				}
				
				if (Operators.bidirectional.Contains(op))
				    
					return right.Trim();
				
				throw new ArgumentException( "The " + op + " operator could not be applied to the given type: "
				                              + right.GetType().Name + "." );
			}
			
			public static object Evaluate(string op, object right)
			{
				if (Operators.negative.Contains(op))
                    
                    return !(Boolean) right;

				if (Operators.opposite.Contains(op))
                    
                    switch (right.GetType().Name)
                    {
                        case "Int64"  : return -(Int64)  right;
                        case "Double" : return -(Double) right;
                        default       : throw new ArgumentException( "The " + op + " operator could not be applied to the given type: "
				                                                      + right.GetType().Name + "." );
                    }
				
				throw new ArgumentException( "The " + op + " operator could not be applied to the given type: "
				                              + right.GetType().Name + "." );
			}
			
			public static object IndexToElement(object coll, object index)
			{
				switch (coll.GetType().Name)
				{
					case "ArrayList":
						return ((ArrayList) coll)[Convert.ToInt32((Int64) index)];
					case "String":
						return    ((string) coll)[Convert.ToInt32((Int64) index)];
					case "Hashtable":
						return ((Hashtable) coll)[index];
					default: throw new ArgumentException( "The subscript operator, 'this[]', could not be applied to the given type: "
						                                   + coll.GetType().Name + "." );
				}
			}
			
			
			/**********************************
			 * --- Left-value Expressions --- *
			 **********************************/
			
			
			public bool ParseLeftValueExpression(string line, object value)
			{
				line = line.Trim();

				if (String.IsNullOrEmpty(line)) throw new FormatException("No expression.");

				string first = MicroRegex.Match.NextOuterGroup(line).Value;
				
				if (Regex.IsMatch(first, Patterns.getname))
				{
					string secnd = line.Substring(first.Length);
					
					first = first.TrimEnd();
					
					if (!String.IsNullOrWhiteSpace(secnd))
					{
						// VARIABLE ONLY
						return ParseLeftValueChainExpression
						(
							LookUpVariable(first),
							secnd,
							value
						);
					}
					
					return LookUpVariable(first, value) == value;
				}
				
				throw new FormatException("Not a valid assign-statement.");
			}
			
			public bool ParseLeftValueChainExpression(object baseObject, string line, object value)
			{
				string first = MicroRegex.Match.NextOuterGroup(line).Value;
				
				Match match = Regex.Match(first, Patterns.getsubscript);
				
				if (match.Success)
				{
					string secnd = line.Substring(first.Length);
					
					first = first.TrimEnd();
					
					if (!String.IsNullOrWhiteSpace(secnd))
					{
						// INDEXER ONLY
						return ParseLeftValueChainExpression
						(
							IndexToElement(baseObject, ParseTrinomialExpression(match.Value)),
							secnd,
							value
						);
					}
					
					return IndexToElement(baseObject, ParseTrinomialExpression(match.Value), value);
				}
				
				throw new FormatException("Not a valid assign-statement.");
			}
			
			public static bool IndexToElement(object coll, object index, object value)
			{
				switch (coll.GetType().Name)
				{
					case "ArrayList":
						var list = (ArrayList) coll;
						return value == (list[Convert.ToInt32((Int64) index)] = value);
					case "Hashtable":
						var table = (Hashtable) coll;
						return value == (table[index] = value);
					default: throw new ArgumentException( "The subscript operator, 'this[]', could not be applied to the given type: "
						                                   + coll.GetType().Name + "." );
				}
			}
			
			public object LookUpVariable(string variableName, object value)
			{
				return _name_table[variableName] = value;
			}
		}
    }
}
