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


		public CGRect originalFrame { get; private set; }
		private UIViewController ViewToAddTo;

		public MBAutoCompleteTextField(IntPtr ptr) : base(ptr)
		{
			
		}

		public void Setup(UIViewController view, List<string> suggestions)
		{
			ViewToAddTo = view;
			DateFetcher  = new DefaultDataFetcher(suggestions);
			initialize();
		}

		public void Setup(UIViewController view, IDataFetcher fetcher)
		{
			ViewToAddTo = view;
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

			// Disable clip to bounds to present the suggestions tableview properly
			if (type != null && type == typeof(UITableViewCell))
			{
				UITableViewCell cell = (UIKit.UITableViewCell)Superview.Superview;
				cell.ClipsToBounds = false;
				AutoCompleteTableView.BackgroundColor = UIColor.Black;

				//var tableViewController = ViewToAddTo as UITableViewController;
				originalFrame = cell.Frame;

				Superview.AddSubview(AutoCompleteTableView);
				//Superview.Remov
				//add constraints
				Superview.AddConstraints(
					AutoCompleteTableView.WithSameCenterY(this).Plus((150 / 2) + 10 + cell.Frame.Height),
					AutoCompleteTableView.WithSameWidth(this),
					AutoCompleteTableView.WithSameLeft(this),
					AutoCompleteTableView.Height().EqualTo(150)
				);

			}
			else {
				Superview.InsertSubviewAbove(AutoCompleteTableView, ViewToAddTo.View);

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
				if (this.Text.Length > 2)
				{
					await showAutoCompleteView();
				}
				else
				{
					hideAutoCompleteView();
				}
			};

		}

		private async Task showAutoCompleteView()
		{
			AutoCompleteTableView.Hidden = false;

			var parentTable = ViewToAddTo as UITableViewController;
			if (parentTable != null) //if there is a parenttable
			{
				parentTable.TableView.Bounces = false;
				parentTable.TableView.AllowsSelection = false;

				//parentTable.View.Superview.InsertSubviewAbove(Superview.Superview, parentTable.TableView);
				parentTable.View.Add(AutoCompleteTableView);
			//	parentTable.View.Superview.BringSubviewToFront(Superview.Superview);
			//	parentTable.View.Inse

			//	this.BringSubviewToFront(AutoCompleteTableView);


		//		parentTable.View.Ins
		//		parentTable.View.InsertSubviewAbove(AutoCompleteTableView, Superview);
		//		Superview.InsertSubviewAbove(AutoCompleteTableView,parentTable.TableView);
			}
			await UpdateTableViewData();
		}

		private void hideAutoCompleteView()
		{
			AutoCompleteTableView.Hidden = true;

			var parentTable = ViewToAddTo as UITableViewController;
			if (parentTable != null) //if there is a parenttable
			{
				parentTable.TableView.Bounces = true;
				//parentTable.TableView.ScrollEnabled = true;
				parentTable.TableView.AllowsSelection = true;
				//parentTable.TableView.UserInteractionEnabled = true;

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

