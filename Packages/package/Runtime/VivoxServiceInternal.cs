using System;
using Unity.Services.Authentication.Internal;
using VivoxUnity;

namespace Unity.Services.Vivox
{
    class VivoxServiceInternal : IVivoxService
    {
        /// <summary>
        /// Keys used to fetch Vivox credentials.
        /// </summary>
        internal const string ServerKey = "com.unity.services.vivox.server";
        internal const string DomainKey = "com.unity.services.vivox.domain";
        internal const string IssuerKey = "com.unity.services.vivox.issuer";
        internal const string TokenKey = "com.unity.services.vivox.token";
        internal const string EnvironmentCustomKey = "com.unity.services.vivox.is-environment-custom";

        /// <summary>
        /// Returns the current player's access token when they are signed in, otherwise null.
        /// </summary>
        internal static IAccessToken accessTokenComponent { get; private set; }

        /// <summary>
        /// Returns the current player's access token when they are signed in, otherwise null.
        /// </summary>
        internal static string AccessToken
        {
            get
            {
                return accessTokenComponent.AccessToken;
            }
        }

        /// <summary>
        /// Returns the current environment ID when they are signed in, otherwise null.
        /// </summary>
        internal static string EnvironmentId { get; private set; }

        /// <summary>
        /// Returns the current player's ID when they are signed in, otherwise null.
        /// </summary>
        internal static string PlayerId { get; private set; }

        internal static string _server;
        internal static Uri ServerUri => new Uri(_server);
        internal static string _domain { get; private set; }
        internal static string _issuer { get; private set; }
        internal static string _key { get; private set; }
        internal static bool _isEnvironmentCustom { get; private set; }

        public bool IsAuthenticated => (PlayerId != null && EnvironmentId != null);

        public Client Client { get; private set; }

        public string Server => _server;
        public string Domain => _domain;
        public string Issuer => _issuer;
        public string Key => _key;
        public bool IsEnvironmentCustom => _isEnvironmentCustom;

        public void Initialize()
        {
            string uriString = Server;

            // If credentials are provided by Udash - append EnvironmentId. Issuer will already be appended to Server Uri as provided by Udash.
            // If custom credentials are in use, do not modify the Server Uri.
            if (!IsEnvironmentCustom)
            {
                string environmentFragment = $"/{EnvironmentId}";
                uriString += environmentFragment;
            }
            
            Client = new Client(new Uri(uriString));
            Client.Initialize();
            if (IsAuthenticated)
            {
                Client.tokenGen = new VivoxJWTTokenGen();
            }
            else
            {
                Client.tokenGen.IssuerKey = Key;
            }
        }

        /// <summary>
        /// Sets the current player's Vivox credentials.
        /// </summary>
        internal static void SetCredentials(string server, string domain, string issuer, string token, bool customEnvironment)
        {
            if (string.IsNullOrEmpty(server))
            {
                throw new ArgumentException($"'{nameof(server)}' is null or empty", nameof(server));
            }
            if (string.IsNullOrEmpty(domain))
            {
                throw new ArgumentException($"'{nameof(domain)}' is null or empty", nameof(domain));
            }
            if (string.IsNullOrEmpty(issuer))
            {
                throw new ArgumentException($"'{nameof(issuer)}' is null or empty", nameof(issuer));
            }

            _server = server;
            _domain = domain;
            _issuer = issuer;
            _key = token;
            _isEnvironmentCustom = customEnvironment;
        }

        /// <summary>
        /// Sets the current player's access token.
        /// </summary>
        internal static void SetAccessTokenComponent(IAccessToken accessToken)
        {
            accessTokenComponent = accessToken;
        }

        /// <summary>
        /// Sets the current player's ID.
        /// </summary>
        internal static void SetPlayerId(string playerId)
        {
            PlayerId = playerId;
        }

        /// <summary>
        /// Sets the current environment ID.
        /// </summary>
        internal static void SetEnvironmentId(string envrionmentId)
        {
            EnvironmentId = envrionmentId;
        }
    }

    /// <summary>
    /// Class used to override the token we pass into any Vivox requests.
    /// </summary>
    internal class VivoxJWTTokenGen : VxTokenGen
    {
        public override string GetToken(string issuer = null, TimeSpan? expiration = null, string userUri = null, string action = null, string tokenKey = null, string conferenceUri = null, string fromUserUri = null)
        {
            return VivoxServiceInternal.AccessToken;
        }
    }
}