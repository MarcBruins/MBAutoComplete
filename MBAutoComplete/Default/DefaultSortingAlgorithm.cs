using System;
using System.Collections.Generic;

namespace MBAutoComplete
{
	public class DefaultSortingAlgorithm : ISortingAlghorithm
	{
		private int _maxChanges = 3;

		public DefaultSortingAlgorithm() { }

		public ICollection<string> DoSort(string userInput, ICollection<string> inputStrings)
		{
			var correctedStrings = new List<string>();
			foreach (string input in inputStrings)
			{
				if (levenshteinAlghoritm(userInput, input) <= _maxChanges)
					correctedStrings.Add(input);
			}
			return correctedStrings;
		}

		//levenshtein alghorithm from http://michalis.site/2013/12/levenshtein/
		private int levenshteinAlghoritm(string s, string t)
		{
			if (s == t) return 0;
			if (s.Length == 0) return t.Length;
			if (t.Length == 0) return s.Length;
			var tLength = t.Length;
			var columns = tLength + 1;
			var v0 = new int[columns];
			var v1 = new int[columns];
			for (var i = 0; i < columns; i++)
				v0[i] = i;
			for (var i = 0; i < s.Length; i++)
			{
				v1[0] = i + 1;
				for (var j = 0; j < tLength; j++)
				{
					var cost = (s[i] == t[j]) ? 0 : 1;
					v1[j + 1] = Math.Min(Math.Min(v1[+j] + 1, v0[j + 1] + 1), v0[j] + cost);
					v0[j] = v1[j];
				}
				v0[tLength] = v1[tLength];
			}
			return v1[tLength];
		}
	}
}