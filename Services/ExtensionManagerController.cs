using ExtensionManager.Contracts.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using ExtensionManager.Contracts.DataContracts.Licensing;
using ExtensionManager.Contracts.DataContracts.Registration;

namespace ExtensionManager.BizLogic.Controllers
{
    [RoutePrefix("api/ExtensionManager")]
    public class ExtensionManagerController : ApiController
    {
        private readonly IExtensionManagerService extensionManagerService;
        private readonly ISigningService signingService;

        public ExtensionManagerController(IExtensionManagerService extensionManagerService, ISigningService signingService)
        {
            this.extensionManagerService = extensionManagerService;
            this.signingService = signingService;
        }

        private bool TryGetCustomerSystemId(out Guid customerSystemId)
        {
            customerSystemId = Guid.Empty;

            if (Request.Headers.Authorization == null
                || Request.Headers.Authorization.Scheme != "Bearer")
                return false;

            var token = Request.Headers.Authorization.Parameter;

            if (!signingService.TryParseToken(token, out CustomerSystemRegistration registration))
                return false;

            customerSystemId = registration.SystemId;

            return true;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetPublicCertificate")]
        public string GetPublicCertificate()
        {
            return signingService.GetPublicCertificate();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetRegistration")]
        public IHttpActionResult GetRegistration()
        {
            var systemId = Guid.NewGuid();

            var token = signingService.CreateToken(new CustomerSystemRegistration { SystemId = systemId });

            return Ok(token);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("TestAuth")]
        public IHttpActionResult TestAuth()
        {
            if (!TryGetCustomerSystemId(out Guid customerSystemId))
                return BadRequest("No or invalid customer registration token.");

            return Ok(customerSystemId);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("TestLicense")]
        public IHttpActionResult TestLicense()
        {
            if (!TryGetCustomerSystemId(out _))
                return BadRequest("No or invalid customer registration token.");

            var license = new License
            {
                ExtensionId = Guid.NewGuid(),
                IssuerId = Guid.NewGuid(),
                Metric = LicenseMetric.User,
                Count = 999,
                ValidUntil = DateTime.UtcNow.AddYears(1),
                Components = new ComponentLicense[]
                {
                    new ComponentLicense { Name = "FEATURE1", ComponentId = Guid.NewGuid(), ValidUntil = DateTime.UtcNow.AddMonths(6) },
                    new ComponentLicense { Name = "FEATURE_TWO", ComponentId = Guid.NewGuid(), ValidFrom = DateTime.UtcNow, ValidUntil = null }
                }
            };

            var licenseToken = signingService.CreateToken(license);

            return Ok(licenseToken);
        }

        //[HttpPost]
        //[Route("Register")]
        //public void Register([FromBody] RegistrationData registrationData)
        //{
        //    throw new NotImplementedException();
        //}

        //[HttpGet]
        //[Route("Products")]
        //public IEnumerable<ProductData> GetProducts()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
