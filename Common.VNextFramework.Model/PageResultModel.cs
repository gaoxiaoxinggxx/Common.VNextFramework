using System.Collections.Generic;

namespace Common.VNextFramework.Model
{
    public class PageResultModel<T>
    {
        public PageResultModel() { }
        public PageResultModel(int pageIndex, int pageSize, int totalCount, List<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Total = totalCount;
            Data = data;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<T> Data { get; set; }
    }
}