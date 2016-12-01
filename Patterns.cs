/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/14/2016
 * Time: 3:54 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PowerWalk
{
    /// <summary>
    /// Description of Patterns.
    /// </summary>
    
    public static class Patterns
    {
//		  public static string word         = "\\w\\s_";
//		  public static string opSymbol     = "((?<=[{word}])[^{word}]{1,2}(?=[{word}]))";
//		  public static string opWord       = "((?<=\\s)((is not)|((not )?[A-Za-z]+))(?=\\s))";
//		  public static string binopPattern = "{opSymbol}|{opWord}";
    		
//        const string forceps = "^\\s*for\\s+((every\\s+(?<stp>\\S+)\\s+)|(each\\s+)?)(?<nme>\\S+)\\s+in\\s+(?<cll>.+)$";
        
        public const string punctuation       = "[^\\s\\w]";
        
		public const string getunaryop        = "^(\\W(?=\\s*))|([A-Za-z]+(?=\\s+))";
		public const string getbinaryop       = "((:|\\!|\\<|\\>)?[^\\w\\s_](:|\\>)?)|((?<=\\s)(of\\s?type)|((is\\s+not)|((not\\s+)?[A-Za-z]+))(?=\\s))";
		public const string getaggregateop    = "\\s+in\\s+";
		public const string getlistdelimiter  = "\\s*,\\s*";
		
		public const string namedvalue        = "([A-Za-z]|_)(\\w|_)*";
		
		public const string getverb           = "(?<=^|\\s+)\\w+(?=[^\\w_]|$)";
		public const string getcommand        = "(?<=^|\\s+)\\w+(?=\\s+|$)";
		public const string getnewname        = "(?<=^\\s*)" + namedvalue + "(?=\\s*$)";
		public const string getname           = "^" + namedvalue + "(?=\\s*$)";
		public const string getargs           = "(?<=^\\s*\\().*(?=\\)\\s*$)";
		public const string getstringliteral  = "(?<=^\")[^\"]*(?=\"$)";
		public const string getgroup          = "^(\\(|\\[).*(\\]|\\))$";
		public const string getlist           = "(?<=^\\{).*(?=\\}$)";
		public const string getmethodargs     = "(?<=^\\().*(?=\\)$)";
		public const string getsubscript      = "(?<=^\\[).*(?=\\]$)";
		public const string getcharacter      = "(?s)(?<=^').(?='$)";
		public const string getinteger        = "^-?\\d+$";
		public const string getfloatpointval  = "^-?\\d*\\.\\d+$";
		
		public const string getwhile          = "(?<=^\\s*while\\s*).*";
		public const string getif             = "(?<=^\\s*(else?\\s*)?if\\s*).*";
		public const string getfor            = "^\\s*for(\\s+each)?(?=\\s+)";
		public const string getloopcontroller = "(?<=^|\\s+)" + namedvalue;
		public const string getloopstep       = "(?<=^every\\s+).*\\S(?=\\s*$)";
		
		public const string typename          = namedvalue + "(\\." + namedvalue + ")*";
		
		public const string gettypename       = "(?<=:\\s*)" + typename;
		public const string getreturntype     = gettypename + "(?<=\\s*$)";
		public const string getdefname        = "(?<=\\s)" + namedvalue;
		public const string getdefparams      = "(?<=^[^\\(]*\\().*(?=\\)[^\\)]*$)";
		
		public const string linenumber        = "^Line \\d+";
    }
}
