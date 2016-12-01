/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/18/2016
 * Time: 1:18 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;

namespace PowerWalk
{
    /// <summary>
    /// Description of Stream.
    /// </summary>
    
    public class Stream
    {
        private readonly StringBuilder builder;
        
        public Stream(string source)
        {
            builder = new StringBuilder(source);
        }
        
        public Stream()
        {
            builder = new StringBuilder();
        }
        
        public void Write(string str)
        {
            builder.Append(str);
        }
        
        public void Write(object content)
        {
            Write(content.ToString());
        }
        
        public void WriteLine(string str)
        {
            Write(str);
            Write('\n');
        }
        
        public void WriteLine(object content)
        {
            WriteLine(content.ToString());
        }
        
        public int Length
        {
            get { return builder.Length; }
        }
        
        public int WhiteSpaceStart
        {
            get
            {
                int index = 0;
                
                while (index < builder.Length && !Char.IsWhiteSpace(builder[index]))
                    index = index + 1;
                
                return index;
            }
        }
        
        public int WhiteSpaceLength
        {
            get
            {
                int index = 0;
                
                while (index < builder.Length && Char.IsWhiteSpace(builder[index]))
                    index = index + 1;
                
                return index + 1;
            }
        }
        
        public int LineLength
        {
            get
            {
                int index = 0;
                
                while (index < builder.Length && builder[index] == '\n')
                    index = index + 1;
                
                return index + 1;
            }
        }
        
        public bool Any()
        {
            return Length > 0;
        }
        
        public string Read()
        {
            builder.Remove(0, WhiteSpaceLength);
            
            var word = new char[WhiteSpaceStart + 1];
            
            builder.CopyTo(0, word, 0, WhiteSpaceStart + 1);
                    
            builder.Remove(0, WhiteSpaceStart + 1);
            
            return new string(word);
        }
        
        public string ReadLine()
        {
            var line = new char[LineLength];
            
            builder.CopyTo(0, line, 0, LineLength);
            
            builder.Remove(0, LineLength);
            
            return new string(line).TrimEnd('\n');
        }
        
        public Stream Clear()
        {
            builder.Clear();
            
            return this;
        }
        
        public override string ToString()
        {
            return builder.ToString();
//            return string.Format("[Stream Builder={0}]", builder);
        }
 
    }
    
    
    
    
    
    
    
    
    
    
    
    
    
}
