using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlashSale.Core.Entities
{
    public class FlashSaleEventEntity : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; }

        public ICollection<FlashSaleItem> FlashSaleItems { get; set; } = new List<FlashSaleItem>();
    }
}
