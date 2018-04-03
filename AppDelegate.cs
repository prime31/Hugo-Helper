using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using Foundation;


namespace HugoHelper
{
	[Register( "AppDelegate" )]
	public partial class AppDelegate : NSApplicationDelegate, INSOpenSavePanelDelegate
	{
		public ViewController viewController;
		static Process _serverProcess;
		static NSToolbarItem _serverToolbarItem;
		static NSImage _serverToolbarDefaultImage;


		public override void DidFinishLaunching( NSNotification notification )
		{}


		public override void WillTerminate( NSNotification notification )
		{
			stopServer();
		}


		static string getPathToHugo()
		{
			var files = Directory.GetFiles( "/usr/local/Cellar/hugo/", "hugo", SearchOption.AllDirectories );
			if( files.Length == 0 )
			{
				showAlert( "Could not location a hugo installed on your system at '/usr/local/Cellar/hugo/'", "Aborting" );
				throw new Exception( "Could not find hugo installed at '/usr/local/Cellar/hugo/'" );
			}

			return files.Last();
		}


		public static void openBlogPost( BlogPost post )
		{
			var app = Constants.markdownAppPath;
			if( app == null )
			{
				showAlert( "A Markdown app has not been set. Use the File menu to select the app you want to use", "No Markdown App Set" );
				return;
			}

			var startInfo = new ProcessStartInfo
			{
				Arguments = post.pathToFile,
				FileName = app,
				WindowStyle = ProcessWindowStyle.Hidden
			};

			using( var proc = Process.Start( startInfo ) )
			{
				Console.WriteLine( "all done with opening blog post" );
			}
		}


		[Action( "validateMenuItem:" )]
		public bool ValidateMenuItem( NSMenuItem item )
		{
			switch( item.Tag )
			{
				case 4:
					return !Constants.storePostsInYearSubdirectory;
				case 5:
					return Constants.storePostsInYearSubdirectory;
			}
			return true;
		}


		partial void onClickDisableBlogPostSubfolder( Foundation.NSObject sender )
		{
			Constants.storePostsInYearSubdirectory = false;
		}


		partial void onClickEnableBlogPostSubfolder( Foundation.NSObject sender )
		{
			Constants.storePostsInYearSubdirectory = true;
		}


		partial void onClickOpenConfigFile( Foundation.NSObject sender )
		{
			var app = Constants.markdownAppPath;
			if( app == null )
			{
				showAlert( "A Markdown app has not been set. Use the File menu to select the app you want to use", "No Markdown App Set" );
				return;
			}

			var startInfo = new ProcessStartInfo
			{
				Arguments = Path.Combine( Constants.hugoProjectPath, "config.toml" ),
				FileName = app,
				WindowStyle = ProcessWindowStyle.Hidden
			};

			using( var proc = Process.Start( startInfo ) )
				Console.WriteLine( "all done with opening config.toml" );
		}


		partial void onClickOpenHugoFolder( Foundation.NSObject sender )
		{
			var app = Constants.markdownAppPath;
			if( app == null )
			{
				showAlert( "A Markdown app has not been set. Use the File menu to select the app you want to use", "No Markdown App Set" );
				return;
			}

			var startInfo = new ProcessStartInfo
			{
				Arguments = Constants.hugoProjectPath,
				FileName = app,
				WindowStyle = ProcessWindowStyle.Hidden
			};

			using( var proc = Process.Start( startInfo ) )
				Console.WriteLine( "all done with opening Hugo folder" );
		}


