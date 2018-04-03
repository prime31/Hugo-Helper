using System;
using AppKit;
using Foundation;


namespace HugoHelper
{
	public partial class ViewController : NSViewController
	{
		public ViewController( IntPtr handle ) : base( handle )
		{
			( NSApplication.SharedApplication.Delegate as AppDelegate ).viewController = this;
		}


		public override void ViewWillAppear()
		{
			base.ViewWillAppear();
			//View.Window.Appearance = NSAppearance.GetAppearance( NSAppearance.NameVibrantDark );
		}


		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			tableView.SortDescriptors = new NSSortDescriptor[] { new NSSortDescriptor( "date", false ) };

			// Create the Table Data Source and populate it
			var dataSource = new TableViewDataSource( tableView );

			tableView.DataSource = dataSource;
			tableView.Delegate = new TableViewDelegate( dataSource );
		}


		public void onSegmentedControlChanged( AppKit.NSSegmentedControl sender )
		{
			Console.WriteLine( "changed: " + sender.GetLabel( sender.SelectedSegment ) );

			( tableView.DataSource as TableViewDataSource ).setShowDrafts( sender.GetLabel( sender.SelectedSegment ) == "Drafts" );
			tableView.ReloadData();
		}


		public void reloadDataSource()
		{
			( tableView.DataSource as TableViewDataSource ).reloadData();
			tableView.ReloadData();
		}

	}
}
