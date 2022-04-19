using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Internal;
using UnityEngine;

namespace Unity.Services.Vivox
{
#if !UNITY_STANDALONE_LINUX
    /// <summary>
    /// This is the package initializer.
    /// By implementing <see cref="IInitializablePackage"/>, it will be initialized in the right order, based on dependencies
    /// </summary>
    class VivoxPackageInitializer : IInitializablePackage
    {

        private IAccessToken AccessToken;

        /// <summary>
        /// Register to Core through a static method that is called before scene load.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            // Pass an instance of this class to Core
            CoreRegistry.Instance.RegisterPackage(new VivoxPackageInitializer())
                // And specify what components it requires, or provides.
                .DependsOn<IProjectConfiguration>()
                .OptionallyDependsOn<IAccessToken>()
                .OptionallyDependsOn<IPlayerId>();
        }

        /// <summary>
        /// This is the Initialize callback that will be triggered by the Core package.
        /// This method will be invoked when the game developer calls UnityServices.InitializeAsync().
        /// </summary>
        /// <param name="registry">
        /// The registry containing components from different packages.
        /// </param>
        /// <returns>
        /// Return a Task representing your initialization.
        /// </returns>
        public Task Initialize(CoreRegistry registry)
        {
            try
            {
                var vivoxService = new VivoxServiceInternal();
                VivoxService.Instance = vivoxService;

                AccessToken = registry.GetServiceComponent<IAccessToken>();
                IPlayerId playerId = registry.GetServiceComponent<IPlayerId>();
                if (playerId != null)
                {
                    // Listen for UAS user's player id to change via signing in and signing out
                    playerId.PlayerIdChanged += HandlePlayerIdChange;
                }
                var config = registry.GetServiceComponent<IProjectConfiguration>();
                var server = config.GetString(VivoxServiceInternal.ServerKey);
                var domain = config.GetString(VivoxServiceInternal.DomainKey);
                var issuer = config.GetString(VivoxServiceInternal.IssuerKey);
                var token = config.GetString(VivoxServiceInternal.TokenKey);
                var isEnvironmentCustom = config.GetBool(VivoxServiceInternal.EnvironmentCustomKey);
                VivoxServiceInternal.SetCredentials(server, domain, issuer, token, isEnvironmentCustom);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[Vivox]: Unable to initialize Vivox. If you are not using Unity Game Services and Authentication Service, set your credentials manually by using the Services > Vivox > Configure menu.${ex}");
            }

            return Task.CompletedTask;
        }

        private void HandlePlayerIdChange(string newId)
        {
            // Until Services Core supports it we need to fetch the Environment Id from the UAS Token
            // Not rethrowing this because we are ok if the envId is not set
            try
            {
                // This is common when a UAS user is logged out
                if (string.IsNullOrWhiteSpace(newId))
                {
                    return;
                }

                VivoxServiceInternal.SetPlayerId(newId);
                
                if (AccessToken != null)
                {
                    VivoxServiceInternal.SetAccessTokenComponent(AccessToken);
                    var parts = AccessToken.AccessToken.Split('.');
                    var payload = parts[1];
                    var payloadJson = Encoding.UTF8.GetString(JwtDecoder.Base64UrlDecode(payload));
                    var payloadData = UnityEngine.JsonUtility.FromJson<AccessToken>(payloadJson);
                    var envId = payloadData.aud.First(s => s.StartsWith("envId:")).Substring(6);
                    VivoxServiceInternal.SetEnvironmentId(envId);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                return;
            }
        }
    }
#endif
}