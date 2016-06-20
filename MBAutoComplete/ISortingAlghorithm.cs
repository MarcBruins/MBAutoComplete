using System;
using System.Collections.Generic;

namespace MBAutoComplete
{
	public interface ISortingAlghorithm
	{
		IList<string> DoSort(string userInput, IList<string> inputStrings);
	}
}

