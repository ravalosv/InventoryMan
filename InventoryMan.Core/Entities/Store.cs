using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMan.Core.Entities
{
    public class Store
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Phone { get; set; } = default!;

        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}
