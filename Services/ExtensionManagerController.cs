using ExtensionManager.Contracts.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ExtensionManager.BizLogic.Controllers
{
    [RoutePrefix("api/ExtensionManager")]
    public class ExtensionManagerController : ApiController
    {
        private readonly IExtensionManagerService extensionManagerService;

        public ExtensionManagerController(IExtensionManagerService extensionManagerService)
        {
            this.extensionManagerService = extensionManagerService;
        }
    }
}
