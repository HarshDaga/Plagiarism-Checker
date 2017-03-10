using System;
using GUI.Model;

namespace GUI.Design
{
	public class DesignDataService : IDataService
	{
		public void GetData ( Action<FileItem, Exception> callback )
		{
			// Use this to create design time data

			//var item = new FileItem ( "Welcome to MVVM Light [design]" );
			//callback ( item, null );
		}
	}
}