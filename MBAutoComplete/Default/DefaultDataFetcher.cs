using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MBAutoComplete
{
	public class DefaultDataFetcher : IDataFetcher
	{
		private List<string> _unsortedData; 

		public DefaultDataFetcher(List<string> unsortedData)
		{
			_unsortedData = unsortedData;
		}

		public async Task PerformFetch(Action<List<string>> completionHandler)
		{
			completionHandler(_unsortedData);
		}
	}
}

