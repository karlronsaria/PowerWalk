/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/7/2016
 * Time: 5:24 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Collections
{
    /// <summary>
    /// Description of Interval.
    /// </summary>

	public enum Boundary    { OPEN, CLOSED }
	public enum Enumeration { POSITIVE, NEGATIVE }
	
	public class Interval : IEnumerable
	{
		private readonly IComparable top;
		private readonly IComparable bottom;
		
		private readonly Boundary upper;
		private readonly Boundary lower;
		
		public IComparable Top
		{
			get { return top; }
		}
		
		public IComparable Bottom
		{
			get { return bottom; }
		}
		
		public Boundary Upper
		{
			get { return upper; }
		}
		
		public Boundary Lower
		{
			get { return lower; }
		}
		
		public IComparable EnumeratorStepSize { get; set; }
		
		public Enumeration EnumeratorOrientation { get; set; }
		
		public Interval( IComparable firstValue, Boundary firstBound,
		                 IComparable secndValue, Boundary secndBound,
		                 IComparable step )
		{
			if (firstValue.CompareTo(secndValue) < 0)
			{
				bottom = firstValue;
				lower  = firstBound;
				
				top    = secndValue;
				upper  = secndBound;
				
				EnumeratorOrientation = Enumeration.POSITIVE;
			}
			else
			{
				bottom = secndValue;
				lower  = secndBound;
				
				top    = firstValue;
				upper  = firstBound;
				
				EnumeratorOrientation = Enumeration.NEGATIVE;
			}
			
			EnumeratorStepSize = step;
		}
		
		public Interval( IComparable firstValue, Boundary firstBound,
		                 IComparable secndValue, Boundary secndBound ) :
		    
		      this(firstValue, firstBound, secndValue, secndBound, 1L) {}
		
		public bool Contains(IComparable value)
		{
			return
			(
				(
					(
						lower == Boundary.OPEN
						&&
						value.CompareTo(bottom) > 0
					)
					||
					(
						lower == Boundary.CLOSED
						&&
						value.CompareTo(bottom) >= 0
					)
				)
				&&
				(
					(
						upper == Boundary.OPEN
						&&
						value.CompareTo(top) < 0
					)
					||
					(
						upper == Boundary.CLOSED
						&&
						value.CompareTo(top) <= 0
					)
				)
			);
		}
		
		public IComparable Start()
		{
		    IComparable startValue = EnumeratorOrientation ==
		        
		        Enumeration.POSITIVE ? (lower == Boundary.CLOSED ? bottom
		                                
		    	                        : (IComparable) Algorithms.Arithmetic.Sum(bottom, EnumeratorStepSize))
		        
		                             : (upper == Boundary.CLOSED ? top
		           
		    	   					    : (IComparable) Algorithms.Arithmetic.Difference(top, EnumeratorStepSize));
		    
		    return Contains(startValue) ? startValue : null;
		}
		
		public IEnumerator GetEnumerator()
		{
		    var e = new Enumerator();
		    
		    e.payload     = this;
		    e.step        = EnumeratorStepSize;
		    e.orientation = EnumeratorOrientation;
		    e.current     = Start();
		    
		    return e;
		}
		
		public class Enumerator : IEnumerator
		{
		    internal delegate bool Action();
		    
		    internal Interval     payload;
		    internal IComparable  current;
		    
		    internal IComparable  step;
		    internal Enumeration  orientation;
		    
		    internal Action       MoveAction;
		    
		    public Enumerator()
		    {
		        MoveAction = CheckEnumerability;
		    }
		    
    		public object Current
    		{
    			get { return (object) current; }
    		}
    		
    		public bool MoveNext()
    		{
    		    return MoveAction();
    		}
            
    		public void Reset()
    		{
    		    current = payload == null ? null : payload.Start();
    		}
    		
    		private IComparable GetNextSum()
    		{
    		    IComparable sum;
    		    
    		    switch (orientation)
    		    {
    		    	case Enumeration.POSITIVE : sum = (IComparable) Algorithms.Arithmetic.Sum(current, step);
    		                                    break;
    		        case Enumeration.NEGATIVE : sum = (IComparable) Algorithms.Arithmetic.Difference(current, step);
    		                                    break;
    		        default : sum = 0L;
    		                  break;
    		    }
    		    
    		    return sum;
    		}
    
    		private bool CheckEnumerability()
    		{
    		    if (payload.Contains(current))
    		    {
    		        MoveAction = Move;
    		        
    		        return true;
    		    }
    		    
    		    return false;
    		}
    		
    		private bool Move()
    		{
    		    IComparable sum = GetNextSum();
    		    
    		    if (payload.Contains(sum))
    		    {
    		        current = sum;
    		        
    		        return true;
    		    }
    		    
    		    return false;
    		}
		}
	}
}

