using Unity.Services.Core.Configuration.Editor;
using UnityEditor.Build;
using UnityEditor;

namespace Unity.Services.Vivox.Editor
{
    /// <summary>
    /// Provider for settings related to Vivox that we want available at runtime.
    /// </summary>
    class VivoxConfigurationProvider : IConfigurationProvider
    {
        int IOrderedCallback.callbackOrder { get; }

        /// <summary>
        /// Adds your configuration values to the given <paramref name="builder"/>.
        /// This method is called on fresh instances created by reflection to be sure you can
        /// reach the settings you want available at runtime directly from a new instance.
        /// </summary>
        /// <param name="builder">
        /// The builder used to create the runtime configuration data.
        /// Use it to set configuration values.
        /// </param>
        public void OnBuildingConfiguration(ConfigurationBuilder builder)
        {
            if (VivoxSettings.Instance.IsEnvironmentCustom || CloudProjectSettings.projectId == VivoxSettings.Instance.PulledCredentialProjectId)
            {
                builder.SetString(VivoxServiceInternal.ServerKey, VivoxSettings.Instance.Server);
                builder.SetString(VivoxServiceInternal.DomainKey, VivoxSettings.Instance.Domain);
                builder.SetString(VivoxServiceInternal.IssuerKey, VivoxSettings.Instance.TokenIssuer);
                builder.SetString(VivoxServiceInternal.TokenKey, VivoxSettings.Instance.TokenKey);
                builder.SetBool(VivoxServiceInternal.EnvironmentCustomKey, VivoxSettings.Instance.IsEnvironmentCustom);
            }
        }
    }
}