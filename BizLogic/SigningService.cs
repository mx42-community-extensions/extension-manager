using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ExtensionManager.Contracts.DataContracts;
using ExtensionManager.Contracts.DataContracts.Licensing;
using ExtensionManager.Contracts.ServiceContracts;
using Matrix42.Common;
using Newtonsoft.Json;
using update4u.SPS.DataLayer;

namespace ExtensionManager.BizLogic
{
    public class SigningService : ISigningService
    {
        private static readonly Guid ConfigurationClassId = SPSDataEngineSchemaReader.ClassGetIDFromName("CAEMConfiguration");

        private X509Certificate2 GetCertificate()
        {
            var configuration = FragmentRequestBase.SimpleLoad(ConfigurationClassId, "CertificateThumbprint", null);

            if (configuration.Rows.Count == 0)
                throw new InvalidOperationException("CertificateThumbprint not set.");

            var thumbprint = ConvertHelper.ParseString(configuration.Rows[0]["CertificateThumbprint"]);

            if (string.IsNullOrWhiteSpace(thumbprint))
                throw new InvalidOperationException("CertificateThumbprint not set or invalid.");

            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            var certificate = store.Certificates
                .Find(X509FindType.FindByThumbprint, thumbprint, false)
                .OfType<X509Certificate2>()
                .FirstOrDefault();

            store.Close();

            if (certificate == null)
                throw new InvalidOperationException("Certificate not found");

            return certificate;
        }

        public string GetPublicCertificate()
        {
            using (var certificate = GetCertificate())
            {
                var binaryPublicCertificate = certificate.Export(X509ContentType.SerializedCert);
                return Convert.ToBase64String(binaryPublicCertificate);
            }
        }


        public string CreateToken<T>(T payload) where T : class
        {
            using (var certificate = GetCertificate())
            {
                return JwsHelper.CreateJsonWebSignature(payload, new X509SecurityKey(certificate));
            }
        }

        public bool TryParseToken<T>(string token, out T payload) where T : class
        {
            using (var certificate = GetCertificate())
            {
                return JwsHelper.TryParseJsonWebSignature(token, new X509SecurityKey(certificate), out payload);
            }
        }
    }
}
