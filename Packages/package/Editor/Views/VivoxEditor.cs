using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using VivoxUnity;
using System;

namespace Unity.Services.Vivox.Editor
{
    internal class VivoxEditor : EditorWindow
    {
        internal const string checkmarkPath = "Packages/com.unity.services.vivox/Editor/Icons/Checkmark.png";
        internal Color confirmationGreen = new Color(0.4117647F, 0.8901961F, 0.6235294F);

        internal enum VivoxConnectionStatus
        {
            Success = 0,
            Testing,
            Failed,
        }

        static readonly Dictionary<VivoxConnectionStatus, VivoxMessage> vivoxStatusContainer = new Dictionary<VivoxConnectionStatus, VivoxMessage>()
        {
            { VivoxConnectionStatus.Success, new VivoxMessage("Connection Successful", checkmarkPath) },
            { VivoxConnectionStatus.Testing, new VivoxMessage("Testing connection...", "") },
            { VivoxConnectionStatus.Failed, new VivoxMessage("Failed to connect", "") },
        };

        VisualElement root;
        VisualElement labelfromUXML;

        // Credential containers.
        TextField server;
        TextField domain;
        TextField tokenIssuer;
        TextField tokenKey;
        EnumField environmentEnum;

        // Hyperlinks.
        Button dashboardLinkButton;
        Button documentationButton;

        // Connection testing UI.
        Button checkConnectionButton;
        Image connectionStatusImage;
        Label connectionStatusLabel;

        VivoxTester vvx;

#if UNITY_2019_4_OR_NEWER && !UNITY_2020_2_OR_NEWER
        [MenuItem("Services/Vivox/Configure", false)]
#endif
        public static void ShowVivoxConfigWindow()
        {
            VivoxEditor wnd = GetWindow<VivoxEditor>();
            wnd.Close();
            wnd = GetWindow<VivoxEditor>();
            wnd.titleContent = new GUIContent(" Vivox") // The space is intentional so we can create a small gap between the icon and tab title.
            {
                image = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.services.vivox/Editor/Icons/Headset.png")
            };
            wnd.maxSize = new Vector2(590f, 445f);
            wnd.minSize = wnd.maxSize;
            wnd.ShowContents();
        }

#if UNITY_2020_2_OR_NEWER
        [MenuItem("Services/Vivox/Configure", priority = 111)]
#endif
        static void ShowProjectSettings()
        {
            SettingsService.OpenProjectSettings("Project/Services/Vivox");
        }

        public void ShowContents()
        {
            // Each editor window contains a root VisualElement object
            root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.unity.services.vivox/Editor/UXML/VivoxEditor.uxml");
            labelfromUXML = visualTree.CloneTree();

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.unity.services.vivox/Editor/StyleSheets/VivoxEditor.uss");
            labelfromUXML.styleSheets.Add(styleSheet);
            root.Add(labelfromUXML);

            SetupCredVars();
            SetupEnvironmentEnum();
            BindButtonCallbacks();
#if ENABLE_EDITOR_GAME_SERVICES
            CloudProjectSettingsEventManager.instance.projectStateChanged += OnProjectBindingChanged;
#endif
        }

        private void Update()
        {
            if (VxClient.Instance.Started && !EditorApplication.isPlayingOrWillChangePlaymode && vvx != null)
            {
                vvx.VivoxUnityRun().MoveNext();
            }
        }

        private void OnDestroy()
        {
#if ENABLE_EDITOR_GAME_SERVICES
            CloudProjectSettingsEventManager.instance.projectStateChanged -= OnProjectBindingChanged;
#endif
        }

        #region Setup

        private void SetupCredVars()
        {
            server = root.Q<TextField>("ServerVar");
            domain = root.Q<TextField>("DomainVar");
            tokenIssuer = root.Q<TextField>("IssuerVar");
            tokenKey = root.Q<TextField>("KeyVar");
            connectionStatusImage = root.Q<Image>("ConnectionImage");
            connectionStatusLabel = root.Q<Label>("ConnectionStatusLabel");

            server.SetValueWithoutNotify(VivoxSettings.Instance.Server);
            domain.SetValueWithoutNotify(VivoxSettings.Instance.Domain);
            tokenIssuer.SetValueWithoutNotify(VivoxSettings.Instance.TokenIssuer);
            tokenKey.SetValueWithoutNotify(VivoxSettings.Instance.TokenKey);
        }

