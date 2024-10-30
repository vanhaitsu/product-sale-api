using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models.OrderModels;
using Services.Interfaces;
using Services.Models.CartModels;
using Services.Models.OrderModels;
using Services.Services;
using System.Web;

namespace API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpPost("vnpay-payment")]
        public async Task<IActionResult> VNPayMethod(OrderModel orderModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }
                var result = await _paymentService.VNPayMethod(orderModel, HttpContext);
                if (result.Status)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("redirect")]
        public IActionResult RedirectWithUpdatedQuery(
            [FromQuery] string vnp_Amount,
            [FromQuery] string vnp_BankCode,
            [FromQuery] string vnp_BankTranNo,
            [FromQuery] string vnp_CardType,
            [FromQuery] string vnp_OrderInfo,
            [FromQuery] string vnp_PayDate,
            [FromQuery] string vnp_ResponseCode,
            [FromQuery] string vnp_TmnCode,
            [FromQuery] string vnp_TransactionNo,
            [FromQuery] string vnp_TransactionStatus,
            [FromQuery] string vnp_TxnRef,
            [FromQuery] string vnp_SecureHash)
        {
            var redirectUrl = $"productsale://productsale.shop/successPage/{vnp_TxnRef}";
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["vnpAmount"] = vnp_Amount;
            queryParams["vnpBankCode"] = vnp_BankCode;
            queryParams["vnpBankTranNo"] = vnp_BankTranNo;
            queryParams["vnpCardType"] = vnp_CardType;
            queryParams["vnpOrderInfo"] = vnp_OrderInfo;
            queryParams["vnpPayDate"] = vnp_PayDate;
            queryParams["vnpResponseCode"] = vnp_ResponseCode;
            queryParams["vnpTmnCode"] = vnp_TmnCode;
            queryParams["vnpTransactionNo"] = vnp_TransactionNo;
            queryParams["vnpTransactionStatus"] = vnp_TransactionStatus;
            queryParams["vnpTxnRef"] = vnp_TxnRef;
            queryParams["vnpSecureHash"] = vnp_SecureHash;
            var finalRedirectUrl = $"{redirectUrl}?{queryParams}";
            return Redirect(finalRedirectUrl);
        }


        [HttpPut("adjustment-status")]
            public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusModel updateOrderStatusModel)
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return ValidationProblem(ModelState);
                    }
                    var result = await _paymentService.UpdateOrderStatus(updateOrderStatusModel);
                    if (result.Status)
                    {
                        return Ok(result);
                    }
                    return BadRequest(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
}
