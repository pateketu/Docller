using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Models
{
    public class Files:PageableData<File>, ISortable<FileSortBy>
    {
        public FileSortBy SortBy { get; set; }
        public SortDirection Direction { get; set; }
    }

    public interface ISortable<T>
    {
        T SortBy { get; set; }
        SortDirection Direction { get; set; }
        
    }
}
