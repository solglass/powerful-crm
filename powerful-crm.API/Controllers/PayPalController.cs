using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using powerful_crm.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayPalController : ControllerBase
    {
        private IPayPalRequestService _payPalService;      

        public PayPalController(IPayPalRequestService payPalService)
        {
            _payPalService = payPalService;          
        }
        [HttpGet]
        public ActionResult GetToken()
        {
            return Ok(_payPalService.GetToken());
        }
    }
}
