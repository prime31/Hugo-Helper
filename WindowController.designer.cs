// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace HugoHelper
{
	[Register ("WindowController")]
	partial class WindowController
	{
		[Outlet]
		AppKit.NSToolbarItem startServerToolbarItem { get; set; }

		[Action ("onClickNewBlogPost:")]
		partial void onClickNewBlogPost (AppKit.NSToolbarItem sender);

		[Action ("onClickReload:")]
		partial void onClickReload (Foundation.NSObject sender);

		[Action ("onClickStartServer:")]
		partial void onClickStartServer (Foundation.NSObject sender);

		[Action ("onClickThemePicker:")]
		partial void onClickThemePicker (Foundation.NSObject sender);

		[Action ("onSegmentedControlChanged:")]
		partial void onSegmentedControlChanged (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (startServerToolbarItem != null) {
				startServerToolbarItem.Dispose ();
				startServerToolbarItem = null;
			}
		}
	}
}
