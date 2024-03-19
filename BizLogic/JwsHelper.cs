using System;
using System.Diagnostics;
using System.IdentityModel.Tokens;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ExtensionManager.BizLogic
{
    public static class JwsHelper
    {
        private const string SignatureAlgorithm = SecurityAlgorithms.RsaSha256Signature;
        private const string SignatureAlgorithmHeader = "RS256";

        private static JwtHeader GetJwtHeader(object payload)
        {
            return new JwtHeader
            {
                ["alg"] = SignatureAlgorithmHeader,
                ["typ"] = payload == null ? "JWT" : payload.GetType().Name.ToUpperInvariant()
            };
        }

        public static string CreateJsonWebSignature(object payload, SecurityKey securityKey)
        {
            if (securityKey == null)
                throw new ArgumentNullException(nameof(securityKey));

            var header = GetJwtHeader(payload);
            var headerBase64 = header.Base64UrlEncode();

            var payloadJson = JsonConvert.SerializeObject(payload);
            var payloadBytes = Encoding.UTF8.GetBytes(payloadJson);
            var payloadBase64 = Base64UrlEncoder.Encode(payloadBytes);

            var signatureData = $"{headerBase64}.{payloadBase64}";
            var signatureDataBytes = Encoding.UTF8.GetBytes(signatureData);

            var signatureFactory = new SignatureProviderFactory();
            var signatureProvider = signatureFactory.CreateForSigning(securityKey, SignatureAlgorithm);

            var signature = signatureProvider.Sign(signatureDataBytes);
            var signatureBase64 = Base64UrlEncoder.Encode(signature);

            return $"{signatureData}.{signatureBase64}";
        }

        public static bool TryParseJsonWebSignature<TPayload>(string jwsToken, SecurityKey securityKey, out TPayload payload) where TPayload : class
        {
            payload = null;

            try
            {
                if (securityKey == null)
                    throw new ArgumentNullException(nameof(securityKey));


                if (!Regex.IsMatch(jwsToken, @"^[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+\.[A-Za-z0-9-_]*$"))
                    return false;

                var splitToken = jwsToken.Split('.');

                var header = JwtHeader.Base64UrlDeserialize(splitToken[0]);

                if (header.Alg != SignatureAlgorithmHeader)
                    return false;

                var signatureFactory = new SignatureProviderFactory();
                var signatureProvider = signatureFactory.CreateForVerifying(securityKey, SignatureAlgorithm);

                var signatureData = $"{splitToken[0]}.{splitToken[1]}";
                var signatureDataBytes = Encoding.UTF8.GetBytes(signatureData);
                var signature = Base64UrlEncoder.DecodeBytes(splitToken[2]);

                if (!signatureProvider.Verify(signatureDataBytes, signature))
                    return false;

                var payloadJson = Base64UrlEncoder.Decode(splitToken[1]);
                payload = JsonConvert.DeserializeObject<TPayload>(payloadJson);

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error parsing JWS: {ex.Message}");
                Trace.WriteLine(ex.StackTrace);
            }

            return false;
        }
    }
}
