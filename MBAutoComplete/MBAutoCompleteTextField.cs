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

		private bool _parentIsUITableViewController = false;

		private UIViewController _parentViewController;
		private UITableViewController _parentTableViewController;

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

			// Check if the superview is a uitableviewcell, by doing this we know that the parent is a uitableviewcontroller
			Type type = Superview.Superview?.GetType();

			//if parent is tableviewcontroller
			if (type != null && type == typeof(UITableViewCell))
			{
				_parentIsUITableViewController = true;
				_parentTableViewController = _parentViewController as UITableViewController;

				// Disable clip to bounds to present the suggestions tableview properly
				UITableViewCell cell = (UIKit.UITableViewCell)Superview.Superview;
				cell.ClipsToBounds = false;
				AutoCompleteTableView.BackgroundColor = UIColor.Black;

				Superview.AddSubview(AutoCompleteTableView);

				//add constraints
				Superview.AddConstraints(
					AutoCompleteTableView.WithSameCenterY(this).Plus((150 / 2) + 10 + cell.Frame.Height),
					AutoCompleteTableView.WithSameWidth(this),
					AutoCompleteTableView.WithSameLeft(this),
					AutoCompleteTableView.Height().EqualTo(150)
				);

			}
			else
			{
				Superview.InsertSubviewAbove(AutoCompleteTableView, _parentViewController.View);

				//add constraints
				Superview.AddConstraints(
					AutoCompleteTableView.AtTopOf(this, this.Frame.Height - 5),
					AutoCompleteTableView.WithSameWidth(this),
					AutoCompleteTableView.WithSameLeft(this),
					AutoCompleteTableView.Height().EqualTo(150)
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

