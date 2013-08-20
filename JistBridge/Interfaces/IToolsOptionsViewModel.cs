using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JistBridge.Interfaces
{
	internal interface IToolsOptionsViewModel
	{
		ICidneOptionsViewModel CidneOptionsViewModel { get; set; }
		IAnbkOptionsViewModel AnbkOptionsViewModel { get; set; }
	}
}
