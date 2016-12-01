/*
 * Created by SharpDevelop.
 * User: Andrew Daniels
 * Date: 10/30/2016
 * Time: 12:10 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace System.Collections
{
	/// <summary>
	/// Description of SortedStringSet.
	/// </summary>
	
	public class SortedStringSet
	{
		private Node  _root  = null;
		private Int64 _count = 0;
		
		public SortedStringSet() {}
		
		public SortedStringSet(string[] range)
		{
			AddRange(range);
		}
		
		public Int64 Count
		{
			get { return _count; }
		}
		
		public bool Any()
		{
			return Count > 0;
		}
		
		private void Add(string str, int pos, ref Node next)
		{
			if (next == null)
				next = new Node(str[pos], false);
			
			if (str[pos] < next._payload)
				Add(str, pos, ref next._left);
			else if (str[pos] > next._payload)
				Add(str, pos, ref next._right);
			else
				if (pos + 1 == str.Length)
				{
					if (!next._word_end)
						_count = _count + 1;
						
					next._word_end = true;
				}
				else
					Add(str, pos + 1, ref next._center);
		}
		
		public void Add(string str)
		{
			if (String.IsNullOrWhiteSpace(str))
				throw new ArgumentException();
			
			Add(str, 0, ref _root);
		}
		
		public void AddRange(string[] range)
		{
			foreach (string str in range) Add(str);
		}
		
		public bool Contains(string str)
		{
			if (String.IsNullOrEmpty(str))
				return false;
			
			int pos = 0; Node cursor = _root;
			
			while (cursor != null)
			{
				if (str[pos] < cursor._payload)
					cursor = cursor._left;
				else if (str[pos] > cursor._payload)
					cursor = cursor._right;
				else
				{
					if (++pos == str.Length)
						return cursor._word_end;
					
					cursor = cursor._center;
				}
			}
			
			return false;
		}
		
		private void Push(ref List<string> listSet,
			              string str,
			              Node cursor)
		{
			if (cursor != null)
			{
				if (cursor._word_end)
					listSet.Add(str + cursor._payload);
				
				Push(ref listSet, str, cursor._left);
				Push(ref listSet, str + cursor._payload, cursor._center);
				Push(ref listSet, str, cursor._right);
			}
		}
		
		public List<string> ToList()
		{
			var list = new List<string>();
			
			Push(ref list, "", _root);
			
			return list;
		}
		
		public List<string> GetSuggestions(string str)
		{
			if (String.IsNullOrWhiteSpace(str))
				return ToList();
			
			var listSet = new List<string>();
			
			Node cursor = _root;
			
			int pos = 0;
			
			while (cursor != null)
			{
				if (str[pos] < cursor._payload)
					cursor = cursor._left;
				else if (str[pos] > cursor._payload)
					cursor = cursor._right;
				else
				{
					if (++pos == str.Length)
					{
						if (cursor._word_end)
						{
							listSet.Add(str);
						}
						
						Push(ref listSet, str, cursor._center);
						
						return listSet;
					}
					
					cursor = cursor._center;
				}
			}
			
			return listSet;
		}
		
		private class Node
		{
			internal char _payload;
			internal Node _left, _center, _right;
			internal bool _word_end;
			
			public Node(char payload, bool wordEnd)
			{
				_payload  = payload;
				_word_end = wordEnd;
			}
		}
	}
}
