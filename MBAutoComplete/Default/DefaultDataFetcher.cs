using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MBAutoComplete
{
	public class DefaultDataFetcher : IDataFetcher
	{
		private ICollection<string> _unsortedData; 

		public DefaultDataFetcher(ICollection<string> unsortedData)
		{
			_unsortedData = unsortedData;
		}

		public Task PerformFetch(MBAutoCompleteTextField textfield, Action<ICollection<string>> completionHandler)
		{
			completionHandler(_unsortedData);
			return Task.CompletedTask;
		}
	}
}

