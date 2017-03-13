using System;

namespace GUI.Model
{
	public class DataService : IDataService
	{
		public void GetData ( Action<FileItem, Exception> callback )
		{
			// Use this to connect to the actual data service

			//var item = new FileItem ( "Welcome to MVVM Light" );
			//callback ( item, null );
		}
	}
}