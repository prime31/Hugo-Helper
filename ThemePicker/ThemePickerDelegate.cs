using System;
using AppKit;


namespace HugoHelper
{
	public class ThemePickerDelegate : NSTableViewDelegate
	{
		public Action<string> onThemeChosen;

		const string kCellIdentifier = "ThemeCell";
		ThemePickerDataSource dataSource;


		public ThemePickerDelegate( ThemePickerDataSource dataSource )
		{
			this.dataSource = dataSource;
		}


		public override NSView GetViewForItem( NSTableView tableView, NSTableColumn tableColumn, nint row )
		{
			var view = (NSTextField)tableView.MakeView( kCellIdentifier, this );
			if( view == null )
			{
				view = new NSTextField();
				view.Identifier = kCellIdentifier;
				view.BackgroundColor = NSColor.Clear;
				view.Bordered = false;
				view.Selectable = false;
				view.Editable = false;
			}

			// Setup view based on the column selected
			view.StringValue = dataSource.themes[(int)row];

			return view;
		}


		public override bool ShouldSelectRow( NSTableView tableView, nint row )
		{
			var theme = dataSource.themes[(int)row];
			if( onThemeChosen != null )
				onThemeChosen( theme );
			
			Console.WriteLine( "theme: " + theme );

			return true;
		}
	}
}
