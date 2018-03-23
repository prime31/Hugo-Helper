using System;


namespace HugoHelper
{
	public class BlogPost
	{
		public string pathToFile;
		public string archetype;

		public string title { get; set; }
		public DateTime date { get; set; }
		public bool draft { get; set; }
		public string[] tags { get; set; }


		public BlogPost()
		{
		}


		public BlogPost( string name, DateTime date, bool isDraft )
		{
			this.title = name;
			this.date = date;
			this.draft = isDraft;
		}


		public string getTagsForDisplay()
		{
			if( tags != null && tags.Length > 0 )
				return string.Join( ", ", tags );
			return string.Empty;
		}


		public override string ToString()
		{
			var sb = new System.Text.StringBuilder();

			var cleanedPath = pathToFile.Replace( Constants.hugoProjectPath, string.Empty );
			sb.Append( cleanedPath );

			if( tags != null && tags.Length > 0 )
				sb.AppendFormat( "\nTags: " + string.Join( ", ", tags ) );
			
			return sb.ToString();
		}
	}
}
