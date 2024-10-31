using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Models.VNPayModels;
using Services.Interfaces;
using System.Web;

namespace Services.Services
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly IConfiguration _configuration;
        public PaymentGatewayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> CreatePaymentUrlVnpay(PaymentInformationModel requestDto, HttpContext httpContext)
        {
            var paymentUrl = "";
            var vnPay = new PaymentInformationModel
            {
                AccountID = requestDto.AccountID,
                Amount = requestDto.Amount,
                CustomerName = requestDto.CustomerName,
                BookingID = requestDto.BookingID
            };
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var pay = new VNPayLibrary();
            var urlCallBack = $"{_configuration["Vnpay:ReturnUrl"]}/{requestDto.BookingID}";

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)requestDto.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(httpContext));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo",
                $"Khach hang: {requestDto.CustomerName} thanh toan hoa don {requestDto.BookingID}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", requestDto.BookingID);
            paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);


            return paymentUrl;
        }
    }
}
