using System.IO;
using Foundation;


namespace HugoHelper
{
	public static class Constants
	{
		const string _defaultsPathKey = "hugo-path";
		const string _markdownAppPathKey = "markdown-app-path";

		public static string logNotificationKey = "log-notification";
		public static string siteRebuiltNotificationKey = "site-rebuilt-notification";

		public static string hugoContentPath { get { return Path.Combine( hugoProjectPath, "content" ); } }

		public static string hugoProjectPath
		{
			get { return NSUserDefaults.StandardUserDefaults.StringForKey( _defaultsPathKey ); }
			set { NSUserDefaults.StandardUserDefaults.SetString( value, _defaultsPathKey ); }
		}

		public static string markdownAppPath
		{
			get { return NSUserDefaults.StandardUserDefaults.StringForKey( _markdownAppPathKey ); }
			set { NSUserDefaults.StandardUserDefaults.SetString( value, _markdownAppPathKey ); }
		}
	}
}
