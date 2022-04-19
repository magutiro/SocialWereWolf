using VivoxUnity;

namespace Unity.Services.Vivox
{
    public interface IVivoxService
    {
        /// <summary>
        /// We are authenticated if we have successfully retrieved both an Environment ID and a Player ID
        /// The implementation can handle things differently if we are not authenticated and cannot produce the EnvID and PlayerID.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Object that is used to start the Vivox service and also houses the current LoginSession for a user which is used to perform operations such as joining channels.
        /// </summary>
        Client Client { get; }

        /// <summary>
        /// Initializes the Client object which will enable a user to login and check/manage audio devices.
        /// </summary>
        void Initialize();

        /// <summary>
        /// The Server value placed into the Vivox tab of the Project Settings
        /// </summary>
        string Server { get; }
        /// <summary>
        /// The Domain value placed into the Vivox tab of the Project Settings
        /// </summary>
        string Domain { get; }
        /// <summary>
        /// The Issuer value placed into the Vivox tab of the Project Settings
        /// </summary>
        string Issuer { get; }
        /// <summary>
        /// Whether or not the credentials were automatically pulled from the Unity Dashboard, or input by hand from the Vivox Developer Portal
        /// </summary>
        bool IsEnvironmentCustom { get; }
    }
}