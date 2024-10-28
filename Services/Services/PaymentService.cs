using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repositories.Entities;
using Repositories.Enums;
using Repositories.Interfaces;
using Repositories.Models.OrderModels;
using Repositories.Models.PaymentModels;
using Repositories.Models.VNPayModels;
using Services.Interfaces;
using Services.Models.OrderModels;
using Services.Models.ResponseModels;

namespace Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaymentGatewayService _paymentGatewayService;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IPaymentGatewayService paymentGatewayService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentGatewayService = paymentGatewayService;
        }
        public async Task<ResponseModel> VNPayMethod(OrderModel orderModel, HttpContext context)
        {
            var result = new ResponseModel();
            var cart = await _unitOfWork.CartRepository.GetByAccount(orderModel.AccountID);
            if (cart == null)
            {
                return new ResponseModel { Message = "Cart cannot be found.", Status = false };
            }
            if (cart.CartItems.Count == 0)
            {
                return new ResponseModel { Message = "Must have at least one product to checkout.", Status = false };
            }
            var checkInformation = await CheckInformation(orderModel.AccountID, 
                                                          orderModel.OrderCartItemModels.Select(_ => _.ProductSizeID).ToList(),
                                                          orderModel.OrderCartItemModels.Select(_ => _.Quantity).ToList());
            if(checkInformation == false)
            {
                return new ResponseModel { Message = "Incorrect data.", Status = false };
            }
            var checkStockProduct = await CheckStockQuantity(orderModel.AccountID,
                                                          orderModel.OrderCartItemModels.Select(_ => _.ProductSizeID).ToList(),
                                                          orderModel.OrderCartItemModels.Select(_ => _.Quantity).ToList());
            if (checkInformation == false)
            {
                return new ResponseModel { Message = "Incorrect data.", Status = false };
            }
            var user = cart.Account;
            if (user == null)
            {
                return new ResponseModel { Message = "User cannot be found.", Status = false };
            }
            var newOrder = CreateOrder(cart, user, orderModel);
            await _unitOfWork.OrderRepository.AddAsync(newOrder);
            int check = await _unitOfWork.SaveChangeAsync();
            if (check > 0)
            {
                var payment = new PaymentInformationModel
                {
                    AccountID = newOrder.AccountID.ToString(),
                    Amount = (double)newOrder.Payment.Amount,
                    CustomerName = cart.Account.FirstName + cart.Account.LastName,
                    BookingID = newOrder.Id.ToString(),
                };
                result.Message = "Payment successfully.";
                result.Status = true;
                result.Data = await _paymentGatewayService.CreatePaymentUrlVnpay(payment, context);
                return result;

            }
            return new ResponseDataModel<PaymentModel>()
            {
                Status = true,
                Message = "Payment fail.",
            };

        }
        public async Task<bool> CheckInformation(Guid userId, List<Guid> productSizeIds, List<int> quantities)
        {
            var cart = await _unitOfWork.CartRepository.GetByAccount(userId);

            if (cart == null || cart.CartItems.Count == 0 || productSizeIds.Count != quantities.Count)
            {
                return false;
              

            }
            for (int i = 0; i < productSizeIds.Count; i++)
            {
                var productSizeId = productSizeIds[i];
                var quantity = quantities[i];
               
                var cartItem = cart.CartItems.FirstOrDefault(_ => _.ProductSizeID == productSizeId);
                if (cartItem == null || cartItem.Quantity != quantity)
                {
                    return false;

                }
            

            }

            return true;

        }
        public async Task<bool> CheckStockQuantity(Guid userId, List<Guid> productSizeIds, List<int> quantities)
        {
            var cart = await _unitOfWork.CartRepository.GetByAccount(userId);
            for (int i = 0; i < productSizeIds.Count; i++)
            {
                var productSizeId = productSizeIds[i];
                var quantity = quantities[i];
                var product = cart.CartItems.Select(_ => _.ProductSize).FirstOrDefault(_ => _.Id == productSizeId);
                var stockProduct = product.StockQuantity;
                if (stockProduct < quantity)
                {
                    return false;
                }
            }
            return true;
        }
        private Order CreateOrder(Cart cart, Account user, OrderModel orderModel)
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                AccountID = user.Id,
                BillingAddress = orderModel.BillingAddress,
                PaymentMethod = PaymentMethod.VNPay,
                OrderCartItems = orderModel.OrderCartItemModels.Select(_ => new OrderCartItem
                {
                    ProductSizeID = _.ProductSizeID,
                    Quantity = _.Quantity,
                    Price = _.PricePerItem
                }).ToList(),
                Payment = new Payment
                {
                    Id = Guid.NewGuid(),
                    Amount = orderModel.OrderCartItemModels.Sum(_ => _.Quantity * _.PricePerItem),
                }
            };
        }

        public async Task<ResponseModel> UpdateOrderStatus(UpdateOrderStatusModel updateOrderStatusModel)
        {
            var order = await _unitOfWork.OrderRepository.GetAsync(updateOrderStatusModel.OrderId, "Payment");
            if (order == null)
            {
                return new ResponseModel { Message = "Not found order!", Status = false };
            }
            else
            {
                if (updateOrderStatusModel.VnPayResponseCode == "00")
                {
                    order.Status = OrderStatus.Success;
                    _unitOfWork.OrderRepository.Update(order);
                    var payment = order.Payment;
                    payment.PaymentStatus = PaymentStatus.Completed;
                    _unitOfWork.PaymentRepository.Update(payment);
                    var rs = await _unitOfWork.SaveChangeAsync();
                    if (rs > 0)
                    {
                        return new ResponseModel { Message = "Successfully", Status = true };
                    }
                    return new ResponseModel { Message = "Fail", Status = false };

                }
            }
            return new ResponseModel { Message = "Error!", Status = false };

        }
    }
}
