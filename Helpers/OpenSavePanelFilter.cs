using System;
using AppKit;
using Foundation;


namespace HugoHelper
{
	public class OpenSavePanelFilter : NSOpenSavePanelDelegate
	{
		public override bool ValidateUrl( NSSavePanel panel, NSUrl url, out NSError outError )
		{
			outError = null;
			return url.AbsoluteUrl.AbsoluteString.EndsWith( ".app" );
		}
	}
}
