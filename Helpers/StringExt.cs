using Foundation;


namespace HugoHelper
{
	public static class StringExt
	{
		public static NSString NSString( this string self )
		{
			return new NSString( self );
		}
	}
}