		public static void startStopServer( NSToolbarItem item )
		{
			_serverToolbarItem = item;
			if( _serverToolbarDefaultImage == null )
				_serverToolbarDefaultImage = item.Image;

			// stop the server if it is running
			if( _serverProcess != null )
			{
				stopServer();
				item.Image = _serverToolbarDefaultImage;
				return;
			}


			item.Image = NSImage.ImageNamed( "server-on.png" );

			var startInfo = new ProcessStartInfo
			{
				Arguments = "server -D",
				FileName = getPathToHugo(),
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				WorkingDirectory = Constants.hugoProjectPath
			};

			_serverProcess = new Process();
			_serverProcess.StartInfo = startInfo;

			_serverProcess.OutputDataReceived += ( sender, args ) =>
			{
				if( args.Data == null )
					return;
				
				Console.WriteLine( args.Data );
				NSNotificationCenter.DefaultCenter.PostNotification( NSNotification.FromName( Constants.logNotificationKey, new NSString( args.Data ) ) );


				if( args.Data.Contains( "Web Server is available" ) )
				{
					// get the port number and open the web page
					var match = new Regex( @"\d+" ).Match( args.Data );
					if( match.Success )
						Process.Start( "http://localhost:" + match.Value );
				}
				else if( args.Data.Contains( "Press Ctrl+C to stop" ) )
				{
					Console.WriteLine( "server appears to be running" );
				}
				else if( args.Data.Contains( "rebuilding site" ) )
				{
					NSNotificationCenter.DefaultCenter.PostNotification( NSNotification.FromName( Constants.siteRebuiltNotificationKey, null ) );
				}
			};

			try
			{
				_serverProcess.Start();
				_serverProcess.BeginOutputReadLine();
				Console.ReadLine();
				//stdError = _serverProcess.StandardError.ReadToEnd();
				//_serverProcess.WaitForExit();
			}
			catch( Exception e )
			{
				throw new Exception( "OS error while executing " + e.Message, e );
			}
		}


		static void stopServer()
		{
			if( _serverProcess == null )
				return;

			try
			{
				_serverProcess.StandardInput.WriteLine( "\x3" );
				_serverProcess.StandardInput.Close();
				_serverProcess.Close();
			}
			catch( Exception e )
			{
				Console.WriteLine( "failed to kill server: " + e );
			}

			_serverProcess = null;

			// kill it dead
			Process.Start( "killall", "hugo" );
		}


		public static void switchTheme( string theme )
		{
			var tomlPath = Path.Combine( Constants.hugoProjectPath, "config.toml" );
			var lines = File.ReadAllLines( tomlPath );

			for( var i = 0; i < lines.Length; i++ )
			{
				if( lines[i].StartsWith( "theme" ) )
				{
					lines[i] = string.Format( "theme = \"{0}\"", theme );
					break;
				}
			}

			File.WriteAllLines( tomlPath, lines );

			// restart the server
			var winCon = NSApplication.SharedApplication.KeyWindow.WindowController as WindowController;
			stopServer();
			startStopServer( winCon.startServerToolbarItemPublic );
		}


		partial void onClickSetMarkdownApp( NSObject sender )
		{
			var openPanel = NSOpenPanel.OpenPanel;
			openPanel.Delegate = new OpenSavePanelFilter();
			openPanel.FloatingPanel = true;
			openPanel.Title = "Choose Your Markdown App";
			openPanel.CanChooseDirectories = true;
			openPanel.AllowedFileTypes = new string[] { "app" };
			openPanel.Directory = "/Applications";

			openPanel.BeginSheet( NSApplication.SharedApplication.KeyWindow, result =>
			{
				if( result == 1 )
				{
					var path = openPanel.Url.AbsoluteString.Replace( "file://", string.Empty );

					// find the full path to the executable
					var executablePath = Path.Combine( path, "Contents/MacOS" ).Replace( "%20", " " );
					var executable = Directory.GetFiles( executablePath ).First();

					if( File.Exists( executable ) )
					{
						Console.WriteLine( "all good: " + executable );
						Constants.markdownAppPath = executable;
						NSUserDefaults.StandardUserDefaults.Synchronize();
					}
					else
					{
						showAlert( "You must choose an application", "Invalid Application" );
					}
				}
			} );
		}


		partial void onClickSetHugoFolder( AppKit.NSMenuItem sender )
		{
			var openPanel = NSOpenPanel.OpenPanel;
			openPanel.Title = "Choose Your Hugo Root Folder";
			openPanel.CanChooseFiles = false;
			openPanel.CanChooseDirectories = true;

			openPanel.BeginSheet( NSApplication.SharedApplication.KeyWindow, result =>
			{
				if( result == 1 )
				{
					var path = openPanel.DirectoryUrl.Path;
					var contentPath = Constants.hugoContentPath;
					if( Directory.Exists( contentPath ) )
					{
						Console.WriteLine( openPanel.DirectoryUrl.Path );
						Console.WriteLine( "all good: " + contentPath );
						Constants.hugoProjectPath = path;
						NSUserDefaults.StandardUserDefaults.Synchronize();

						viewController.reloadDataSource();
					}
					else
					{
						showAlert( "This does not appear to be a Hugo blog.", "Invalid Directory" );
					}
				}
			} );
		}


		public void addNewBlogPost()
		{
			onClickNewBlogPost( null );
		}


