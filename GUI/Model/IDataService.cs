using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUI.Model
{
	public interface IDataService
	{
		void GetData ( Action<FileItem, Exception> callback );
	}
}
