using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repositories.Models.VNPayModels;
using Services.Interfaces;

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

            pay.AddRequestData("vnpVersion", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnpCommand", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnpTmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnpAmount", ((int)requestDto.Amount * 100).ToString());
            pay.AddRequestData("vnpCreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnpCurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnpIpAddr", pay.GetIpAddress(httpContext));
            pay.AddRequestData("vnpLocale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnpOrderInfo",
                $"Khach hang: {requestDto.CustomerName} thanh toan hoa don {requestDto.BookingID}");
            pay.AddRequestData("vnpOrderType", "other");
            pay.AddRequestData("vnpReturnUrl", urlCallBack);
            pay.AddRequestData("vnpTxnRef", requestDto.BookingID);
            paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);


            return paymentUrl;
        }
    }
}
