﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TrakHound_Client.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Generic.List<TH_Plugins.Client.PluginConfiguration> Plugin_Configurations {
            get {
                return ((global::System.Collections.Generic.List<TH_Plugins.Client.PluginConfiguration>)(this["Plugin_Configurations"]));
            }
            set {
                this["Plugin_Configurations"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public long Usage_MemoryUsed_Value {
            get {
                return ((long)(this["Usage_MemoryUsed_Value"]));
            }
            set {
                this["Usage_MemoryUsed_Value"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime Usage_MemoryUsed_Date {
            get {
                return ((global::System.DateTime)(this["Usage_MemoryUsed_Date"]));
            }
            set {
                this["Usage_MemoryUsed_Date"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int Usage_TimesOpened {
            get {
                return ((int)(this["Usage_TimesOpened"]));
            }
            set {
                this["Usage_TimesOpened"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int UpdateBehavior {
            get {
                return ((int)(this["UpdateBehavior"]));
            }
            set {
                this["UpdateBehavior"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool FirstOpen {
            get {
                return ((bool)(this["FirstOpen"]));
            }
            set {
                this["FirstOpen"] = value;
            }
        }
    }
}
