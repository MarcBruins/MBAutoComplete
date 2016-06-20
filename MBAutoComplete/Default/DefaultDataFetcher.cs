using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MBAutoComplete
{
	public class DefaultDataFetcher : IDataFetcher
	{
		private IList<string> _unsortedData; 

		public DefaultDataFetcher(IList<string> unsortedData)
		{
			_unsortedData = unsortedData;
		}

		public async Task PerformFetch(Action<IList<string>> completionHandler)
		{
			completionHandler(_unsortedData);
		}
	}
}

