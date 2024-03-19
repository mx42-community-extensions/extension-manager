using ExtensionManager.BizLogic;
using ExtensionManager.Contracts.ServiceContracts;
using Matrix42.Hosting.Contracts;

namespace ExtensionManager.Properties
{
    public class DependencyRegistrator : DependencyRegistratorBase, IDependencyRegistrator
    {
        public void Register(IDependencyContainer container, IDependencyResolver resolver)
        {
            container.RegisterSingletonType<ISigningService, SigningService>();
            container.RegisterSingletonType<ILicensingService, LicensingService>();
            container.RegisterSingletonType<IExtensionManagerService, ExtensionManagerService>();
        }
    }
}
