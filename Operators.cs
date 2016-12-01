/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/8/2016
 * Time: 12:06 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerWalk
{
    /// <summary>
    /// Description of Operators.
    /// </summary>
    
    public static class Operators
    {
		public static readonly string comment       = "#";
		public static readonly string linecontinue  = "...";
		
		/*****************************
		 * --- NAMED VALUE TYPES --- *
		 *****************************/
		
		public static readonly SortedStringSet readonlyvalue   = new SortedStringSet(new []{ "const", "constant", "readonly" });
		public static readonly SortedStringSet writeonlyvalue  = new SortedStringSet(new []{ "writeonly" });
		public static readonly SortedStringSet readwritevalue  = new SortedStringSet(new []{ "readwrite" });
		public static readonly SortedStringSet bidirectional   = new SortedStringSet(new []{ "ref", "reference" });
		public static readonly SortedStringSet statictype      = new SortedStringSet(new []{ ":", "as", "of type" });
		public static readonly SortedStringSet inparameter     = new SortedStringSet(new []{ "in" });
		public static readonly SortedStringSet outparameter    = new SortedStringSet(new []{ "out" });
		
        /****************************
         * --- BINARY OPERATORS --- *
         ****************************/
        
        public static readonly SortedStringSet or              = new SortedStringSet(new []{ "|", "or" });
        public static readonly SortedStringSet and             = new SortedStringSet(new []{ "&", "and" });
        public static readonly SortedStringSet equal           = new SortedStringSet(new []{ "=", "equal", "is" });
        public static readonly SortedStringSet notequal        = new SortedStringSet(new []{ "!=", "is not", "not equal" });
        public static readonly SortedStringSet lessthan        = new SortedStringSet(new []{ "<", "lessthan" });
        public static readonly SortedStringSet greaterthan     = new SortedStringSet(new []{ ">", "greaterthan" });
        public static readonly SortedStringSet lessthanOrEqual = new SortedStringSet(new []{ "<=" });
        public static readonly SortedStringSet grtrthanOrEqual = new SortedStringSet(new []{ ">=" });
        public static readonly SortedStringSet within          = new SortedStringSet(new []{ "in" });
        public static readonly SortedStringSet without         = new SortedStringSet(new []{ "not in" });
        public static readonly SortedStringSet contain         = new SortedStringSet(new []{ "contain" });
        public static readonly SortedStringSet notcontain      = new SortedStringSet(new []{ "not contain" });
        public static readonly SortedStringSet match           = new SortedStringSet(new []{ "match" });
        public static readonly SortedStringSet notmatch        = new SortedStringSet(new []{ "not match" });
        public static readonly SortedStringSet plus            = new SortedStringSet(new []{ "+", "plus" });
        public static readonly SortedStringSet minus           = new SortedStringSet(new []{ "-", "minus" });
        public static readonly SortedStringSet by              = new SortedStringSet(new []{ "*", "by" });
        public static readonly SortedStringSet over            = new SortedStringSet(new []{ "/", "over" });
        public static readonly SortedStringSet mod             = new SortedStringSet(new []{ "%", "mod" });
		
        /*****************************************
         * --- COMMAND-LINE BINARY OPERATORS --- *
         *****************************************/
        
//        public static readonly string pipeline        = "|";
//        public static readonly string redirectclobber = ">";
//        public static readonly string redirectappend  = ">>";
        
        /***************************
         * --- UNARY OPERATORS --- *
         ***************************/
        
        public static readonly SortedStringSet negative        = new SortedStringSet(new []{ "!", "not" });
        public static readonly SortedStringSet opposite        = new SortedStringSet(new []{ "-" });
        
        /**********************
         * --- KEY VALUES --- *
         **********************/
        
        public static readonly SortedStringSet confirm         = new SortedStringSet(new []{ "true", "verily" }); // IT IS SO (?)
        public static readonly SortedStringSet deny            = new SortedStringSet(new []{ "false", "inverily" });
        public static readonly SortedStringSet decline         = new SortedStringSet(new []{ "nil", "none", "null", "void", "naught" });
        public static readonly SortedStringSet returnvoid      = new SortedStringSet(new []{ "void" });
        
        /*****************
         * --- VERBS --- *
         *****************/
        
        public static class Verbs
        {
            public static readonly SortedStringSet declare       = new SortedStringSet(new []{ "declare", "init", "initialize" });
            public static readonly SortedStringSet assign        = new SortedStringSet(new []{ "assign", "set" }); // calc, calculate
            public static readonly SortedStringSet clear         = new SortedStringSet(new []{ "clear", "unset" });
            public static readonly SortedStringSet release       = new SortedStringSet(new []{ "release", "del", "delete" });
            public static readonly SortedStringSet increment     = new SortedStringSet(new []{ "incr", "increment" });
            public static readonly SortedStringSet decrement     = new SortedStringSet(new []{ "decr", "decrement" });
            public static readonly SortedStringSet addto         = new SortedStringSet(new []{ "add" });
            public static readonly SortedStringSet subtractfrom  = new SortedStringSet(new []{ "subtr", "subtract" });
            public static readonly SortedStringSet multiplyby    = new SortedStringSet(new []{ "mult", "multiply" });
            public static readonly SortedStringSet divideby      = new SortedStringSet(new []{ "div", "divide" });
            public static readonly SortedStringSet remainderfrom = new SortedStringSet(new []{ "mod", "remainder" });
            public static readonly SortedStringSet input         = new SortedStringSet(new []{ "in", "input", "read", "readhost" });
            public static readonly SortedStringSet inputline     = new SortedStringSet(new []{ "inputline", "readline" });
            public static readonly SortedStringSet output        = new SortedStringSet(new []{ "out", "output", "write", "writehost", "print" });
            public static readonly SortedStringSet outputline    = new SortedStringSet(new []{ "outputline", "writeline", "printline" });
            public static readonly SortedStringSet error         = new SortedStringSet(new []{ "err", "error", "writerror" });
            public static readonly SortedStringSet errorline     = new SortedStringSet(new []{ "errorline", "writerrorline" });
            public static readonly SortedStringSet returnvalue   = new SortedStringSet(new []{ "return" });
            public static readonly SortedStringSet callprocess   = new SortedStringSet(new []{ "call" });
            public static readonly SortedStringSet defineprocess = new SortedStringSet(new []{ "def", "define" });
            
        	public static readonly SortedStringSet Set = GetVerbs();
        	
            public static class Commands
            {
                public const string include   = "include";
                public const string show      = "show";
                public const string load      = "load";
                public const string reset     = "reset";
                public const string reload    = "reload";
                public const string force     = "force";
                public const string current   = "current";
                public const string terminate = "exit";
                public const string push      = "push";
                public const string pop       = "pop";
                
                public static readonly SortedStringSet clearscreen = new SortedStringSet(new []{ "cls", "clearscreen" });
                
                public static readonly SortedStringSet gethelp = new SortedStringSet(new []{ "help", "-help", "?", "-?", "/?" });
                
                
            	public static readonly SortedStringSet Set = GetCommands();
            	
                
			    public static SortedStringSet GetCommands()
			    {
			        var commands = new SortedStringSet();
			        
			        commands.Add(include);
			        commands.Add(show);
			        commands.Add(load);
			        commands.Add(reset);
			        commands.Add(reload);
			        commands.Add(terminate);
			        commands.Add(push);
			        commands.Add(pop);
			        
			        foreach (var command in clearscreen.ToList())
			        {
			        	commands.Add(command);
			        }
			        
			        foreach (var command in gethelp.ToList())
			        {
			        	commands.Add(command);
			        }
			        
			        return commands;
			    }
            }
            
		    public static SortedStringSet GetVerbs()
		    {
		        var commands = new SortedStringSet();
		        
		        foreach (var verb in Verbs.declare.ToList())       commands.Add(verb);
		        foreach (var verb in Verbs.assign.ToList())        commands.Add(verb);
		        foreach (var verb in Verbs.clear.ToList())         commands.Add(verb);
		        foreach (var verb in Verbs.release.ToList())       commands.Add(verb);
		        foreach (var verb in Verbs.increment.ToList())     commands.Add(verb);
		        foreach (var verb in Verbs.decrement.ToList())     commands.Add(verb);
		        foreach (var verb in Verbs.addto.ToList())         commands.Add(verb);
		        foreach (var verb in Verbs.subtractfrom.ToList())  commands.Add(verb);
		        foreach (var verb in Verbs.multiplyby.ToList())    commands.Add(verb);
		        foreach (var verb in Verbs.divideby.ToList())      commands.Add(verb);
		        foreach (var verb in Verbs.remainderfrom.ToList()) commands.Add(verb);
		        foreach (var verb in Verbs.input.ToList())         commands.Add(verb);
		        foreach (var verb in Verbs.inputline.ToList())     commands.Add(verb);
		        foreach (var verb in Verbs.output.ToList())        commands.Add(verb);
		        foreach (var verb in Verbs.outputline.ToList())    commands.Add(verb);
		        foreach (var verb in Verbs.error.ToList())         commands.Add(verb);
		        foreach (var verb in Verbs.errorline.ToList())     commands.Add(verb);
		        foreach (var verb in Verbs.returnvalue.ToList())   commands.Add(verb);
		        foreach (var verb in Verbs.callprocess.ToList())   commands.Add(verb);
				
		        return commands;
		    }
        }
        
        public static readonly SortedStringSet   right_to_left = new SortedStringSet(new []{ ":=", "<-", "on" });
        public static readonly SortedStringSet   left_to_right = new SortedStringSet(new []{ "=:", "->", "into" });
        
        public static readonly string novelty_declare = "on";
        
		public static readonly SortedStringSet   unops                 = GetUnaryOperators();
		public static readonly SortedStringSet[] binops                = GetBinaryOperators();
		public static readonly SortedStringSet   assignops             = GetAssignmentOperators();
		public static readonly SortedStringSet   permissionops         = GetPermissionOperators();
		public static readonly SortedStringSet   parameterOrientations = GetParameterOrientations();
		
		public static readonly Dictionary<string, object> keyValues = GetKeyValues();
        
        // clear     : clear, reset
        // release   : release, remove, delete, unset
        // increment : incr, increment
        // decrement : decr, decrement
        // addto     : addto
        // add       : add                                   needs operator: to
        // subtractfrom : subtractfrom, subtrfrom, subtr
        // subtract  : subtract                              needs operator: from
        // multiply  : mult, multiply                        needs operator: by
        // divide    : div, divide                           needs operator: by
        // mod       : mod                                   needs operator: by
        
        public const int LEVELS = 5;
        
        public enum OperatorLevel
        {
            BOOLEAN_OR,
            BOOLEAN_AND,
            RELATIONAL,
            ADDITIVE,
            MULTIPLICATIVE
        }
        
        public static SortedStringSet[] GetBinaryOperators()
        {
            var operators = new SortedStringSet[LEVELS];
            
            operators[(int) OperatorLevel.BOOLEAN_OR]      = (SortedStringSet) or;
            operators[(int) OperatorLevel.BOOLEAN_AND]     = (SortedStringSet) and;
            operators[(int) OperatorLevel.RELATIONAL]      = new SortedStringSet();
            operators[(int) OperatorLevel.ADDITIVE]        = new SortedStringSet();
            operators[(int) OperatorLevel.MULTIPLICATIVE]  = new SortedStringSet();
            
            foreach
            (
                SortedStringSet subset in new SortedStringSet[]
                {
                    equal, notequal, lessthan, greaterthan, lessthanOrEqual, grtrthanOrEqual,
                    within, without, contain, notcontain, match, notmatch
                }
            )
                foreach (string op in subset.ToList()) operators[(int) OperatorLevel.RELATIONAL].Add(op);
            
            foreach (SortedStringSet subset in new SortedStringSet[]{plus, minus})
            	
                foreach (string op in subset.ToList()) operators[(int) OperatorLevel.ADDITIVE].Add(op);
            
            foreach (SortedStringSet subset in new SortedStringSet[]{by, over, mod})
            	
                foreach (string op in subset.ToList()) operators[(int) OperatorLevel.MULTIPLICATIVE].Add(op);
            
            return operators;
        }
        
        public static SortedStringSet GetUnaryOperators()
        {
            var operators = new SortedStringSet();
            
            foreach (string op in negative.ToList())
                
                operators.Add(op);
            
            foreach (string op in opposite.ToList())
                
                operators.Add(op);
            
            foreach (string op in bidirectional.ToList())
                
                operators.Add(op);
            
            return operators;
        }
        
        public static SortedStringSet GetAssignmentOperators()
        {
            var operators = new SortedStringSet();
            
            foreach (string op in right_to_left.ToList())
            
                operators.Add(op);
                
            foreach (string op in left_to_right.ToList())
            
                operators.Add(op);
                
            return operators;
        }
        
        public static SortedStringSet GetPermissionOperators()
        {
            var operators = new SortedStringSet();
            
            foreach (string op in readonlyvalue.ToList())
                
                operators.Add(op);
            
            foreach (string op in writeonlyvalue.ToList())
                
                operators.Add(op);
            
            foreach (string op in readwritevalue.ToList())
                
                operators.Add(op);
            
            return operators;
        }
        
        public static SortedStringSet GetParameterOrientations()
        {
            var operators = new SortedStringSet();
            
            foreach (string op in inparameter.ToList())
                
                operators.Add(op);
            
            foreach (string op in outparameter.ToList())
                
                operators.Add(op);
            
            return operators;
        }
        
        public static Dictionary<string, object> GetKeyValues()
        {
        	var operators = new Dictionary<string, object>();
        	
        	foreach (string op in confirm.ToList())
        		
        		operators.Add(op, true);
        	
        	foreach (string op in deny.ToList())
        		
        		operators.Add(op, false);
        	
        	foreach (string op in decline.ToList())
        		
        		operators.Add(op, null);
        	
        	return operators;
        }
    }
}