        private void SetupEnvironmentEnum()
        {
            environmentEnum = root.Q<EnumField>("EnvironmentVar");
            if (environmentEnum != null)
            {
                environmentEnum.Init(VivoxSettings.Instance.IsEnvironmentCustom ? EnvironmentType.Custom : EnvironmentType.Automatic);

                environmentEnum.RegisterValueChangedCallback(evt =>
                {
                    if ((EnvironmentType)evt.newValue == EnvironmentType.Automatic)
                    {
                        tokenKey.SetEnabled(false);
                        tokenIssuer.SetEnabled(false);
                        domain.SetEnabled(false);
                        server.SetEnabled(false);
                        if (!CheckBind())
                            return;
                        FetchCredentials();
                        VivoxSettings.Instance.IsEnvironmentCustom = false;
                    }
                    else
                    {
                        tokenKey.SetEnabled(true);
                        tokenIssuer.SetEnabled(true);
                        domain.SetEnabled(true);
                        server.SetEnabled(true);
                        VivoxSettings.Instance.IsEnvironmentCustom = true;
                    }
                    VivoxSettings.Instance.Save();
                });

                if (!CheckBind())
                    return;

                if ((EnvironmentType)environmentEnum.value == EnvironmentType.Automatic)
                {
                    tokenKey.SetEnabled(false);
                    tokenIssuer.SetEnabled(false);
                    domain.SetEnabled(false);
                    server.SetEnabled(false);
                }
                else
                {
                    return;
                }

                FetchCredentials();
            }
        }

        private void BindButtonCallbacks()
        {
            // Hyperlink buttons.
            dashboardLinkButton = root.Q<Button>("DashboardLinkButton");
            if (dashboardLinkButton != null)
            {
                dashboardLinkButton.clicked += OnDashboardButtonPressed;
            }
            documentationButton = root.Q<Button>("DocumentationLinkButton");
            if (documentationButton != null)
            {
                documentationButton.clicked += OnDeveloperPortalButtonPressed;
            }

            // Connection test button.
            checkConnectionButton = root.Q<Button>("ConnectionButton");
            if (checkConnectionButton != null)
            {
                checkConnectionButton.clicked += OnTestConnectionButtonPressed;
            }
        }

        #endregion

        #region Utility

        private void FetchCredentials()
        {
            VivoxApiClient.Instance.GetGatewayToken(OnGatewayTokenFetched, Debug.LogError);

            void OnGatewayTokenFetched(TokenExchangeResponse gateResp)
            {
                VivoxApiClient.Instance.GetCredentials(gateResp.Token, OnCredentialsFetched, HandleException);
                void OnCredentialsFetched(GetVivoxCredentialsResponse credResp)
                {
                    VivoxSettings.Instance.TokenKey = credResp.Credentials.Key;
                    VivoxSettings.Instance.TokenIssuer = credResp.Credentials.Issuer;
                    VivoxSettings.Instance.Server = credResp.Credentials.Environment.ServerUri;
                    VivoxSettings.Instance.Domain = credResp.Credentials.Environment.Domain;
                    VivoxSettings.Instance.PulledCredentialProjectId = CloudProjectSettings.projectId;
                    VivoxSettings.Instance.Save();


                    server.value = VivoxSettings.Instance.Server;
                    domain.value = VivoxSettings.Instance.Domain;
                    tokenKey.value = VivoxSettings.Instance.TokenKey;
                    tokenIssuer.value = VivoxSettings.Instance.TokenIssuer;
                }
                void HandleException(Exception exception)
                {
                    server.value = "";
                    domain.value = "";
                    tokenKey.value = "";
                    tokenIssuer.value = "";
                    environmentEnum.value = EnvironmentType.Custom;
                    if (exception.Message == "Object reference not set to an instance of an object")
                    {
                        Debug.LogError("[Vivox]: Failed to pull Credentials from the Unity Dashboard. Ensure that the associated project has Vivox enabled and has generated Vivox Credentials on the Unity Dashboard.");
                        return;
                    }
                    Debug.LogError(exception);
                }
            }
        }