		partial void onClickNewBlogPost( Foundation.NSObject sender )
		{
			// first prompt for the name of the post
			var alert = NSAlert.WithMessage( "Enter the page file name", "OK", "Cancel", null, "Enter the filename for your new page" );
			var postFilenameInput = new NSTextField( new CoreGraphics.CGRect( 0, 30, 250, 24 ) );
			postFilenameInput.PlaceholderString = "Page filename (excluding extension)";

			var archetypeInput = new NSTextField( new CoreGraphics.CGRect( 0, 0, 250, 24 ) );
			archetypeInput.PlaceholderString = "Page archetype (defaults to 'posts')";

			// create a combobox and populate it with any found archetypes
			var archetypeComboBox = new NSComboBox( new CoreGraphics.CGRect( 0, 0, 250, 24 ) );
			archetypeComboBox.VisibleItems = 12;
			foreach( var folder in Directory.GetDirectories( Constants.hugoContentPath ) )
				archetypeComboBox.Add( Path.GetFileName( folder ).NSString() );

			var view = new NSView( new CGRect( 0, 0, 250, 60 ) );
			view.AddSubview( postFilenameInput );
			view.AddSubview( archetypeComboBox );

			alert.AccessoryView = view;

			BeginInvokeOnMainThread( async () =>
			{
				var result = await alert.BeginSheetAsync( NSApplication.SharedApplication.KeyWindow );
				if( result == NSModalResponse.OK )
				{
					var filename = postFilenameInput.StringValue;
					filename = filename.Replace( " ", "-" );

					if( filename.Length == 0 )
					{
						await Task.Delay( 500 ).ContinueWith( t =>
						{
							BeginInvokeOnMainThread( () => showAlert( "Invalid filename found", "We aren't going to create a new page with that slop!" ) );
						} );
						return;
					}

					if( !filename.EndsWith( ".md" ) )
						filename = filename + ".md";

					var archetype = archetypeComboBox.StringValue;
					if( string.IsNullOrEmpty( archetype ) )
						archetype = "posts";

					var subfolder = Constants.storePostsInYearSubdirectory ? DateTime.Now.ToString( "yyyy" ) : null;
					createBlogPostFile( filename, archetype, subfolder );
				}
			} );
		}


		void createBlogPostFile( string filename, string archtype = "posts", string subfolder = null )
		{
			var subfolderStr = subfolder != null ? subfolder + "/" : string.Empty;
			var startInfo = new ProcessStartInfo
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				Arguments = string.Format( "new {0}/{1}{2}", archtype, subfolderStr, filename ),
				FileName = getPathToHugo(),
				WorkingDirectory = Constants.hugoProjectPath,
				WindowStyle = ProcessWindowStyle.Hidden
			};

			using( var proc = Process.Start( startInfo ) )
			{
				proc.WaitForExit();
				Console.WriteLine( "all done creating new blog post" );
			}

			viewController.reloadDataSource();
		}


		partial void onClickStartHugulpWatcher( NSObject sender )
		{
			var command = string.Format( "cd {0} && hugulp watch", Constants.hugoProjectPath );
			runCommandInTerminal( command );
		}


		partial void onClickBuildSite( Foundation.NSObject sender )
		{
			var command = string.Format( "cd {0} && rm -rf public && hugo && hugulp build", Constants.hugoProjectPath );
			runCommandInTerminal( command );
		}


		partial void onClickShowServerLogs( Foundation.NSObject sender )
		{
			var winCon = NSApplication.SharedApplication.KeyWindow.WindowController as WindowController;
			winCon.showServerLogs();
		}


		void runCommandInTerminal( string command )
		{
			command = string.Format( "-e 'tell application \"Terminal\" to do script \"{0}\"'", command );

			var p = new Process();
			p.StartInfo.FileName = "osascript";
			p.StartInfo.WorkingDirectory = Constants.hugoProjectPath;
			p.StartInfo.Arguments = command;
			p.StartInfo.CreateNoWindow = false;
			p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
			p.StartInfo.UseShellExecute = true;

			p.Start();
			p.WaitForExit();
		}


		static void showAlert( string message, string informativeText )
		{
			var alert = new NSAlert()
			{
				AlertStyle = NSAlertStyle.Informational,
				InformativeText = informativeText,
				MessageText = message,
			};
			alert.AddButton( "OK" );
			alert.BeginSheet( NSApplication.SharedApplication.KeyWindow );
		}

	}
}
