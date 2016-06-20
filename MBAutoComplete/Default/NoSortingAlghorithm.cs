using System;
using System.Collections.Generic;

namespace MBAutoComplete
{
	public class NoSortingAlghorithm : ISortingAlghorithm
	{
		public NoSortingAlghorithm(){}

		public IList<string> DoSort(string userInput, IList<string> inputStrings)
		{
			return inputStrings;
		}
	}
}

