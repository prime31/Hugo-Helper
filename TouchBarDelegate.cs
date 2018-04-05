using System;
using AppKit;
using Foundation;


namespace HugoHelper
{
	public class TouchBarDelegate : NSTouchBarDelegate
	{
		AppDelegate appDelegate { get { return NSApplication.SharedApplication.Delegate as AppDelegate; } }

		const string _identifierPrefix = "com.prime31.";
		NSCustomTouchBarItem _serverItem;


		public TouchBarDelegate()
		{
			NSNotificationCenter.DefaultCenter.AddObserver( Constants.serverStartedNotificationKey.NSString(), note =>
			{
				BeginInvokeOnMainThread( () => {
					var button = _serverItem.View as NSButton;
					button.Title = "Stop Server";
					button.Image = NSImage.ImageNamed( NSImageName.StatusAvailable );
				} );
			} );

			NSNotificationCenter.DefaultCenter.AddObserver( Constants.serverStoppedNotificationKey.NSString(), note =>
			{
				BeginInvokeOnMainThread( () => {
					var button = _serverItem.View as NSButton;
					button.Title = "Start Server";
					button.Image = NSImage.ImageNamed( NSImageName.StatusUnavailable );
				} );
			} );
		}


		public string[] getDefaultIdentifiers()
		{
			return new string[] { _identifierPrefix + "0", _identifierPrefix + "1",
				"NSTouchBarItemIdentifierFixedSpaceLarge",
				_identifierPrefix + "2",
				"NSTouchBarItemIdentifierFlexibleSpace",
				_identifierPrefix + "3" };
		}


		public override NSTouchBarItem MakeItem( NSTouchBar touchBar, string identifier )
		{
			try
			{
				var id = int.Parse( identifier.Replace( _identifierPrefix, string.Empty ) );

				switch( id )
				{
					case 0:
						return makeServerButton( identifier );
					case 1:
						return makeStartHugulpButton( identifier );
					case 2:
						return makeAddNewPostButton( identifier );
					case 3:
						return makeOpenFolderButton( identifier );
				}
			}
			catch( Exception e )
			{
				Console.WriteLine( e );
			}

			return null;
		}


		NSTouchBarItem makeServerButton( string identifier )
		{
			_serverItem= new NSCustomTouchBarItem( identifier );
			var button = NSButton.CreateButton( "Start Server", () => AppDelegate.startStopServer() );
			button.Image = NSImage.ImageNamed( NSImageName.StatusUnavailable );
			button.ImagePosition = NSCellImagePosition.ImageLeft;
			_serverItem.View = button;

			return _serverItem;
		}


		NSTouchBarItem makeAddNewPostButton( string identifier )
		{
			var item = new NSCustomTouchBarItem( identifier );
			var button = NSButton.CreateButton( "New Post", () => appDelegate.addNewBlogPost() );
			button.Image = NSImage.ImageNamed( NSImageName.TouchBarAddTemplate );
			button.ImagePosition = NSCellImagePosition.ImageLeft;
			item.View = button;

			return item;
		}


		NSTouchBarItem makeStartHugulpButton( string identifier )
		{
			var item = new NSCustomTouchBarItem( identifier );
			var button = NSButton.CreateButton( "Start hugulp Watcher", () => appDelegate.startHugulpWatcher() );
			item.View = button;

			return item;
		}


		NSTouchBarItem makeOpenFolderButton( string identifier )
		{
			var item = new NSCustomTouchBarItem( identifier );
			var button = NSButton.CreateButton( "Open Hugo Folder", () => appDelegate.openHugoFolder() );
			button.Image = NSImage.ImageNamed( NSImageName.TouchBarFolderTemplate );
			button.ImagePosition = NSCellImagePosition.ImageLeft;
			item.View = button;

			return item;
		}

	}
}
