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
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var pay = new VNPayLibrary();

            var baseReturnUrl = _configuration["Vnpay:ReturnUrl"];

            var urlCallBack = $"{baseReturnUrl}/{requestDto.BookingID}";

            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)requestDto.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(httpContext));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo",
                HttpUtility.UrlEncode($"Khach hang: {requestDto.CustomerName} thanh toan hoa don {requestDto.BookingID}"));
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", requestDto.BookingID);
            var baseUrl = _configuration["Vnpay:BaseUrl"];
            var hashSecret = _configuration["Vnpay:HashSecret"];
            var rawPaymentUrl = pay.CreateRequestUrl(baseUrl, hashSecret);
            var paymentUri = new Uri(rawPaymentUrl);
            var queryParams = HttpUtility.ParseQueryString(paymentUri.Query);
            var modifiedParams = new List<string>();

            foreach (string key in queryParams.Keys)
            {
                if (key != null)
                {
                    var camelCaseKey = ToCamelCase(key.Replace("vnp_", "vnp"));
                    modifiedParams.Add($"{camelCaseKey}={HttpUtility.UrlEncode(queryParams[key])}");
                }
            }
            var finalPaymentUrl = $"{baseUrl}?{string.Join("&", modifiedParams)}";
            return finalPaymentUrl;
        }

        private string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var words = str.Split('_');
            var result = words[0].ToLower();

            for (int i = 1; i < words.Length; i++)
            {
                if (!string.IsNullOrEmpty(words[i]))
                    result += char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }

            return result;
        }
    }
}
