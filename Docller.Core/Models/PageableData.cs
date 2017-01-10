using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Docller.Core.Models
{
    public class PageableData<T> : List<T>, IPageable
    {
        private int? _totalPages;

        public PageableData()
        {
            
        }

        public PageableData(IEnumerable<T> collection ):base(collection)
        {
            
        }
        public int TotalCount
        {
            get;internal set;
        }

        public int PageSize
        {
            get;internal set;
        }

        public int PageNumber { get; internal set; }
        public int TotalPages
        {
            get
            {
                if (_totalPages == null)
                {
                    _totalPages = GetTotalPages();
                }
                return _totalPages.Value;
            }
            
        }

        public int GetPreviousPage()
        {
            return PageNumber - 1;
        }

        public int GetNextPage()
        {
            return PageNumber + 1;
        }

        public bool IsActivePage(int pageNumber)
        {
            return PageNumber == pageNumber;
        }

        public bool HasPreviousPage
        {
            get
            {
                return (this.PageNumber > 1 && this.TotalPages > 0);
            }
        }


        public bool HasNextPage
        {
            get
            {
                return (this.PageNumber < this.TotalPages);
            }
        }

        public bool HasPages
        {
            get { return this.TotalPages > 0; }
        }

        private int GetTotalPages()
        {
            if (this.TotalCount > this.PageSize && this.PageSize > 0)
            {
                return (int)Math.Ceiling((this.TotalCount / (double)this.PageSize));
            }
            return 0;
        }
    }
}
