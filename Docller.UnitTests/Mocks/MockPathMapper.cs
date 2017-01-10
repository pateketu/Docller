using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docller.Core.Common;

namespace Docller.UnitTests.Mocks
{
    public class MockPathMapper:IPathMapper 
    {
        public string MapPath(string relativePath)
        {
            return @"C:\Docller\Source\Docller.UI\Views\Mail\TransmittalEmail.html.cshtml";
        }
    }
}
