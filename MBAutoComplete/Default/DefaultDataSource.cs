using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace MBAutoComplete
{
	public class DefaultDataSource : AutoCompleteDataSource
	{
		private string CellIdentifier = "DefaultIdentifier";
		private ICollection<string> suggestions;

		public override void NewSuggestions(ICollection<string> suggestions)
		{
			this.suggestions = suggestions;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
			string item = suggestions.ElementAt(indexPath.Row);

			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);

			cell.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
			cell.TextLabel.Text = item;

			return cell;
		}



		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			base.RowSelected(tableView, indexPath);
		}
	}
}

