using System;
using System.Collections.Generic;

namespace MBAutoComplete
{
	public interface ISortingAlghorithm
	{
		ICollection<string> DoSort(string userInput, ICollection<string> inputStrings);
	}
}

