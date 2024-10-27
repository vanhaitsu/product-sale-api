using Repositories.Entities;
using Repositories.Interfaces;
using Repositories.Models.CartItemModels;
using Services.Common;
using Services.Interfaces;
using Services.Models.CartItemModels;
using Services.Models.CartModels;
using Services.Models.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Pagination<GetAllOrdersModel>> GetAllOrderByAccounts(OrderFilterModel orderFilterModel)
        {
            var checkOrderExist = await _unitOfWork.OrderRepository.GetOrderByAccount(orderFilterModel.userId);
            if (checkOrderExist == null)
            {
                return new Pagination<GetAllOrdersModel>(
                          new List<GetAllOrdersModel>(),
                          orderFilterModel.PageIndex,
                          orderFilterModel.PageSize, 0);
            }
            var userId = checkOrderExist.AccountID;
            var ordersResult = await _unitOfWork.OrderRepository.GetAllAsync(
               filter: _ => _.AccountID == userId,
               include: "OrderCartItems",
               pageIndex: orderFilterModel.PageIndex,
               pageSize: orderFilterModel.PageSize
           );
            if (ordersResult != null || ordersResult.Data.Any())
            {
                var orderModelList = ordersResult.Data.Select(_ => new GetAllOrdersModel
                {
                    OrderID = _.Id,
                    AccountID = _.AccountID,
                    BillingAddress = _.BillingAddress,
                    CreateDate = _.CreationDate,
                    OrderDate = _.OrderDate,
                    TotalPrice = _.OrderCartItems.Sum(_ => _.Quantity * _.Price),
                    PaymentMethod = (int)_.PaymentMethod,
                    Status = (int)_.Status,
                    OrderCartItemModels = _.OrderCartItems.Select(_ => new OrderCartItemModel
                    {
                        ProductSizeID = _.ProductSizeID,
                        Quantity = _.Quantity,
                        Price = _.Price,
                        OrderStatus = (int)_.OrderStatus
                    }).ToList()
                }).ToList();

                return new Pagination<GetAllOrdersModel>(
                    orderModelList,
                    orderFilterModel.PageIndex,
                    orderFilterModel.PageSize,
                    ordersResult.TotalCount
                );
            }

            return null;
        }
    }
}
