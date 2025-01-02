using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMan.Infrastructure.Data.Pagination
{
    public class MyPagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public MyPagedResult(IEnumerable<T> items, int totalItems, int pageNumber, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        // Constructor sin parámetros para permitir inicialización de objeto
        public MyPagedResult()
        {
            Items = new List<T>();
        }

        // Método de utilidad para crear una instancia vacía
        public static MyPagedResult<T> Empty()
        {
            return new MyPagedResult<T>
            {
                Items = Enumerable.Empty<T>(),
                TotalItems = 0,
                PageNumber = 1,
                PageSize = 10
            };
        }
    }
}
