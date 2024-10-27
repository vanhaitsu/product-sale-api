using Repositories.Models.AccountModels;
using Services.Common;
using Services.Models.AccountModels;
using Services.Models.CartModels;
using Services.Models.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IOrderService
    {
        Task<Pagination<GetAllOrdersModel>> GetAllOrderByAccounts(OrderFilterModel orderFilterModel);
    }
}
