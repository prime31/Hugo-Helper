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
	[Register ("LogViewController")]
	partial class LogViewController
	{
		[Outlet]
		AppKit.NSTextView textView { get; set; }

		[Action ("onClickClear:")]
		partial void onClickClear (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (textView != null) {
				textView.Dispose ();
				textView = null;
			}
		}
	}
}
