using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using MBAutoComplete;
using UIKit;

namespace SampleViewController
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			AutoCompleteTextField.Setup(this, new List<string>() { "customizable", "xamarin", "autocomplete", "ios" });
			AutoCompleteTextField.AutoCompleteViewSource = new MyViewSource();
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

	}

	class MyViewSource : MBAutoCompleteViewSource
	{
		private ICollection<string> _suggestions;
		private string _cellIdentifier = "CellId";

		public override void NewSuggestions(ICollection<string> suggestions)
		{
			_suggestions = suggestions;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell(_cellIdentifier);
			string item = _suggestions.ElementAt(indexPath.Row);

			if (cell == null)
				cell = new UITableViewCell(UITableViewCellStyle.Default, _cellIdentifier);

			cell.BackgroundColor = UIColor.Orange;
			cell.TextLabel.TextColor = UIColor.White;
			cell.TextLabel.Text = item;

			return cell;
		}
	}
}

