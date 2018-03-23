using System;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;


namespace HugoHelper
{
	public class TableViewDelegate : NSTableViewDelegate
	{
		const string kCellIdentifier = "BlogPostCell";
		TableViewDataSource dataSource;
		NSTextFieldCell extraTextFieldCell;
		CGRect tallRect;


		public TableViewDelegate( TableViewDataSource dataSource )
		{
			this.dataSource = dataSource;
			this.extraTextFieldCell = new NSTextFieldCell();
		}


		public override NSView GetViewForItem( NSTableView tableView, NSTableColumn tableColumn, nint row )
		{
			// This pattern allows you reuse existing views when they are no-longer in use.
			// If the returned view is null, you instance up a new view
			// If a non-null view is returned, you modify it enough to reflect the new data
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

			var blogPost = dataSource.blogPosts[(int)row];
			view.ToolTip = blogPost.ToString();

			// Setup view based on the column selected
			switch( tableColumn.Title )
			{
				case "Title":
					view.StringValue = blogPost.title;
				break;
				case "Date":
					view.StringValue = blogPost.date.ToString( "MM/dd/yyyy" );
				break;
				case "Tags":
				view.StringValue = blogPost.getTagsForDisplay();
				break;
				case "Archetype":
					view.StringValue = blogPost.archetype;
				break;
			}

			return view;
		}


		public override nfloat GetRowHeight( NSTableView tableView, nint row )
		{
			//if( tallRect.Size.Width == 0 )
			{
				var firstColumn = tableView.TableColumns()[0];
				tallRect = new CGRect( 0, 0, firstColumn.Width, float.MaxValue );
			}

			var post = dataSource.blogPosts[(int)row];
			extraTextFieldCell.Title = post.title;

			var result = extraTextFieldCell.CellSizeForBounds( tallRect ).Height + 5;

			if( result < tableView.RowHeight )
				result = tableView.RowHeight;

			return result;
		}


		public override nint GetNextTypeSelectMatch( NSTableView tableView, nint startRow, nint endRow, string searchString )
		{
			nint row = 0;
			foreach( var blogPost in dataSource.blogPosts )
			{
				if( blogPost.title.Contains( searchString ) )
					return row;
				++row;
			}

			return 0;
		}


		public override bool ShouldSelectRow( NSTableView tableView, nint row )
		{
			var post = dataSource.blogPosts[(int)row];
			AppDelegate.openBlogPost( post );

			Task.Delay( 1000 ).ContinueWith( t =>
			{
				BeginInvokeOnMainThread( () => tableView.DeselectAll( this ) );
			} );

			return true;
		}

	}
}
