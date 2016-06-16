using System;
using System.Collections.Generic;

namespace MBAutoComplete
{
	public interface ISortingAlghorithm
	{
		List<string> DoSort(List<string> inputStrings);
	}
}

