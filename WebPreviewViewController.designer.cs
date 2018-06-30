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
	[Register ("WebPreviewViewController")]
	partial class WebPreviewViewController
	{
		[Outlet]
		AppKit.NSButton backButton { get; set; }

		[Outlet]
		WebKit.WKWebView desktopWebView { get; set; }

		[Outlet]
		AppKit.NSButton forwardButton { get; set; }

		[Outlet]
		WebKit.WKWebView mobileWebView { get; set; }

		[Action ("onClickBackButton:")]
		partial void onClickBackButton (Foundation.NSObject sender);

		[Action ("onClickForwardButton:")]
		partial void onClickForwardButton (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (backButton != null) {
				backButton.Dispose ();
				backButton = null;
			}

			if (forwardButton != null) {
				forwardButton.Dispose ();
				forwardButton = null;
			}

			if (desktopWebView != null) {
				desktopWebView.Dispose ();
				desktopWebView = null;
			}

			if (mobileWebView != null) {
				mobileWebView.Dispose ();
				mobileWebView = null;
			}
		}
	}
}
