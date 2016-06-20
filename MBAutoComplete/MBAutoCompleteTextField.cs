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

		public AutoCompleteDataSource DataSource
		{
			get;
			set;
		} = new DefaultDataSource();

		public UITableView AutoCompleteTableView
		{
			get;
			private set;
		}

		public IDataFetcher DateFetcher
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


		private UIViewController _parentViewController;


		//UITableViewcontroller settings
		private bool _parentIsUITableViewController = false;
		private UITableViewController _parentTableViewController;
		private bool _parentTableViewBounces = false;
		private bool _parentTableViewAllowsSelection = false;

		public MBAutoCompleteTextField(IntPtr ptr) : base(ptr)
		{
			
		}

		public void Setup(UIViewController view, List<string> suggestions)
		{
			_parentViewController = view;
			DateFetcher  = new DefaultDataFetcher(suggestions);
			initialize();
		}

		public void Setup(UIViewController view, IDataFetcher fetcher)
		{
			_parentViewController = view;
			DateFetcher = fetcher;
			initialize();
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
			AutoCompleteTableView.Hidden = true;
			AutoCompleteTableView.TranslatesAutoresizingMaskIntoConstraints = false;
			AutoCompleteTableView.Source = this.DataSource;

			//Some textfield settings
			this.TranslatesAutoresizingMaskIntoConstraints = false;
			this.Delegate = this;
			this.AutocorrectionType = UITextAutocorrectionType.No;
			this.ClearButtonMode = UITextFieldViewMode.WhileEditing;

			var isTableViewController = _parentViewController as UITableViewController;

			//if parent is tableviewcontroller
			if (isTableViewController != null)
			{
				_parentIsUITableViewController = true;
				_parentTableViewController = isTableViewController;
				_parentTableViewBounces = _parentTableViewController.TableView.Bounces;
				_parentTableViewAllowsSelection = _parentTableViewController.TableView.AllowsSelection;

				// Disable clip to bounds to present the suggestions tableview properly
				UITableViewCell cell = Superview.Superview as UIKit.UITableViewCell;
				cell.ClipsToBounds = false;
				AutoCompleteTableView.BackgroundColor = UIColor.Black;

				//Add the view to the contentview of the cell
				Superview.AddSubview(AutoCompleteTableView);

				//Get indexpath to set the constraint to the right cell
				NSIndexPath indexPath = _parentTableViewController.TableView.IndexPathForCell(cell);

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

			DataSource.AutoCompleteTextField = this; //ugly hack?
										
			//listen to edit events
			this.AllEditingEvents += async (sender,eventargs) =>
			{
				if (this.Text.Length > StartAutoCompleteAfterTicks)
					await showAutoCompleteView();
				else
					hideAutoCompleteView();
			};

		}

		private async Task showAutoCompleteView()
		{
			AutoCompleteTableView.Hidden = false;

			if (_parentIsUITableViewController) //if there is a parenttable
			{
				_parentTableViewController.TableView.Bounces = false;
				_parentTableViewController.TableView.AllowsSelection = false;

				_parentTableViewController.View.Add(AutoCompleteTableView);//todo fix so that is not added multiple times
			}
			await UpdateTableViewData();
		}

		private void hideAutoCompleteView()
		{
			AutoCompleteTableView.Hidden = true;

			if (_parentIsUITableViewController) //if there is a parenttable
			{
				_parentTableViewController.TableView.Bounces = true;
				_parentTableViewController.TableView.AllowsSelection = true;
			}
		}

		public async Task UpdateTableViewData(){

			await DateFetcher.PerformFetch(
				delegate (List<string> unsortedData)
			{
				var sorted = this.SortingAlghorithm.DoSort(unsortedData);

				this.DataSource.Suggestions = sorted;

				AutoCompleteTableView.ReloadData();
			}
			);
		}

	}
}

