﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LiveSplit.Web.Share
{


    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    public sealed partial class ShareSettings : global::System.Configuration.ApplicationSettingsBase
    {

        private static ShareSettings defaultInstance = ((ShareSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ShareSettings())));

        public static ShareSettings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TwitterOAuthToken
        {
            get
            {
                return ((string)(this["TwitterOAuthToken"]));
            }
            set
            {
                this["TwitterOAuthToken"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TwitterAccessToken
        {
            get
            {
                return ((string)(this["TwitterAccessToken"]));
            }
            set
            {
                this["TwitterAccessToken"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TwitchAccessToken
        {
            get
            {
                return ((string)(this["TwitchAccessToken"]));
            }
            set
            {
                this["TwitchAccessToken"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TwitterFormat
        {
            get
            {
                return ((string)(this["TwitterFormat"]));
            }
            set
            {
                this["TwitterFormat"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TwitchFormat
        {
            get
            {
                return ((string)(this["TwitchFormat"]));
            }
            set
            {
                this["TwitchFormat"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TwitterFormatRunning
        {
            get
            {
                return ((string)(this["TwitterFormatRunning"]));
            }
            set
            {
                this["TwitterFormatRunning"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SRLIRCUsername
        {
            get
            {
                return ((string)(this["SRLIRCUsername"]));
            }
            set
            {
                this["SRLIRCUsername"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SRLIRCPassword
        {
            get
            {
                return ((string)(this["SRLIRCPassword"]));
            }
            set
            {
                this["SRLIRCPassword"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string SpeedrunComAccessToken
        {
            get
            {
                return ((string)(this["SpeedrunComAccessToken"]));
            }
            set
            {
                this["SpeedrunComAccessToken"] = value;
            }
        }
    }
}