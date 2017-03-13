using System;

namespace GUI.Model
{
	public interface IDataService
	{
		void GetData ( Action<FileItem, Exception> callback );
	}
}