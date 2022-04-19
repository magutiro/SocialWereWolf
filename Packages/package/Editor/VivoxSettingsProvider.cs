using System.Collections.Generic;
using System;
using Unity.Services.Core.Editor;
using UnityEditor;
using VivoxUnity;
using UnityEditor.UIElements;
using UnityEditor.CrashReporting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Vivox.Editor
{
    class VivoxSettingsProvider : EditorGameServiceSettingsProvider
    {
        const string k_PackageName = "com.unity.services.vivox";
        const string k_IsServiceEnabledKey = "isServiceEnabled";
        const string k_IsTestModeKey = "isTestMode";
        const string k_Title = "Vivox";

        VivoxSettingsProvider(string name, SettingsScope scope, IEnumerable<string> keywords = null)
            : base(name, scope, keywords) {}

        protected override IEditorGameService EditorGameService
            => EditorGameServiceRegistry.Instance.GetEditorGameService<VivoxIdentifier>();

        protected override string Title => k_Title;

        protected override string Description
            => "Deliver the best voice and text communications for your players with Unity's managed solution. Developers using pre-2022.1 versions of the editor or Vivox Developer Portal credentials should go to the Services > Vivox > Configure menu, which is located in the upper area of the editor window.";

        protected override VisualElement GenerateServiceDetailUI()
        {
            var containerAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath.Common);
            VisualElement containerUI = null;
            if (containerAsset != null)
            {
                containerUI = containerAsset.CloneTree().contentContainer;

                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(UssPath.Common);
                if (styleSheet != null)
                {
                    containerUI.styleSheets.Add(styleSheet);
                    SetUpDashboardButton(containerUI);
                    SetUpDocumentationButton(containerUI);
                    SetUpCredentials(containerUI);
                    SetUpTestToggle(containerUI);
                    FetchCredentials(containerUI);
#if !ENABLE_EDITOR_GAME_SERVICES
                    RemoveExternalLinkIcons(containerUI);
#endif
#if !AUTH_PACKAGE_PRESENT
                    SetUpAuthPackageWarning(containerUI);
#endif
                }
            }

            return containerUI;
        }

        protected override VisualElement GenerateUnsupportedDetailUI()
        {
            return GenerateServiceDetailUI();
        }

        static void SetUpTestToggle(VisualElement parentElement)
        {
            var testModeToggle = parentElement.Q<Toggle>(UxmlNode.TestModeToggle);
            testModeToggle.value = VivoxSettings.Instance.IsTestMode;
            testModeToggle.RegisterValueChangedCallback((ChangeEvent<bool> testModeChange) => {
                var settings = VivoxSettings.Instance;
                settings.IsTestMode = testModeChange.newValue;
                if (!testModeChange.newValue)
                {
                    settings.TokenKey = "";
                    var KeyVar = parentElement.Q<TextElement>(UxmlNode.KeyVar);
                    KeyVar.text = "";
                }
                else
                {
                    FetchCredentials(parentElement);
                }
                settings.Save();
            });
        }

        static void SetUpAuthPackageWarning(VisualElement parentElement)
        {
            var authPackageWarning = parentElement.Q<TextElement>(UxmlNode.AuthPackageWarning);
            authPackageWarning.text = "The Authentication Package has not been imported. For the easiest Vivox Token Vending experience, import the Unity Authentication Package. Otherwise, select the Documentation link and then refer to the Access Token Developer Guide.";
        }

        static void RemoveExternalLinkIcons(VisualElement parentElement)
        {
            var externalLinks = parentElement.Q<VisualElement>(UxmlNode.ExternalLinks + "Dash");
            externalLinks.visible = false;
            externalLinks = parentElement.Q<VisualElement>(UxmlNode.ExternalLinks + "Docs");
            externalLinks.visible = false;
        }

        static void SetUpDashboardButton(VisualElement parentElement)
        {
            var dashboardSdkButton = parentElement.Q(UxmlNode.Dashboard);
            if (dashboardSdkButton != null)
            {
                var clickable = new Clickable(() =>
                {
                    //EditorGameServiceAnalyticsSender.SendProjectSettingsDownloadUserReportingSDKEvent();
                    Application.OpenURL(URL.Dashboard);
                });
                dashboardSdkButton.AddManipulator(clickable);
            }
        }

        static void SetUpDocumentationButton(VisualElement parentElement)
        {
            var downloadSdkButton = parentElement.Q(UxmlNode.Documentation);
            if (downloadSdkButton != null)
            {
                var clickable = new Clickable(() =>
                {
                    //EditorGameServiceAnalyticsSender.SendProjectSettingsDownloadUserReportingSDKEvent();
                    Application.OpenURL(URL.Documentation);
                });
                downloadSdkButton.AddManipulator(clickable);
            }
        }

        static void SetUpCredentials(VisualElement parentElement)
        {
            var ServerVar = parentElement.Q<TextElement>(UxmlNode.ServerVar);
            var DomainVar = parentElement.Q<TextElement>(UxmlNode.DomainVar);
            var IssuerVar = parentElement.Q<TextElement>(UxmlNode.IssuerVar);
            var KeyVar = parentElement.Q<TextElement>(UxmlNode.KeyVar);

            ServerVar.text = VivoxSettings.Instance.Server;
            DomainVar.text = VivoxSettings.Instance.Domain;
            KeyVar.text = VivoxSettings.Instance.TokenKey;
            IssuerVar.text = VivoxSettings.Instance.TokenIssuer;
            ServerVar.SetEnabled(false);
            DomainVar.SetEnabled(false);
            KeyVar.SetEnabled(false);
            IssuerVar.SetEnabled(false);
        }

        private static void FetchCredentials(VisualElement parentElement)
        {
            var ServerVar = parentElement.Q<TextElement>(UxmlNode.ServerVar);
            var DomainVar = parentElement.Q<TextElement>(UxmlNode.DomainVar);
            var IssuerVar = parentElement.Q<TextElement>(UxmlNode.IssuerVar);
            var KeyVar = parentElement.Q<TextElement>(UxmlNode.KeyVar);
            if (String.IsNullOrEmpty(CloudProjectSettings.projectId))
            {
                Debug.LogError("[Vivox]: ProjectId not set. You must link a Unity Dashboard project with Vivox Credentials before using the Vivox Services Page");
                return;
            }
            VivoxApiClient.Instance.GetGatewayToken(OnGatewayTokenFetched, Debug.LogError);

            void OnGatewayTokenFetched(TokenExchangeResponse gateResp)
            {
                VivoxApiClient.Instance.GetCredentials(gateResp.Token, OnCredentialsFetched, HandleException);
                void OnCredentialsFetched(GetVivoxCredentialsResponse credResp)
                {
                    if (VivoxSettings.Instance.IsTestMode)
                    {
                        VivoxSettings.Instance.TokenKey = credResp.Credentials.Key;
                    }
                    VivoxSettings.Instance.TokenIssuer = credResp.Credentials.Issuer;
                    VivoxSettings.Instance.Server = credResp.Credentials.Environment.ServerUri;
                    VivoxSettings.Instance.Domain = credResp.Credentials.Environment.Domain;
                    VivoxSettings.Instance.PulledCredentialProjectId = CloudProjectSettings.projectId;
                    VivoxSettings.Instance.Save();


                    ServerVar.text = VivoxSettings.Instance.Server;
                    DomainVar.text = VivoxSettings.Instance.Domain;
                    KeyVar.text = VivoxSettings.Instance.TokenKey;
                    IssuerVar.text = VivoxSettings.Instance.TokenIssuer;
                }
                void HandleException(Exception exception)
                {
                    ServerVar.text = "";
                    DomainVar.text = "";
                    KeyVar.text = "";
                    IssuerVar.text = "";
                    if (exception.Message == "Object reference not set to an instance of an object")
                    {
                        Debug.LogError("[Vivox]: Failed to pull Credentials from the Unity Dashboard. Ensure that the associated project has Vivox enabled and has generated Vivox Credentials on the Unity Dashboard.");
                        return;
                    }
                    Debug.LogError(exception);
                }
            }
        }

        [InitializeOnLoadMethod]
        static void FetchCredentialsOnLoad()
        {
            if (String.IsNullOrEmpty(CloudProjectSettings.projectId))
            {
                VivoxSettings.Instance.IsEnvironmentCustom = true;
                VivoxSettings.Instance.Save();
                return;
            }
            if(VivoxSettings.Instance.IsEnvironmentCustom && (!String.IsNullOrEmpty(VivoxSettings.Instance.TokenKey) || !String.IsNullOrEmpty(VivoxSettings.Instance.TokenIssuer) || !String.IsNullOrEmpty(VivoxSettings.Instance.Server) || !String.IsNullOrEmpty(VivoxSettings.Instance.Domain)))
            {
                return;
            }
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
                    VivoxSettings.Instance.IsEnvironmentCustom = false;
                    VivoxSettings.Instance.PulledCredentialProjectId = CloudProjectSettings.projectId;
                    VivoxSettings.Instance.Save();
                }
                void HandleException(Exception exception)
                {
                    VivoxSettings.Instance.IsEnvironmentCustom = true;
                    VivoxSettings.Instance.Save();
                    if (exception.Message == "Object reference not set to an instance of an object")
                    {
                        Debug.LogError("[Vivox]: Failed to pull Credentials from the Unity Dashboard. Ensure that the associated project has Vivox enabled and has generated Vivox Credentials on the Unity Dashboard.");
                        return;
                    }
                    Debug.LogError(exception);
                }
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new VivoxSettingsProvider(GenerateProjectSettingsPath(k_Title), SettingsScope.Project);
        }

        static class UxmlPath
        {
            public const string Common = "Packages/com.unity.services.vivox/Editor/UXML/VivoxPackageEditor.uxml";
        }

        static class UssPath
        {
            public const string Common = "Packages/com.unity.services.vivox/Editor/StyleSheets/VivoxPackageEditor.uss";
        }

        static class UxmlNode
        {
            public const string Dashboard = "DashboardLinkButton";
            public const string Documentation = "DocumentationLinkButton";
            public const string CheckConnection = "ConnectionButton";
            public const string ServerVar = "ServerVar";
            public const string DomainVar = "DomainVar";
            public const string IssuerVar = "IssuerVar";
            public const string KeyVar = "KeyVar";
            public const string TestModeToggle = "TestToggle";
            public const string ExternalLinks = "ExternalLink";
            public const string AuthPackageWarning = "AuthPackageWarning";
        }

        static class URL
        {
            public const string Dashboard = "https://dashboard.unity3d.com";
            public const string Documentation = "https://docs.vivox.com/v5/general/unity/5_15_0/Default.htm";
        }
    }
}
