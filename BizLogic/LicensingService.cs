using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExtensionManager.Contracts.ServiceContracts;

namespace ExtensionManager.BizLogic
{
    public class LicensingService : ILicensingService
    {
        private readonly ISigningService _signingService;

        public LicensingService(ISigningService signingService)
        {
            _signingService = signingService;
        }

        public string[] GetLicenses(Guid customerSystemId)
        {
            throw new NotImplementedException();
        }
    }
}
