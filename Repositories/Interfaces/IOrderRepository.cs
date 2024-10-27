using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<bool> CheckBuyProductAlready(Guid accountId, Guid productId);
        Task<Order> GetOrderByAccount(Guid accountId);
    }
}
