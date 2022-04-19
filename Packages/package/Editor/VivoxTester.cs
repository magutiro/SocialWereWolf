using UnityEngine;
using VivoxUnity;
using System.Collections;

namespace Unity.Services.Vivox.Editor
{
    /// <summary>
    /// This class represents a very lite version of a Vivox wrapper used to quickly test if a login will succeed given the supplied credentials.
    /// The loginsession and client are immediately torn down whether or not the login succeeds or fails.
    /// </summary>
    internal class VivoxTester
    {
        public System.Action<LoginState> OnUserLoggedInEvent;

        private readonly string _server;
        private readonly string _domain;
        private readonly string _tokenIssuer;
        private readonly string _tokenKey;
        private System.TimeSpan _tokenExpiration = System.TimeSpan.FromSeconds(90);

        private Client _client = new Client();
        private AccountId _accountId;

        public ILoginSession LoginSession;

        private System.Uri ServerUri => new System.Uri(_server);
        public LoginState LoginState { get; private set; }

        public VivoxTester(string server, string domain, string tokenIssuer, string tokenKey)
        {
            _server = server;
            _domain = domain;
            _tokenIssuer = tokenIssuer;
            _tokenKey = tokenKey;
        }

        public void Login()
        {
            Client.tokenGen = new VxTokenGen();
            _client.Initialize();
            string uniqueId = System.Guid.NewGuid().ToString();
            //for proto purposes only, need to get a real token from server eventually

            _accountId = new AccountId(_tokenIssuer, uniqueId, _domain);
            LoginSession = _client.GetLoginSession(_accountId);
            LoginSession.PropertyChanged += OnLoginSessionPropertyChanged;
            LoginSession.BeginLogin(ServerUri, LoginSession.GetLoginToken(_tokenKey, _tokenExpiration), SubscriptionMode.Accept, null, null, null, ar =>
            {
                try
                {
                    LoginSession.EndLogin(ar);
                }
                catch (System.Exception e)
                {
                    // Handle error 
                    Debug.LogError(e.ToString());
                    // Unbind if we failed to login.
                    return;
                }
            });
        }

        public void Logout()
        {
            if (LoginSession != null && LoginState != LoginState.LoggedOut && LoginState != LoginState.LoggingOut)
            {
                LoginSession.Logout();
            }
        }

        private void OnLoginSessionPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != "State")
            {
                return;
            }
            var loginSession = (ILoginSession)sender;
            LoginState = loginSession.State;
            switch (LoginState)
            {
                case LoginState.LoggingIn:
                    {
                        break;
                    }
                case LoginState.LoggedIn:
                    {
                        OnUserLoggedInEvent?.Invoke(LoginState);
                        Logout();
                        Client.Cleanup();
                        break;
                    }
                case LoginState.LoggedOut:
                    {
                        OnUserLoggedInEvent?.Invoke(LoginState);
                        Client.Cleanup();
                        break;
                    }
                default:
                    break;
            }
        }

        public IEnumerator VivoxUnityRun()
        {
            try
            {
                Client.RunOnce();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error: " + e.Message);
            }

            yield return null;
        }
    }
}