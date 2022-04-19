using Unity.Services.Core;

namespace Unity.Services.Vivox
{
    /// <summary>
    /// Utilities to simplify setting options related to this SDK through code.
    /// </summary>
    public static class InitializationOptionsExtensions
    {
        /// <summary>
        /// An extension to set the credentials for the Vivox SDK.
        /// </summary>
        /// <param name="tokenKey">This is optional because a developer could be leveraging Unity Authentication tokens or vending tokens server-side.</param>
        /// <returns>
        /// Return <paramref name="self"/>.
        /// Fluent interface pattern to make it easier to chain set options operations.
        /// </returns>
        public static InitializationOptions SetVivoxCredentials(this InitializationOptions self, string server, string domain, string issuer, string tokenKey = "")
        {
            self.SetOption(VivoxServiceInternal.ServerKey, server);
            self.SetOption(VivoxServiceInternal.DomainKey, domain);
            self.SetOption(VivoxServiceInternal.IssuerKey, issuer);
            self.SetOption(VivoxServiceInternal.TokenKey, tokenKey);
            return self;
        }
    }
}