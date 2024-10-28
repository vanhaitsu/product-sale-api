using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.OrderModels
{
    public class UpdateOrderStatusModel
    {
        public Guid OrderId { get; set; }
        public string VnPayResponseCode { get; set; }

    }
}
