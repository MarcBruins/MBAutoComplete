using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;


namespace MBAutoComplete
{
	public abstract class MBAutoCompleteViewSource : UITableViewSource
	{
		public event EventHandler RowSelectedEvent;

		private ICollection<string> _suggestions = new List<string>();
		public ICollection<string> Suggestions
		{
			get
			{
				return _suggestions;
			}
			set
			{
				_suggestions = value;
				NewSuggestions(_suggestions);
			}
		}

		public abstract void NewSuggestions(ICollection<string> suggestions);

		public MBAutoCompleteTextField AutoCompleteTextField
		{
			get;
			set;
		}

		public abstract override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath);

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return Suggestions.Count;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			AutoCompleteTextField.Text = Suggestions.ElementAt(indexPath.Row);
			AutoCompleteTextField.AutoCompleteTableView.Hidden = true;

			RowSelectedEvent?.Invoke(this, EventArgs.Empty);
		}
	}
}

