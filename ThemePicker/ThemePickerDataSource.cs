using System;
using System.Collections.Generic;
using System.IO;
using AppKit;


namespace HugoHelper
{
	public class ThemePickerDataSource : NSTableViewDataSource
	{
		public List<string> themes = new List<string>();


		public ThemePickerDataSource()
		{
			foreach( var folder in Directory.GetDirectories( Path.Combine( Constants.hugoProjectPath, "themes" ) ) )
			{
				themes.Add( Path.GetFileName( folder ) );
			}
		}


		public override nint GetRowCount( NSTableView tableView )
		{
			return themes.Count;
		}

	}
}
