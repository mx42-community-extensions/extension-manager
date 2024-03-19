using ExtensionManager.Contracts.DataContracts;

namespace ExtensionManager.Contracts.ServiceContracts
{
    public interface ISigningService
    {
        /// <summary>
        /// Gets the public certificate of the signing certificate.
        /// </summary>
        /// <returns>Base64 encoded public certificate.</returns>
        string GetPublicCertificate();

        /// <summary>
        /// Creates a token from the given payload.
        /// </summary>
        /// <typeparam name="T">Type of the payload.</typeparam>
        /// <param name="payload">The payload.</param>
        /// <returns>JWS token.</returns>
        string CreateToken<T>(T payload) where T : class;

        /// <summary>
        /// Tries to parse the token and returns the payload if successful.
        /// </summary>
        /// <typeparam name="T">Type of the payload.</typeparam>
        /// <param name="token">The token.</param>
        /// <param name="payload">The payload.</param>
        /// <returns><see langword="true"/> if the token could be parsed, otherwise <see langword="false"/>.</returns>
        bool TryParseToken<T>(string token, out T payload) where T : class;
    }
}