namespace System.Collections.Generic
{
    /// <summary>
    /// Description of Interval.
    /// </summary>

//	public enum Boundary    { OPEN, CLOSED }
//	public enum Enumeration { POSITIVE, NEGATIVE }
	
	public class Interval<T> : IEnumerable<T> where T : IComparable
	{
		private readonly T top;
		private readonly T bottom;
		
		private readonly Boundary upper;
		private readonly Boundary lower;
		
		public T Top
		{
			get { return top; }
		}
		
		public T Bottom
		{
			get { return bottom; }
		}
		
		public Boundary Upper
		{
			get { return upper; }
		}
		
		public Boundary Lower
		{
			get { return lower; }
		}
		
		public T EnumeratorStepSize { get; set; }
		
		public Enumeration EnumeratorOrientation { get; set; }
		
		public Interval( T firstValue, Boundary firstBound,
		                 T secndValue, Boundary secndBound,
		                 T step )
		{
			if (firstValue.CompareTo(secndValue) < 0)
			{
				bottom = firstValue;
				lower  = firstBound;
				
				top    = secndValue;
				upper  = secndBound;
				
				EnumeratorOrientation = Enumeration.POSITIVE;
			}
			else
			{
				bottom = secndValue;
				lower  = secndBound;
				
				top    = firstValue;
				upper  = firstBound;
				
				EnumeratorOrientation = Enumeration.NEGATIVE;
			}
			
			EnumeratorStepSize = step;
		}
		
		public bool Contains(T value)
		{
			return
			(
				(
					(
						lower == Boundary.OPEN
						&&
						value.CompareTo(bottom) > 0
					)
					||
					(
						lower == Boundary.CLOSED
						&&
						value.CompareTo(bottom) >= 0
					)
				)
				&&
				(
					(
						upper == Boundary.OPEN
						&&
						value.CompareTo(top) < 0
					)
					||
					(
						upper == Boundary.CLOSED
						&&
						value.CompareTo(top) <= 0
					)
				)
			);
		}
		
		public T Start()
		{
		    return EnumeratorOrientation ==
		        
		        Enumeration.POSITIVE ? (lower == Boundary.CLOSED ? bottom
		                                
		    	                        : (T) Algorithms.Arithmetic.Sum(bottom, EnumeratorStepSize))
		        
		                             : (upper == Boundary.CLOSED ? top
		           
		                                : (T) Algorithms.Arithmetic.Difference(top, EnumeratorStepSize));
		}
		
		public IEnumerator<T> GetEnumerator()
		{
		    var e = new Enumerator<T>();
		    
		    e.payload     = this;
		    e.step        = EnumeratorStepSize;
		    e.orientation = EnumeratorOrientation;
		    e.current     = Start();
		    
		    return e;
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
		
		public class Enumerator<E> : IEnumerator<E> where E : IComparable
		{
		    internal delegate bool Action();
		    
		    internal Interval<E>  payload;
		    internal E            current;
		    
		    internal E            step;
		    internal Enumeration  orientation;
		    
		    internal Action       MoveAction;
		    
		    public Enumerator()
		    {
		        MoveAction = CheckEnumerability;
		    }
		    
    		public E Current
    		{
    			get { return current; }
    		}
    		
    		object IEnumerator.Current
    		{
    			get { return (object) current; }
    		}
    		
			public void Dispose()
			{
				payload    = null;
				MoveAction = null;
			}
			
    		public bool MoveNext()
    		{
    		    return MoveAction();
    		}
            
    		public void Reset()
    		{
    		    current = payload.Start();
    		}
    		
    		private E GetNextSum()
    		{
    		    switch (orientation)
    		    {
    		    	case Enumeration.POSITIVE : return (E) Algorithms.Arithmetic.Sum(current, step);
    		        case Enumeration.NEGATIVE : return (E) Algorithms.Arithmetic.Difference(current, step);
    		        default : throw new Exception("Bug: Attempting to select a non-existent Orientation value.");
    		    }
    		}
    
    		private bool CheckEnumerability()
    		{
    		    if (payload.Contains(current))
    		    {
    		        MoveAction = Move;
    		        
    		        return true;
    		    }
    		    
    		    return false;
    		}
    		
    		private bool Move()
    		{
    		    E sum = GetNextSum();
    		    
    		    if (payload.Contains(sum))
    		    {
    		        current = sum;
    		        
    		        return true;
    		    }
    		    
    		    return false;
    		}
		}
	}
}
