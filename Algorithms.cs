/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/11/2016
 * Time: 1:42 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Algorithms
{
    /// <summary>
    /// Description of Arithmetic.
    /// </summary>
    
    public static class Collections
    {
        public static bool Contains(object left, object right)
        {
            if (left as string != null)
            {
                if (right is char)
                {
                    return ((string) left).Contains("" + (char) right);
                }
                
                return ((string) left).Contains((string) right);
            }
            
            if (left as IList != null)
            {
                return ((IList) left).Contains(right);
            }
            
            if (left as SortedSet<object> != null)
            {
                return ((SortedSet<object>) left).Contains(right);
            }
            
            if (left as Interval != null)
            {
            	return ((Interval) left).Contains((IComparable) right);
            }
            
            throw new ArgumentException("The aggregate operator could not be applied to the given types: "
                                        + left.GetType().Name + ", " + right.GetType().Name + ".");
        }
        
        public static string ToString<V>(IDictionary<string, V> table)
        {
		    string str = "";
		    
		    foreach (var key in from every_key in table.Keys orderby every_key select every_key)
		    {
		        str += "  " + key + "\n";
		    }
		    
		    return str;
        }
    }
    
    public static class Arithmetic
    {
		public static object Sum(object left, object right)
        {
		    if (right.GetType().Name == "String")
		    {
		        return left + (string) right;
		    }
		    
            switch (left.GetType().Name)
            {
				case "ArrayList":
				{
					var superList = new ArrayList((ICollection) left);
				    superList.AddRange((ICollection) right);
					return superList;
				}
                case "String":
				{
					return (string) left + right;
				}
				case "Char":
				{
					switch (right.GetType().Name)
					{
						case "Int64": return (char) (Convert.ToInt32((char) left) + (int) (Int64) right);
					}
					break;
				}
                case "Int64":
                {
                    switch (right.GetType().Name)
                    {
                        case "Int64"  : return (Int64) left + (Int64)  right;
                        case "Double" : return (Int64) left + (Double) right;
                    }
                    break;
                }
                case "Double":
                {
                    switch (right.GetType().Name)
                    {
                        case "Int64"  : return (Double) left + (Int64)  right;
                        case "Double" : return (Double) left + (Double) right;
                    }
                    break;
                }
            }
            
            throw new ArgumentException("Cannot add a " + right.GetType().Name + " to a " + left.GetType() + ".");
        }

        public static object Difference(object left, object right)
        {
            switch (left.GetType().Name)
            {
				case "Char":
				{
					switch (right.GetType().Name)
					{
						case "Int64": return (char) (Convert.ToInt32((char) left) - (int) (Int64) right);
					}
					break;
				}
                case "Int64":
                {
                    switch (right.GetType().Name)
                    {
                        case "Int64"  : return (Int64) left - (Int64)  right;
                        case "Double" : return (Int64) left - (Double) right;
                    }
                    break;
                }
                case "Double":
                {
                    switch (right.GetType().Name)
                    {
                        case "Int64"  : return (Double) left - (Int64)  right;
                        case "Double" : return (Double) left - (Double) right;
                    }
                    break;
                }
            }

            throw new ArgumentException("Cannot subtract a " + right.GetType().Name + " from a " + left.GetType() + ".");
        }

        public static object Product(object left, object right)
        {
            switch (left.GetType().Name)
            {
                case "Int64":
                {
                    switch (right.GetType().Name)
                    {
                        case "Int64"  : return (Int64) left * (Int64)  right;
                        case "Double" : return (Int64) left * (Double) right;
                    }
                    break;
                }
                case "Double":
                {
                    switch (right.GetType().Name)
                    {
                        case "Int64"  : return (Double) left * (Int64)  right;
                        case "Double" : return (Double) left * (Double) right;
                    }
                    break;
                }
            }

            throw new ArgumentException("Cannot multiply a " + left.GetType().Name + " by a " + right.GetType() + ".");
        }

        public static object Quotient(object left, object right)
        {
            switch (left.GetType().Name)
            {
                case "Int64":
                {
                    switch (right.GetType().Name)
                    {
                        case "Int64"  : return (Int64) left / (Int64)  right;
                        case "Double" : return (Int64) left / (Double) right;
                    }
                    break;
                }
                case "Double":
                {
                    switch (right.GetType().Name)
                    {
                        case "Int64"  : return (Double) left / (Int64)  right;
                        case "Double" : return (Double) left / (Double) right;
                    }
                    break;
                }
            }

            throw new ArgumentException("Cannot divide a " + left.GetType().Name + " by a " + right.GetType() + ".");
        }

        public static object Remainder(object left, object right)
        {
            switch (left.GetType().Name)
            {
                case "Int64":
                {
                    switch (right.GetType().Name)
                    {
                        case "Int64"  : return (Int64) left % (Int64) right;
                        case "Double" : throw new Exception("TypeMismatchException");
                    }
                    break;
                }
            }

            throw new ArgumentException("Cannot mod a " + left.GetType().Name + " by a " + right.GetType() + ".");
        }
    }
}
