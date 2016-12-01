/*
 * Created by SharpDevelop.
 * User: Drew
 * Date: 4/7/2016
 * Time: 2:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PowerWalk
{
	public enum TypingPolicies    { WEAK,   STRONG }
	public enum RetrievalPolicies { STRICT, LOOSE  }
	
	public static class Value
	{
	    public class Empty: Object {};
	    
	    public static readonly object NOVALUE = new Empty();
    	
    	public static bool IsEmpty(Object value) { return value.Equals(NOVALUE); }
	}
    	
	public abstract partial class Named
	{
	    public static partial class Static
	    {
            public static Dictionary<string, Factory> GetStaticTypes()
            {
                var types = new Dictionary<string, Factory>();
                
                types.Add("",            new Factory(e => new Named.Value(e)));
                
                types.Add("Object",      new Factory(e => new Object<object>(e)));
                types.Add("String",      new Factory(e => new Object<string>(e)));
                types.Add("Integer",     new Factory(e => new Object<Int32>(e)));
                types.Add("Long",        new Factory(e => new Object<Int64>(e)));
                types.Add("Int32",       new Factory(e => new Object<Int32>(e)));
                types.Add("Int64",       new Factory(e => new Object<Int64>(e)));
                types.Add("Float",       new Factory(e => new Object<float>(e)));
                types.Add("Double",      new Factory(e => new Object<double>(e)));
                types.Add("Boolean",     new Factory(e => new Object<bool>(e)));
                types.Add("Array",       new Factory(e => new Object<Array>(e)));
                types.Add("Hashtable",   new Factory(e => new Object<Hashtable>(e)));
                types.Add("ArrayList",   new Factory(e => new Object<ArrayList>(e)));
                
                types.Add("Object[]",    new Factory(e => new Object<object[]>(e)));
                types.Add("String[]",    new Factory(e => new Object<string[]>(e)));
                types.Add("Integer[]",   new Factory(e => new Object<Int32[]>(e)));
                types.Add("Long[]",      new Factory(e => new Object<Int64[]>(e)));
                types.Add("Int32[]",     new Factory(e => new Object<Int32[]>(e)));
                types.Add("Int64[]",     new Factory(e => new Object<Int64[]>(e)));
                types.Add("Float[]",     new Factory(e => new Object<float[]>(e)));
                types.Add("Double[]",    new Factory(e => new Object<double[]>(e)));
                types.Add("Boolean[]",   new Factory(e => new Object<bool[]>(e)));
                types.Add("Array[]",     new Factory(e => new Object<Array[]>(e)));
                types.Add("Hashtable[]", new Factory(e => new Object<Hashtable[]>(e)));
                types.Add("ArrayList[]", new Factory(e => new Object<ArrayList[]>(e)));
                
                return types;
            }
        }
	}
    
    /// <summary>
    /// Description of NameTable.
    /// </summary>

	public class NameTable
	{
		private readonly List<Dictionary<string, Named>> _payload;
		
		private delegate bool GetHandler(bool keyFound, string key);
		private delegate bool SetHandler(IDictionary<string, Named> level, string key, object nextValue);
		
		private SetHandler HandleSettingConflict;
		private SetHandler HandleDeclareConflict;
		private GetHandler HandleMissingVariable;
		
		private bool EnforceTyping(IDictionary<string, Named> level, string key, object nextValue)
		{
		    if ((new string[]{nextValue.GetType().ToString(), "Object"}).Contains(level[key].GetType().ToString()))
			{
				(level[key]).Payload = nextValue;
				return true;
			}
			
			return false;
		}
		
//		private bool TryTypeCast(IDictionary level, object key, object nextValue)
//		{
//		    
//		}
		
		private bool SetValueAndReturnTrue(IDictionary<string, Named> level, string key, object nextValue)
		{
	        level[key].Payload = nextValue;
		    return true;
		}
		
		private bool ReturnFalse(IDictionary<string, Named> level, string key, object nextValue)
		{
			return false;
		}
		
		private bool PassThrough(bool keyFound, string key)
		{
			return keyFound;
		}
		
		private bool ProvideAndReturnTrue(bool keyFound, string key)
		{
			if (!keyFound)
				AddValue(key, null);
			
			return true;
		}
		
		public NameTable(Dictionary<string, Named> baseLvl, TypingPolicies typing, RetrievalPolicies retrieval)
		{
			_payload = new List<Dictionary<string, Named>>();
			_payload.Add(baseLvl);
			
			switch (typing)
			{
				case TypingPolicies.STRONG:
					HandleSettingConflict = EnforceTyping;
					break;
				case TypingPolicies.WEAK:
					HandleSettingConflict = SetValueAndReturnTrue;
					break;
			}
			
			switch (retrieval)
			{
				case RetrievalPolicies.STRICT:
					HandleDeclareConflict = ReturnFalse;
					HandleMissingVariable = PassThrough;
					break;
				case RetrievalPolicies.LOOSE:
					HandleDeclareConflict = SetValueAndReturnTrue;
					HandleMissingVariable = ProvideAndReturnTrue;
					break;
			}
		}
		
		public NameTable() : this(new Dictionary<string, Named>(), TypingPolicies.WEAK, RetrievalPolicies.LOOSE) {}
		
		public NameTable(Dictionary<string, Named> baseLvl): this(baseLvl, TypingPolicies.WEAK, RetrievalPolicies.LOOSE) {}
		
		public int Find(string name)
		{
			bool found = false;
			int i = 0;
			
			while(!found && i < _payload.Count)
			    
			    if (_payload[i].ContainsKey(name))
				    
					found = true;
				
				else
				    
					i = i + 1;
			
			return found ? i : -1;
		}
		
		public object Get(string name)
		{
		    int index = Find(name);
		    
		    return index >= 0 ? (_payload[index][name]).Payload : null;
		}
		
		public object this[string name]
		{
			get
			{
				int index = Find(name);
				
				if (HandleMissingVariable(index >= 0, name))
				    
				    return (_payload[index >= 0 ? index : Count - 1][name]).Payload;
				
				throw new KeyNotFoundException("The name, \"" + name + "\", was not found in the variable list.");
			}
			
			set
			{
				int  index   = Find(name);
				bool success = index >= 0;
				
				if (HandleMissingVariable(success, name))
				{
    				index = success ? index : Count - 1;
    				
				    if (!HandleSettingConflict(_payload[index], name, value))
				    {
				        throw new InvalidCastException( "Could not convert " + value.GetType().Name + " to type "
				                                         + _payload[index].GetType().Name + "." );
				    }
				}
				else
				{
				    throw new KeyNotFoundException("The name, \"" + name + "\", was not found in the variable list.");
				}
			}
		}
		
		public int Count
		{
			get { return _payload.Count; }
		}
		
		public void Push()
		{
			_payload.Add(new Dictionary<string, Named>());
		}
		
		public bool Pop()
		{
			if(_payload.Any())
			{
				_payload.RemoveAt(_payload.Count - 1);
				
				return true;
			}
			
			return false;
		}
		
		public Dictionary<string, Named> GetTable(string key)
		{
		    foreach (var level in _payload)
		        if (level.ContainsKey(key))
		            return level;
		    
		    return (Dictionary<string, Named>) null;
		}
		
		public bool AddDynamicValue(string key, Named.Value value)
		{
			foreach (var level in _payload)
				if (level.ContainsKey(key))
					return HandleDeclareConflict(level, key, value.Payload);
			
			_payload.Last().Add(key, value);
			
			return true;
		}
		
		public bool AddNonStandardValue(string key, Named value)
		{
		    foreach (var level in _payload)
		        if (level.ContainsKey(key))
		            return false;
		    
		    _payload.Last().Add(key, value);
		    
		    return true;
		}
		
		// Add Named Value
		public bool AddValue(string key, object value, bool isReadable = true, bool isWriteable = true)
		{
			foreach (var level in _payload)
				if (level.ContainsKey(key))
					return HandleDeclareConflict(level, key, value);
			
			_payload.Last().Add(key, new Named.Value(value, isReadable, isWriteable));
			
			return true;
		}
		
		// Add Named Reference
		public bool AddReference(string key, Dictionary<string, Named> table, string keyRef, bool isReadable = true, bool isWriteable = true)
		{
		    foreach (var level in _payload)
		        if (level.ContainsKey(key))
		            return false;
		    
		    _payload.Last().Add(key, new Named.Reference(table, keyRef, isReadable, isWriteable));
		    
		    return true;
		}
		
		// Add Named Reference
		public bool AddReference(string key, string keyRef, bool isReadable = true, bool isWriteable = true)
		{
		    return AddReference(key, GetTable(keyRef), keyRef, isReadable, isWriteable);
		}
		
		// Add Named Static Value
		public bool AddValue(string key, string typename, object value, bool isReadable = true, bool isWriteable = true)
		{
		    foreach (var level in _payload)
		        if (level.ContainsKey(key))
		            return false;
		    
		    _payload.Last().Add(key, new Named.Static.Value(typename, value, isReadable, isWriteable));
		    
		    return true;
		}
		
		// Add Named Static Reference
		public bool AddReference(string key, string typename, Dictionary<string, Named> table, string keyRef, bool isReadable = true, bool isWriteable = true)
		{
		    foreach (var level in _payload)
		        if (level.ContainsKey(key))
		            return false;
		    
		    _payload.Last().Add(key, new Named.Static.Reference(typename, table, keyRef, isReadable, isWriteable));
		    
		    return true;
		}
		
		// Add Named Static Reference
		public bool AddReference(string key, string typename, string keyRef, bool isReadable = true, bool isWriteable = true)
		{
		    return AddReference(key, typename, GetTable(keyRef), keyRef, isReadable, isWriteable);
		}
		
		public bool Remove(string key)
		{
			int index = Find(key);
			
			if (index < 0)
				return false;
			
			_payload[index].Remove(key);
			
			return true;
		}
		
		// TODO: Change tabs to string formatters.
		public override string ToString()
		{
			string str = "";
			
			for (int i = 0; i < _payload.Count; ++i)
			{
				str += new string(' ', 2 * (i + 1)) + "LEVEL " + (i + 1) + "\n";
				
				if (_payload[i].Any())
				{
					str += "\n";
					
					foreach (var key in from k in _payload[i].Keys orderby k select k)
						
						str += new string(' ', 2 * (i + 1) + 1) + key + "  "
							+ _payload[i][key] + "\n";
				}
				
				str += "\n";
			}
			
			return str;
		}
	}
	
	internal delegate object TryGet();
	internal delegate void   TrySet(object value);
	
	public abstract partial class Named
	{
	    private readonly bool _readable;
	    private readonly bool _writeable;
	    
	    private readonly TryGet Read;
	    private readonly TrySet Write;
	    
        private const string DENY_READABILITY  = "Permission denied: Cannot read value.";
        private const string DENY_WRITEABILITY = "Permission denied: Cannot overwrite value.";
        
	    protected Named(bool isReadable, bool isWriteable)
	    {
	        _readable  = isReadable;
	        _writeable = isWriteable;
	        
	        Read  = isReadable  ? new TryGet(Get) : new TryGet(() => { throw new Exception(DENY_READABILITY); });
	        Write = isWriteable ? new TrySet(Set) : new TrySet( e => { throw new Exception(DENY_WRITEABILITY); });
	    }
	    
	    protected Named(): this(true, true) {}
	    
	    public abstract object Get();
	    public abstract void   Set(object payload);
	    
	    public object Payload
	    {
	        get { return Read(); }
	        set { Write(value);   }
	    }
	    
	    public bool IsReadable
	    {
	        get { return _readable; }
	    }
	    
	    public bool IsWriteable
	    {
	        get { return _writeable; }
	    }
        
		// TODO: Change tabs to string formatters.
        public override string ToString()
        {
            return (_readable  ? "R" : "")
            	 + (_writeable ? "W" : "") + new string(' ', Preferences.tab_size * 2)
            	 + Get();
        }
        
        public class Value : Named
        {
            private object _payload;
            
            public Value(object payload, bool isReadable, bool isWriteable):
                base(isReadable, isWriteable)
            {
                _payload = payload;
            }
            
            public Value(object payload):
                this(payload, true, true) {}
            
            public override object Get()
            {
                return _payload;
            }
            
            public override void Set(object payload)
            {
                _payload = payload;
            }
            
            public static Value ReadOnly(object payload)
            {
                return new Value(payload, true, false);
            }
            
            public static Value WriteOnly(object payload)
            {
                return new Value(payload, false, true);
            }
        }
        
        public class Reference : Named
        {
            private readonly Dictionary<string, Named> _table;
            private readonly string _key;
            
            public Reference(Dictionary<string, Named> table, string key, bool isReadable, bool isWriteable):
                base(isReadable, isWriteable)
            {
                _table = table;
                _key   = key;
            }
            
            public Reference(Dictionary<string, Named> table, string key):
                this(table, key, true, true) {}
            
            public override object Get()
            {
                return ((Named) _table[_key]).Payload;
            }
            
            public override void Set(object payload)
            {
                ((Named) _table[_key]).Payload = payload;
            }
            
            public static Reference ReadOnly(Dictionary<string, Named> table, string key)
            {
                return new Reference(table, key, true, false);
            }
            
            public static Reference WriteOnly(Dictionary<string, Named> table, string key)
            {
                return new Reference(table, key, false, true);
            }
        }
        
        public static partial class Static
        {
            public delegate object Factory(object e);
            
            public static Dictionary<string, Factory> NewObject = GetStaticTypes();
            
            private class Object<T> : Named, ICloneable
            {
                private T _payload;
                
                public Object(object payload, bool isReadable, bool isWriteable):
                    base(isReadable, isWriteable)
                {
                    _payload = (T) payload;
                }
                
                public Object(object payload):
                    this(payload, true, true) {}
                
                public Object():
                    this(null, true, true) {}
                
                public override object Get()
                {
                    return _payload;
                }
                
                public override void Set(object payload)
                {
                    _payload = (T) payload;
                }
        
                #region ICloneable implementation
        
                public object Clone()
                {
                    return new Object<T>(_payload);
                }
        
                #endregion
            }
            
            public class Value : Named
            {
                private Named _ptr;
                
                public Value(string typename, object payload, bool isReadable, bool isWriteable):
                    base(isReadable, isWriteable)
                {
                    _ptr = (Named) NewObject[typename](payload);
                }
                
                public Value(string typename, Named ptr):
                    this(typename, ptr.Payload, ptr.IsReadable, ptr.IsWriteable) {}
                
                public Value(string typename, bool isReadable, bool isWriteable):
                    this(typename, null, isReadable, isWriteable) {}
                
                public Value(string typename, object payload):
                    this(typename, payload, true, true) {}
                
                public Value(string typename):
                    this(typename, null, true, true) {}
                
                public override object Get()
                {
                    return _ptr.Payload;
                }
                
                public override void Set(object payload)
                {
                    _ptr.Payload = payload;
                }
                
                public static Static.Value ReadOnly(string typename, object payload)
                {
                    return new Static.Value(typename, payload, true, false);
                }
                
                public static Static.Value ReadOnly(string typename)
                {
                    return new Static.Value(typename, true, false);
                }
                
                public static Static.Value WriteOnly(string typename, object payload)
                {
                    return new Static.Value(typename, payload, false, true);
                }
                
                public static Static.Value WriteOnly(string typename)
                {
                    return new Static.Value(typename, false, true);
                }
            }
            
            public class Reference : Named
            {
                private readonly Dictionary<string, Named> _table;
                private readonly string _key;
                
                private Named _original;
                
                public Reference(string typename, Dictionary<string, Named> table, string key, bool isReadable, bool isWriteable):
                    base(isReadable, isWriteable)
                {
                    _table = table;
                    _key   = key;
                    
                    _original    = (Named) _table[_key];
                    _table[_key] =
                        
                        new Static.Value
                        (
                            typename,
                            _original
                        );
                }
                
                public Reference(string typename, Dictionary<string, Named> table, string key):
                    this(typename, table, key, true, true) {}
                
                ~Reference()
                {
                    _original.Payload = ((Named) _table[_key]).Payload;
                    _table[_key] = _original;
                }
                
                public override object Get()
                {
                    return ((Named) _table[_key]).Payload;
                }
                
                public override void Set(object payload)
                {
                    ((Named) _table[_key]).Payload = payload;
                }
                
                public static Reference ReadOnly(Dictionary<string, Named> table, string key)
                {
                    return new Reference(table, key, true, false);
                }
                
                public static Reference WriteOnly(Dictionary<string, Named> table, string key)
                {
                    return new Reference(table, key, false, true);
                }
            }
        }
	}
}
