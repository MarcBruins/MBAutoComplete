using System;
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
	[Register("MBAutoCompleteTextField")]
	public class MBAutoCompleteTextField : UITextField, IUITextFieldDelegate
	{
		public ISortingAlghorithm SortingAlghorithm
		{
			get;
			set;
		} = new DefaultSortingAlgorithm();

		private AutoCompleteDataSource _dataSource;
		public AutoCompleteDataSource DataSource
		{
			get { return _dataSource; }
			set {
				_dataSource = value; 
				_dataSource.AutoCompleteTextField = this;
				if(AutoCompleteTableView != null)
					AutoCompleteTableView.Source = this.DataSource;
			}
		}

		public UITableView AutoCompleteTableView
		{
			get;
			private set;
		}

		public IDataFetcher DataFetcher
		{
			get;
			set;
		}

		public int StartAutoCompleteAfterTicks
		{
			get;
			set;
		} = 2;

		public int AutocompleteTableViewHeight
		{
			get;
			set;
		} = 150;

		//store parent as uiviewcontroller or as uitableviewcontroller
		private UIViewController _parentViewController;
		private UITableViewController _parentTableViewController;

		//UITableViewcontroller settings
		private bool _parentIsUITableViewController = false;
		private bool _parentTableViewBounces = false;
		private bool _parentTableViewAllowsSelection = false;

		public MBAutoCompleteTextField(IntPtr ptr) : base(ptr){}

		public void Setup(UIViewController view, IList<string> suggestions)
		{
			_parentViewController = view;
			DataFetcher  = new DefaultDataFetcher(suggestions);
			DataSource = new DefaultDataSource();
			initialize();
		}

		public void Setup(UIViewController view, IDataFetcher fetcher)
		{
			_parentViewController = view;
			DataFetcher = fetcher;
			DataSource = new DefaultDataSource();
			initialize();
		}

		public void Setup(UIViewController view, IList<string> suggestions, UITableViewController tableViewController)
		{
			_parentTableViewController = tableViewController;
			Setup(view, suggestions);
		}

		public void Setup(UIViewController view, IDataFetcher fetcher, UITableViewController tableViewController)
		{
			_parentTableViewController = tableViewController;
			Setup(view, fetcher);
		}

		private void initialize()
		{
			//Make new tableview and do some settings
			AutoCompleteTableView = new UITableView();
			AutoCompleteTableView.Layer.CornerRadius = 5; //rounded corners
			AutoCompleteTableView.ContentInset = UIEdgeInsets.Zero;
			AutoCompleteTableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight; //for resizing (switching from table to portait for example)
			AutoCompleteTableView.Bounces = false;
			AutoCompleteTableView.BackgroundColor = UIColor.Clear;
			AutoCompleteTableView.TranslatesAutoresizingMaskIntoConstraints = false;
			AutoCompleteTableView.Source = this.DataSource;
			AutoCompleteTableView.TableFooterView = new UIView();
			AutoCompleteTableView.Hidden = true;

			//Some textfield settings
			this.TranslatesAutoresizingMaskIntoConstraints = false;
			this.Delegate = this;
			this.AutocorrectionType = UITextAutocorrectionType.No;
			this.ClearButtonMode = UITextFieldViewMode.WhileEditing;

			var isTableViewController = _parentTableViewController ?? (_parentViewController as UITableViewController);

			//if parent is tableviewcontroller
			if (isTableViewController != null)
			{
				_parentIsUITableViewController = true;
				if (_parentTableViewController == null)
					_parentTableViewController = isTableViewController;
				_parentTableViewBounces = _parentTableViewController.TableView.Bounces;
				_parentTableViewAllowsSelection = _parentTableViewController.TableView.AllowsSelection;

				//Add the view to the contentview of the cell
				Superview.AddSubview(AutoCompleteTableView);

				UITableViewCell cell = Superview.Superview as UIKit.UITableViewCell;

				//Get indexpath to set the constraint to the right cell
				NSIndexPath indexPath = _parentTableViewController.TableView.IndexPathForCell(cell);

				if (indexPath == null)
				{
					Console.WriteLine("Should be initialized in the ViewDidAppear and not in the ViewDidLoad!");
					return;
				}
				//add constraints
				Superview.AddConstraints(
					AutoCompleteTableView.WithSameCenterY(this).Plus((AutocompleteTableViewHeight / 2) + 10 + cell.Frame.Height * indexPath.Row),
					AutoCompleteTableView.WithSameWidth(this),
					AutoCompleteTableView.WithSameLeft(this),
					AutoCompleteTableView.Height().EqualTo(AutocompleteTableViewHeight)
				);
			}
			else
			{
				Superview.InsertSubviewBelow(AutoCompleteTableView, _parentViewController.View);

				//add constraints
				Superview.AddConstraints(
					AutoCompleteTableView.AtTopOf(this, this.Frame.Height - 5),
					AutoCompleteTableView.WithSameWidth(this),
					AutoCompleteTableView.WithSameLeft(this),
					AutoCompleteTableView.Height().EqualTo(AutocompleteTableViewHeight)
				);
			}

			//listen to edit events
			this.EditingChanged += async (sender, eventargs) =>
			{
				if (this.Text.Length > StartAutoCompleteAfterTicks)
				{
					showAutoCompleteView();
					await UpdateTableViewData();
				}
			};

			this.EditingDidEnd += (sender, eventargs) =>
			{
				hideAutoCompleteView();
			};
		}

		private void showAutoCompleteView()
		{
			AutoCompleteTableView.SetContentOffset(CGPoint.Empty, false);
			AutoCompleteTableView.Hidden = false;

			if (_parentIsUITableViewController) //if is in uitableviewcontroller
			{
				_parentTableViewController.TableView.Bounces = false;
				_parentTableViewController.TableView.AllowsSelection = false;

				_parentTableViewController.View.Add(AutoCompleteTableView);
			}
		}

		private void hideAutoCompleteView()
		{
			AutoCompleteTableView.Hidden = true;

			if (_parentIsUITableViewController) //if is in uitableviewcontroller
			{
				_parentTableViewController.TableView.Bounces = _parentTableViewBounces;
				_parentTableViewController.TableView.AllowsSelection = _parentTableViewAllowsSelection;
			}
		}

		public async Task UpdateTableViewData()
		{
			await DataFetcher.PerformFetch(this, delegate (ICollection<string> unsortedData)
			{
				var sorted = this.SortingAlghorithm.DoSort(this.Text, unsortedData);
				this.DataSource.Suggestions = sorted;

				AutoCompleteTableView.ReloadData();
			}
			);
		}
	}
}