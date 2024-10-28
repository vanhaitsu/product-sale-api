using Microsoft.AspNetCore.Http;
using Repositories.Models.OrderModels;
using Repositories.Models.PaymentModels;
using Services.Models.OrderModels;
using Services.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IPaymentService
    {
        Task<ResponseModel> VNPayMethod(OrderModel orderModel, HttpContext context);
        Task<ResponseModel> UpdateOrderStatus(UpdateOrderStatusModel updateOrderStatusModel);
    }

}
