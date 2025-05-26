using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.Banding
{
    public class PaginationModel<T>
    {
        public PaginationModel()
        {
            Items = new List<T>();
        }
        public long Count { get; set; }
        public long PageIndex { get; set; }
        public long PageSize { get; set; }
        public List<T> Items { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
    }
}
