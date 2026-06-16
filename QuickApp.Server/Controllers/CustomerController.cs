// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickApp.Core.Services;
using QuickApp.Core.Services.Shop;
using QuickApp.Server.Services.Email;
using QuickApp.Server.ViewModels.Shop;

namespace QuickApp.Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CustomerController : BaseApiController
    {
        private readonly IEmailSender _emailSender;
        private readonly ICustomerService _customerService;

        public CustomerController(IMapper mapper, ILogger<CustomerController> logger, IEmailSender emailSender,
            ICustomerService customerService)
            : base(logger, mapper)
        {
            _emailSender = emailSender;
            _customerService = customerService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var allCustomers = _customerService.GetAllCustomersData();
            return Ok(_mapper.Map<IEnumerable<CustomerVM>>(allCustomers));
        }

#if DEBUG
        [HttpGet("throw")]
        public IEnumerable<CustomerVM> Throw()
        {
            throw new CustomerException($"This is a test exception: {DateTime.Now}");
        }

        [HttpGet("email")]
        public async Task<string> Email()
        {
            var recipientName = "QickApp Tester"; //         <===== Put the recipient's name here
            var recipientEmail = "test@ebenmonney.com"; //   <===== Put the recipient's email here

            var message = EmailTemplates.GetTestEmail(recipientName, DateTime.UtcNow);

            (var success, var errorMsg) = await _emailSender.SendEmailAsync(recipientName, recipientEmail,
                "Test Email from QuickApp", message);

            if (success)
                return "Success";

            return $"Error: {errorMsg}";
        }
#endif

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"value: {id}";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
