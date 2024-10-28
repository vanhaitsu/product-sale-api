using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Models.OrderModels;
using Services.Interfaces;
using Services.Models.CartModels;
using Services.Models.OrderModels;
using Services.Services;

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
