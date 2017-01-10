using System;
using System.Globalization;
using Docller.Core.Models;

namespace Docller.Core.Exceptions
{
    public class InvalidBlobException : Exception
    {
        public InvalidBlobException(BlobBase blob, string message):base(string.Format(CultureInfo.InvariantCulture, "Blob {0} is not valid, {1}",blob.FileName,message))
        {
            
        }
    }
}