/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/29/2016
 * Time: 8:04 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PowerWalk
{
    /// <summary>
    /// Description of Definition.
    /// </summary>
    
    public class Definition
    {
        public const string DEFAULT_TYPENAME = "Object";
        
        private string _path;
        private int    _line_number;
        private string _return_type;
        
        private Definition(string path, int lineNumber, string returnType)
        {
            _path = path;
            _line_number = lineNumber;
            _return_type = returnType;
        }
        
        public string Path
        {
            get { return _path; }
        }
        
        public int LineNumber
        {
            get { return _line_number; }
        }
        
        public string ReturnType
        {
            get { return _return_type; }
        }
        
        public class DefinitionId : IComparable<DefinitionId>
        {
            private readonly string       _name;
            private readonly List<string> _preconditions;
            private Definition            _reference;
            
            public DefinitionId(string name, Definition reference)
            {
                _name          = name;
                _preconditions = new List<string>();
                _reference     = reference;
            }
            
            public DefinitionId(string name, List<string> preconditions, Definition reference)
            {
                _name          = name;
                _preconditions = preconditions;
                _reference     = reference;
            }
            
            public string Name
            {
                get { return _name; }
            }
            
            public List<string> Preconditions
            {
                get { return _preconditions; }
            }
            
            public Definition Reference
            {
                get { return _reference; }
            }
            
            public int CompareTo(DefinitionId other)
            {
                return _name.Equals(other._name) ?
                    
                	(_preconditions.Count == other._preconditions.Count ?
                	 
                	  string.Compare((GetKey(_name, _preconditions)), GetKey(other._name, other._preconditions), StringComparison.Ordinal) :
                     
                        (_preconditions.Count < other._preconditions.Count ? -1 : 1) ) :
                    
                            string.Compare(_name, other._name, StringComparison.Ordinal);
            }
        }
        
        private static readonly Dictionary<string, Definition> _table     = new Dictionary<string, Definition>();
        private static readonly SortedSet<DefinitionId>        _tree      = new SortedSet<DefinitionId>();
        private static readonly SortedStringSet                _key_names = new SortedStringSet();
        
        public static Dictionary<string, Definition> Table
        {
            get { return _table; }
        }
        
        public static SortedSet<DefinitionId> Tree
        {
            get { return _tree; }
        }
        
        public static SortedStringSet KeyNames
        {
        	get { return _key_names; }
        }
        
        public static string GetKey(string name, List<string> typenames)
        {
            string key = name;
            
            foreach (var typename in typenames)
            {
                key += "-" + typename;
            }
            
            return key;
        }
        
        public static string GetKey(string name, List<object> arguments)
        {
            var typenames = new List<string>();
            
            string key = name;
            
            foreach (var argument in arguments)
            {
                key += "-" + argument.GetType().Name;
            }
            
            return key;
        }
        
        public static IEnumerable<DefinitionId> GetOverloadTable(string name)
        {
            return from def in _tree where def.Name.Equals(name) select def;
        }
        
        public static IEnumerable<DefinitionId> GetOverloadTable(string name, int count)
        {
            return from def in _tree where def.Name.Equals(name) && def.Preconditions.Count == count select def;
        }
        
        private static bool Push(string name, List<string> typenames, Definition newDef)
        {
            string key = GetKey(name, typenames);
            
            if(_table.ContainsKey(key))
            {
                return false;
            }
            
            _table.Add(key, newDef);
            _tree.Add(new DefinitionId(name, typenames, newDef));
            _key_names.Add(name);
            
            return true;
        }
        
        public static bool Push(string path, List<string> page)
        {
            bool isUsablePath = false;
            
            List<string> typenames;
            
            string name = "", returnType = "";
            
            int lineStart;
            
            for (int i = 0; i < page.Count; ++i)
            {
                if (Operators.Verbs.defineprocess.Contains(Regex.Match(page[i], Patterns.getverb).Value))
                {
                    isUsablePath = false;
                    
                    lineStart = i;
                    
                    typenames = ParseHeading(page, ref i, ref name, ref returnType);
                    
                    if (Push(name, typenames, new Definition(path, lineStart, returnType)))
                    {
                        isUsablePath = true;
                    }
                    else
                    {
                        throw new Exception("Definition Conflict: A process is already defined for the key: '" + GetKey(name, typenames) + "'.");
                    }
                }
            }
            
            return isUsablePath;
        }
        
//        public static bool Push(string name, List<object> arguments, Definition newDef)
//        {
//            string key = GetKey(name, arguments);
//            
//            if(_table.ContainsKey(key))
//            {
//                return false;
//            }
//            
//            var typenames = new List<string>();
//            
//            foreach(var argument in arguments)
//            {
//                typenames.Add(argument.GetType().Name);
//            }
//            
//            _table.Add(key, newDef);
//            _tree.Add(new DefinitionId(name, typenames, newDef));
//            
//            return true;
//        }
        
        private static int PreconditionMatches(IEnumerable<object> arguments, IEnumerable<string> typenames)
        {
            int matches = 0;
            
            var argumentIt = arguments.GetEnumerator();
            var typenameIt = typenames.GetEnumerator();
            
            while(argumentIt.MoveNext() && typenameIt.MoveNext())
            {
                if(argumentIt.Current.Equals(typenameIt.Current))
                {
                    matches = matches + 1;
                }
            }
            
            return matches;
        }
        
        private static List<Definition> BestMatchingOverloads(IEnumerable<DefinitionId> overloads, IEnumerable<object> arguments)
        {
            int bestNumber  = 0;
            var bestMatches = new List<Definition>();
            int matches;
            
            foreach (var overload in overloads)
            {
                matches = PreconditionMatches(arguments, overload.Preconditions);
                
                if(matches == bestNumber)
                {
                    bestMatches.Add(overload.Reference);
                }
                else if(matches > bestNumber)
                {
                    bestMatches.Clear();
                    bestMatches.Add(overload.Reference);
                    bestNumber = matches;
                }
            }
            
            return bestMatches;
        }
        
        public static Definition Get(string name, List<object> arguments)
        {
            try
            {
                return _table[GetKey(name, arguments)];
            }
            catch(KeyNotFoundException)
            {
                var bestMatches = BestMatchingOverloads(GetOverloadTable(name, arguments.Count), arguments);
                
                switch(bestMatches.Count)
                {
                    case 0  : throw new Exception("An overload of the name '" + name + "' is not defined for the given arguments: " + GetKey(name, arguments));
                    
                    case 1  : return bestMatches.First();
                    
                    default : throw new Exception("The overload of the name '" + name + "' is ambiguous with the given arguments: " + GetKey(name, arguments));
                }
            }
        }
        
        public static object Call(Interpreter.Sequence sender, string name, List<object> arguments)
        {
            Definition newProcess = Get(name, arguments);
            
            int lineNumber = newProcess.LineNumber;
            
            List<string> script = Transcript.Table[newProcess.Path];
            
            int startingLvl = Interpreter.Sequence.GetLevel(script[lineNumber]);
            
            List<Parameter> parameters = ParseHeading(script, ref lineNumber);
            
            var receiver = new Interpreter.Sequence();
            
        	Dictionary<string, Named> table;
        	
            for (int i = 0; i < parameters.Count; ++i)
            {
            	if (parameters[i].referential)
            	{
	            	try
	            	{
	            		table = sender.Names.GetTable((string) arguments[i]);
	            	}
		            catch (Exception)
		            {
		            	throw new Exception("Failed to get a reference for a bidirectional parameter. Are you missing a 'ref' keyword?");
		            }
	            }
            	else
            	{
            		table = null;
            	}
            	
                Verbs.Declare( receiver,
                               parameters[i].key,
                               parameters[i].key,
                               parameters[i].readable,
                               parameters[i].writeable,
                               arguments[i],
                               table,
                               parameters[i].typename );
            }
            
            try
            {
                while (lineNumber < script.Count)
                {
                    if (String.IsNullOrWhiteSpace(script[lineNumber]))
                    {
                        lineNumber = lineNumber + 1;
                    }
                    else if (Interpreter.Sequence.GetLevel(script[lineNumber]) > startingLvl)
                    {
                        lineNumber = receiver.Execute(script, lineNumber);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Return r)
            {
                if (Operators.returnvoid.Contains(newProcess.ReturnType))
                {
                    if (r.Payload == null)
                    {
                        throw new Exception("Line " + (lineNumber + 1) + ": Syntax Error: Returning 'null' from a process that returns void.");
                    }
                    
                    throw new Exception("Line " + (lineNumber + 1) + ": Syntax Error: Returning a '" + r.Payload.GetType().Name + "' from a process that returns void.");
                }
                
                if (r.Payload != null && !r.Payload.GetType().Name.Equals(newProcess.ReturnType))
                {
                    throw new Exception("Line " + (lineNumber + 1) + ": Type Mismatch: Returning a '" + r.Payload.GetType().Name + "' from a process that returns '" + newProcess.ReturnType + "'.");
                }
                
                return r.Payload;
            }
            
            if (!Operators.returnvoid.Contains(newProcess.ReturnType))
            {
                throw new Exception("Line " + (newProcess.LineNumber + 1) + ": Syntax Error: No return at the end of non-void returning process.");
            }
            
            return null;
        }
        
        public struct Parameter
        {
            public bool readable;
            public bool writeable;
            public bool referential;
            
            public string key;
            public string typename;
            public string expression;
            
            public Parameter( bool   isReadable,  bool   isWriteable,      bool   isReferential,
                              string keyProperty, string typenameProperty, string expressionProperty )
            {
                readable    = isReadable;
                writeable   = isWriteable;
                referential = isReferential;
                key         = keyProperty;
                typename    = typenameProperty;
                expression  = expressionProperty;
            }
            
            public Parameter(bool isReferential, string declaration): this()
            {
                string op = "";
                
                ParseDeclaration
                (
                    declaration,
                    ref key, ref readable, ref writeable,
                    ref op, ref typename, ref expression
                );
                
                Verbs.VerifyRightToLeftOperator(op);
                
                referential = isReferential;
            }
            
            public Parameter(string inOrOut, string declaration):
                this(ParameterOrientation(inOrOut), declaration) {}
        }
        
        public static List<Parameter> ParseHeading(IList<string> queue, ref int start)
        {
            var parameters = new List<Parameter>();
            
            string verb = null, complement = null;
            
            try
            {
                switch (Regex.Match(queue[start], Patterns.punctuation).Value)
                {
                    case ":" :
                    {
                        bool isOutParameter;
                        
                        try
                        {
                            Verbs.SplitSentence(queue[start + 1], ref verb, ref complement);
                            
                            isOutParameter = ParameterOrientation(verb);
                            
                            while (Operators.parameterOrientations.Contains(verb))
                            {
                                start = start + 1;
                                
                                foreach (string declaration in complement.Split(','))
                                {
                                    if (!String.IsNullOrWhiteSpace(declaration))
                                    {
                                        parameters.Add(new Parameter(isOutParameter, declaration));
                                    }
                                }
                                
                                if (Regex.IsMatch(queue[start], ",\\s*$"))
                                {
                                    complement = queue[start + 1];
                                }
                                else
                                {
                                    Verbs.SplitSentence(queue[start + 1], ref verb, ref complement);
                                    
                                    isOutParameter = ParameterOrientation(verb);
                                }
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            throw new Exception("Bug: Attempting to parse an unfinished definition.");
                        }
                    }
                    
                    break;
                    
                    case "(" :
                    {
                        foreach (string declaration in 
                                 
                                 Regex.Match(queue[start], Patterns.getdefparams).Value.Split(','))
                        {
                            if (!String.IsNullOrWhiteSpace(declaration))
                            {
                                Verbs.SplitSentence(declaration, ref verb, ref complement);
                                
                                if (Operators.parameterOrientations.Contains(verb))
                                {
                                    parameters.Add(new Parameter(verb, complement));
                                }
                                else
                                {
                                    parameters.Add(new Parameter(false, declaration));
                                }
                            }
                        }
                    }
                    
                    break;
                }
            }
            catch (Exception e)
            {
                throw new Exception(CommandLineInterface.GetErrorMessageWithLineNumberAndHeading(start, "Syntax Error", e));
            }
            
            start = start + 1;
            
            return parameters;
        }
        
        public static List<string> ParseHeading(IList<string> queue, ref int start, ref string name, ref string returnType)
        {
            var typenames = new List<string>();
            
            string verb = null, complement = null;
            
            Match typenameCapture;
            
            name = Regex.Match(queue[start], Patterns.getdefname).Value;
            
            returnType = Regex.Match
            (
                queue[start],
                Patterns.gettypename,
                RegexOptions.RightToLeft
            )
            .Value;
            
            switch (Regex.Match(queue[start], Patterns.punctuation).Value)
            {
                case ":" :
                {
                    try
                    {
                        Verbs.SplitSentence(queue[start + 1], ref verb, ref complement);
                        
                        while (Operators.parameterOrientations.Contains(verb))
                        {
                            start = start + 1;
                            
                            foreach (string declaration in complement.Split(','))
                            {
                                if (!String.IsNullOrWhiteSpace(declaration))
                                {
                                    typenameCapture = Regex.Match(declaration, Patterns.gettypename);
                                    
                                    typenames.Add(typenameCapture.Success ? typenameCapture.Value : Definition.DEFAULT_TYPENAME);
                                }
                            }
                            
                            if (Regex.IsMatch(queue[start], ",\\s*$"))
                            {
                                complement = queue[start + 1];
                            }
                            else
                            {
                                Verbs.SplitSentence(queue[start + 1], ref verb, ref complement);
                            }
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        throw new Exception("Bug: Attempting to parse an unfinished definition.");
                    }
                }
                
                break;
                
                case "(" :
                {
                    foreach (string declaration in 
                             
                             Regex.Match(queue[start], Patterns.getdefparams).Value.Split(','))
                    {
                        if (!String.IsNullOrWhiteSpace(declaration))
                        {
                            typenameCapture = Regex.Match(declaration, Patterns.gettypename);
                            
                            typenames.Add(typenameCapture.Success ? typenameCapture.Value : Definition.DEFAULT_TYPENAME);
                        }
                    }
                }
                
                break;
            }
            
            start = start + 1;
            
            return typenames;
        }
        
//        public static Parameter.Orientation ParameterOrientation(string word)
//        {
//            switch (word)
//            {
//                case "in"  :
//                case "In"  : return Parameter.Orientation.In;
//                case "out" :
//                case "Out" : return Parameter.Orientation.Out;
//                default    : return Parameter.Orientation.InvalidParameter;
//            }
//        }
        
        public static bool ParameterOrientation(string word)
        {
            switch (word)
            {
                case "out" :
                case "Out" : return true;
                default : return false;
            }
        }
        
        public static void ParseDeclaration
		    
        		    ( string line,
        		      ref string key,
        		      ref bool read,
        		      ref bool write,
        		      ref string op,
        		      ref string typename,
        		      ref string expression )
		{
		    string left = "", right = "";
		    
			// SET  : <nil>
			// line : "const name of type string := \"Adam\""
			
			Verbs.Declarations.GetPermissionProperties(ref line, ref left, ref right, ref read, ref write);
			
			// SET  :  read, write
			// line : "name of type string := \"Adam\""
			
			Verbs.Declarations.GetPayloadProperty(ref line, ref left, ref op, ref right, ref expression);
			
			// SET  :  read, write, payload
			// line : "name of type string"
			
			string temp = "";
			
			Verbs.Declarations.GetTypenameProperty(ref line, ref left, ref temp, ref right, ref typename);
			
			// SET  :  read, write, payload, typename
			// line : "name"
			
		    Match match = Regex.Match(line, Patterns.getnewname);
		    
		    if (!match.Success)
		        
		        throw new FormatException("No valid variable name in the phrase: \"" + line.Trim() + "\".");
		    
			key = match.Value;
			
			// SET  : read, write, referential, payload, typename, key
		}
		
		public static string TableToString()
		{
		    string str = "";
		    
		    foreach (var key in from every_key in _table.Keys orderby every_key select every_key)
		    {
		    	str += "  " + key + " : " + _table[key]._return_type + "\n";
		    }
		    
		    return str;
		    
//		    return Algorithms.Collections.ToString(_table);
		}
		
		public static void Clear()
		{
		    _table.Clear();
		    _tree.Clear();
		}
    }
}













