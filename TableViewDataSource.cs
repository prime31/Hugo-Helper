using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AppKit;
using Foundation;
using YamlDotNet.Serialization;


namespace HugoHelper
{
	public class TableViewDataSource : NSTableViewDataSource
	{
		public List<BlogPost> blogPosts = new List<BlogPost>();
		List<BlogPost> _allBlogPosts = new List<BlogPost>();
		bool _isShowingDrafts = true;
		NSTableView _tableView;
		bool _lastSortAscending;
		char[] _pathSeperators = new char[] { Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar };


		public TableViewDataSource( NSTableView tableView )
		{
			_tableView = tableView;
			_tableView.SortDescriptors = new NSSortDescriptor[] {
				new NSSortDescriptor( "date", false )
			};
			reloadData();

			NSNotificationCenter.DefaultCenter.AddObserver( Constants.siteRebuiltNotificationKey.NSString(), n =>
			{
				BeginInvokeOnMainThread( () => reloadData() );
			} );
		}


		public void reloadData()
		{
			_allBlogPosts.Clear();

			// only load from disk if we have a proper path
			if( Constants.hugoProjectPath != null )
			{
				var path = Constants.hugoContentPath;
				var files = Directory.GetFiles( path, "*.md", SearchOption.AllDirectories );
				foreach( var file in files )
				{
					var post = parseFrontMatter( file );

					// add the path
					post.pathToFile = file;

					// extract the archetype. if we have a first folder that is what we use
					var relativePath = file.Replace( path, string.Empty ).TrimStart( _pathSeperators );
					var pathParts = relativePath.Split( _pathSeperators );

					// if we have more than 1 part left (filename) then the first of them is our archetype folder
					if( pathParts.Length > 1 )
						post.archetype = pathParts[0];

					_allBlogPosts.Add( post );
				}
			}

			setShowDrafts( _isShowingDrafts );
			sortPosts( _tableView.SortDescriptors[0].Key, _lastSortAscending );
		}


		BlogPost parseFrontMatter( string path )
		{
			var file = File.ReadAllText( path );

			// we need YAML front matter!
			if( file.IndexOf( "---" ) < 0 )
				return new BlogPost( "---- This file does not have YAML front matter! Fix it! ---", DateTime.Now, true );

			var index = file.IndexOf( "---", file.IndexOf( "---" ) + 4 );

			var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
			var post = deserializer.Deserialize<BlogPost>( file.Substring( 0, index ) );

			return post;
		}


		public void setShowDrafts( bool showDrafts )
		{
			_isShowingDrafts = showDrafts;
			blogPosts = _allBlogPosts.Where( post => post.draft == _isShowingDrafts ).ToList();
		}


		public override nint GetRowCount( NSTableView tableView )
		{
			return blogPosts.Count;
		}


		#region Sorting

		void sortByDate( bool ascending )
		{
			if( ascending )
				blogPosts.Sort( ( x, y ) => x.date.CompareTo( y.date ) );
			else
				blogPosts.Sort( ( x, y ) => -1 * x.date.CompareTo( y.date ) );
		}


		void sortByTitle( bool ascending )
		{
			if( ascending )
				blogPosts = blogPosts.OrderBy( p => p.title ).ThenByDescending( p => p.date ).ToList();
			else
				blogPosts = blogPosts.OrderByDescending( p => p.title ).ThenByDescending( p => p.date ).ToList();
		}


		void sortByArchetype( bool ascending )
		{
			if( ascending )
				blogPosts = blogPosts.OrderBy( p => p.archetype ).ThenByDescending( p => p.date ).ToList();
			else
				blogPosts = blogPosts.OrderByDescending( p => p.archetype ).ThenByDescending( p => p.date ).ToList();
		}


		public void sortPosts( string key, bool ascending )
		{
			Console.WriteLine( "Sort: {0} -> {1}", key, ascending );

			_lastSortAscending = ascending;

			if( key == "title" )
				sortByTitle( ascending );
			else if( key == "archetype" )
				sortByArchetype( ascending );
			else
				sortByDate( ascending );
		}


		public override void SortDescriptorsChanged( NSTableView tableView, NSSortDescriptor[] oldDescriptors )
		{
			var tbSort = tableView.SortDescriptors;
			sortPosts( tbSort[0].Key, tbSort[0].Ascending );

			// Refresh table
			tableView.ReloadData();
		}

		#endregion

	}
}
