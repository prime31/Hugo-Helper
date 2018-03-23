using System;
using Foundation;
using AppKit;


namespace HugoHelper
{
	public partial class LogViewController : NSViewController
	{
		System.Text.StringBuilder _stringBuilder = new System.Text.StringBuilder();


		public LogViewController( IntPtr handle ) : base( handle )
		{
			NSNotificationCenter.DefaultCenter.AddObserver( Constants.logNotificationKey.NSString(), note =>
			{
				_stringBuilder.AppendLine( note.Object as NSString );
				BeginInvokeOnMainThread( () =>
				{
					textView.Value = _stringBuilder.ToString();
					textView.ScrollRangeToVisible( new NSRange( textView.Value.Length, 0 ));
				});
			} );
		}


		partial void onClickClear( AppKit.NSButton sender )
		{
			_stringBuilder.Clear();
			textView.Value = string.Empty;
		}
	}
}
