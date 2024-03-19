using ExtensionManager.Contracts.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionManager.BizLogic
{
    public class ExtensionManagerService : IExtensionManagerService
    {
        private readonly ISigningService _signingService;

        public ExtensionManagerService(ISigningService signingService)
        {
            _signingService = signingService;
        }
    }
}
