using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace MBAutoComplete
{
	public class DefaultDataSource : AutoCompleteDataSource
	{
		private string CellIdentifier = "DefaultIdentifier";


		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
			string item = Suggestions[indexPath.Row];

			//---- if there are no cells to reuse, create a new one
			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);

			cell.BackgroundColor = UIColor.LightGray;
			cell.TextLabel.Text = item;

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			base.RowSelected(tableView, indexPath);
		}
	}
}

