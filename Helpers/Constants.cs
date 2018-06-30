using System.IO;
using Foundation;


namespace HugoHelper
{
	public static class Constants
	{
		const string _defaultsPathKey = "hugo-path";
		const string _markdownAppPathKey = "markdown-app-path";
		const string _storePostsInYearSubdirectory = "store-posts-in-year-subdir";

		public static string logNotificationKey = "log-notification";
		public static string siteRebuiltNotificationKey = "site-rebuilt-notification";
		public static string serverStartedNotificationKey = "server-started-notification";
		public static string serverFoundUrlNotificationKey = "server-found-url-notification";
		public static string serverStoppedNotificationKey = "server-stopped-notification";

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


		public static bool storePostsInYearSubdirectory
		{
			get { return NSUserDefaults.StandardUserDefaults.BoolForKey( _storePostsInYearSubdirectory ); }
			set { NSUserDefaults.StandardUserDefaults.SetBool( value, _storePostsInYearSubdirectory ); }
		}
	}
}
