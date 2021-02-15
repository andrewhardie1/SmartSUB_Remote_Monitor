using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSUB_Remote_Monitor
{
    public static class AppConstants
    {
        //Notification Hub
        public static string NotificationChannelName { get; set; } = "XamarinNotifyChannel";
        public static string NotificationHubName { get; set; } = "SmartSUB";
        public static string ListenConnectionString { get; set; } = "Endpoint=sb://notificationhubsmartsubalarms.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=UaaRk2GVs+umbGc96ijwafFIXBE8t6eB0NlM/1JN72Q=";
        public static string DebugTag { get; set; } = "XamarinNotify";
        public static string[] SubscriptionTags { get; set; } = { "default" };
        public static string FCMTemplateBody { get; set; } = "{\"data\":{\"message\":\"$(messageParam)\"}}";
        public static string APNTemplateBody { get; set; } = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";

        //Azure Active Directory
        static readonly string tenantName = "SmartSUB";
        static readonly string tenantId = "SmartSUB.onmicrosoft.com";
        static readonly string clientId = "3a7ae87e-7feb-4c9d-a50e-ecbba72c90eb";
        static readonly string policySignin = "b2c_1_signin";
        static readonly string policyPassword = "b2c_1_passwordreset";
        static readonly string iosKeychainSecurityGroup = "com.smartsub.xamarinfcmapp";

        static readonly string[] scopes = { "openid", "offline_access" };
        static readonly string authorityBase = $"https://{tenantName}.b2clogin.com/tfp/{tenantId}/";
        public static string ClientId
        {
            get
            {
                return clientId;
            }
        }
        public static string AuthoritySignin
        {
            get
            {
                return $"{authorityBase}{policySignin}";
            }
        }
        public static string AuthorityPasswordReset
        {
            get
            {
                return $"{authorityBase}{policyPassword}";
            }
        }
        public static string[] Scopes
        {
            get
            {
                return scopes;
            }
        }
        public static string IosKeychainSecurityGroups
        {
            get
            {
                return iosKeychainSecurityGroup;
            }
        }
    }
}
