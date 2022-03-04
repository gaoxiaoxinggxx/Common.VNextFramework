using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.VNextFramework.Model
{
    public class PageQueryModel
    {
        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; }

        [Range(1, 100)]
        public int PageSize { get; set; }

        public IEnumerable<Sort> Sorts { get; set; }

        public IEnumerable<Filter> Filters { get; set; }
    }

    public class Sort
    {
        public string Field
        {
            get;
            set;
        }

        public bool Desc
        {
            get;
            set;
        }
    }

    public class Filter
    {
        public string Field
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }
    }
}