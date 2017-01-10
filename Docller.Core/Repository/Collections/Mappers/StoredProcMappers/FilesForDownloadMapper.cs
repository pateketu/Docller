using System.Collections.Generic;
using System.Data;
using Docller.Core.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers.StoredProcMappers
{
    public class FilesForDownloadMapper : FilesMapper
    {
        public FilesForDownloadMapper(IRowMapper<File> fileMapper):base(fileMapper)
        {
            
        }
        public override IEnumerable<File> MapSet(IDataReader reader)
        {
            Dictionary<long, File> dictionary = new Dictionary<long, File>();
        
            using (reader)
            {
                dictionary = this.Map(reader);
            }
            return dictionary.Values;
        }
    }
}