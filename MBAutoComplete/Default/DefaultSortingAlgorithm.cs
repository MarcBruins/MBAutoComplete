using System;
using System.Collections.Generic;

namespace MBAutoComplete
{
	public class DefaultSortingAlgorithm : ISortingAlghorithm
	{
		public List<string> DoSort(List<string> inputStrings)
		{
			return inputStrings;
		}
	}
}