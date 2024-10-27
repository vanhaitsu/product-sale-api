using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.CartModels;
using Services.Models.NotificationModels;
using Services.Services;

namespace API.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IOneSignalPushNotificationService _oneSignalPushNotificationService;

        public NotificationController(IOneSignalPushNotificationService oneSignalPushNotificationService)
        {
            _oneSignalPushNotificationService = oneSignalPushNotificationService;
        }

        [HttpPost()]
        //[Authorize(Roles = "User, Expert")]
        public async Task<IActionResult> Push([FromBody] OneSignalNotificationModel oneSignalNotificationModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ValidationProblem(ModelState);
                }
                var result = await _oneSignalPushNotificationService.SendNotificationAsync(oneSignalNotificationModel);
                if (result)
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
