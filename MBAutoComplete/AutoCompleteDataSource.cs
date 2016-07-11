using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Cirrious.FluentLayouts.Touch;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;


namespace MBAutoComplete
{
	public abstract class AutoCompleteDataSource : UITableViewSource
	{
		public event EventHandler RowSelectedEvent;

		private IList<string> _suggestions = new List<string>();
		public IList<string> Suggestions
		{
			get
			{
				return _suggestions;
			}
			set
			{
				_suggestions = value;
			}
		}

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
			AutoCompleteTextField.Text = Suggestions[indexPath.Row];
			AutoCompleteTextField.AutoCompleteTableView.Hidden = true;

			RowSelectedEvent?.Invoke(this, EventArgs.Empty);
		}
	}
}