        private void SetInteractables(bool isActive)
        {
            checkConnectionButton.SetEnabled(isActive);
            if (VivoxSettings.Instance.IsEnvironmentCustom)
            {
                server.SetEnabled(isActive);
                domain.SetEnabled(isActive);
                tokenIssuer.SetEnabled(isActive);
                tokenKey.SetEnabled(isActive);
            }
        }
        private void SetConnectionStatusUI(VivoxConnectionStatus status, string additionalStatusInfo = "")
        {
            var currentStatus = vivoxStatusContainer[status];
            connectionStatusLabel.text = string.IsNullOrEmpty(additionalStatusInfo) ? currentStatus.StatusMessage : $"{currentStatus.StatusMessage}: {additionalStatusInfo}";
            switch (status)
            {
                case VivoxConnectionStatus.Success:
                    connectionStatusLabel.style.color = confirmationGreen;
                    connectionStatusImage.style.backgroundImage = AssetDatabase.LoadAssetAtPath<Texture2D>(currentStatus.IconPath);
                    break;
                case VivoxConnectionStatus.Testing:
                    connectionStatusLabel.style.color = Color.yellow;
                    connectionStatusImage.style.backgroundImage = null;
                    break;
                case VivoxConnectionStatus.Failed:
                    connectionStatusLabel.style.color = Color.red;
                    connectionStatusImage.style.backgroundImage = null;
                    break;
                default:
                    break;
            }
            SetInteractables(true);
        }
        private void SaveCredentials()
        {
            VivoxSettings.Instance.Domain = domain.text;
            VivoxSettings.Instance.Server = server.text;
            VivoxSettings.Instance.TokenIssuer = tokenIssuer.text;
            VivoxSettings.Instance.TokenKey = tokenKey.text;
            VivoxSettings.Instance.Save();
        }

        private bool CheckBind()
        {
            if (CloudProjectSettings.projectId == "")
            {
                environmentEnum.value = EnvironmentType.Custom;
                environmentEnum.SetEnabled(false);
                tokenKey.SetEnabled(true);
                tokenIssuer.SetEnabled(true);
                domain.SetEnabled(true);
                server.SetEnabled(true);
                VivoxSettings.Instance.IsEnvironmentCustom = true;
                VivoxSettings.Instance.Save();
                return false;
            }
            return true;
        }

        #endregion

        #region Callbacks

#if ENABLE_EDITOR_GAME_SERVICES
        private void OnProjectBindingChanged()
        {
            var environmentEnum = root.Q<EnumField>("EnvironmentVar");
            if (CloudProjectSettings.projectBound)
            {
                environmentEnum.SetEnabled(true);
                if ((EnvironmentType)environmentEnum.value == EnvironmentType.Automatic)
                    FetchCredentials();
            }
            else
            {
                server.value = "";
                domain.value = "";
                tokenIssuer.value = "";
                tokenKey.value = "";
                VivoxSettings.Instance.TokenKey = "";
                VivoxSettings.Instance.TokenIssuer = "";
                VivoxSettings.Instance.Server = "";
                VivoxSettings.Instance.Domain = "";
                VivoxSettings.Instance.Save();
                environmentEnum.value = EnvironmentType.Custom;
                tokenKey.SetEnabled(true);
                tokenIssuer.SetEnabled(true);
                domain.SetEnabled(true);
                server.SetEnabled(true);
                environmentEnum.SetEnabled(false);
            }
        }
#endif

        private void OnLoginStatusChange(LoginState state)
        {
            switch (state)
            {
                case LoginState.LoggedOut:
                    SetConnectionStatusUI(VivoxConnectionStatus.Failed);
                    break;
                case LoginState.LoggedIn:
                    SetConnectionStatusUI(VivoxConnectionStatus.Success);
                    break;
                default:
                    break;
            }
            vvx.OnUserLoggedInEvent -= OnLoginStatusChange;
        }

        private void OnTestConnectionButtonPressed()
        {
            if (string.IsNullOrEmpty(server.text) ||
                string.IsNullOrEmpty(domain.text) ||
                string.IsNullOrEmpty(tokenIssuer.text) ||
                string.IsNullOrEmpty(tokenKey.text))
            {
                SetConnectionStatusUI(VivoxConnectionStatus.Failed, "Empty Credentials");
                return;
            }

            SetConnectionStatusUI(VivoxConnectionStatus.Testing);
            SetInteractables(false);
            vvx = new VivoxTester(server.text, domain.text, tokenIssuer.text, tokenKey.text);
            vvx.OnUserLoggedInEvent += OnLoginStatusChange;
            try
            {
                vvx.Login();
                SaveCredentials();
            }
            catch (Exception)
            {
                SetConnectionStatusUI(VivoxConnectionStatus.Failed, "Invalid credentials");
                throw;
            }
        }

        void OnDeveloperPortalButtonPressed()
        {
            Application.OpenURL("https://docs.vivox.com/v5/general/unity/5_15_0/en-us/Default.htm");
        }

        void OnDashboardButtonPressed()
        {
            Application.OpenURL("https://dashboard.unity3d.com");
        }



        #endregion

    }

    /// <summary>
    /// A small container for housing a path to a particular icon and a status message.
    /// </summary>
    internal class VivoxMessage
    {
        public string StatusMessage { get; }
        public string IconPath { get; }

        public VivoxMessage(string message, string image)
        {
            StatusMessage = message;
            IconPath = image;
        }
    }
}
