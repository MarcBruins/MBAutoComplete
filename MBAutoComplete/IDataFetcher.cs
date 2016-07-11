using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MBAutoComplete
{
	public interface IDataFetcher
	{
		Task PerformFetch(MBAutoCompleteTextField textField, Action<ICollection<string>> completionHandler);
	}
}

