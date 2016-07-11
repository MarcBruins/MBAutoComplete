using System;
using System.Collections.Generic;

namespace MBAutoComplete
{
	public class NoSortingAlghorithm : ISortingAlghorithm
	{
		public NoSortingAlghorithm(){}

		public ICollection<string> DoSort(string userInput, ICollection<string> inputStrings)
		{
			return inputStrings;
		}
	}
}

