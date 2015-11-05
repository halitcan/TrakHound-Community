﻿using System;

using System.ComponentModel.Composition;
using System.Windows.Media;

using System.Data;

using TH_Configuration;

namespace TH_PlugIns_Server
{
    [InheritedExport(typeof(ConfigurationPage))]
    public interface ConfigurationPage
    {

        string PageName { get; }

        ImageSource Image { get; }

        event SettingChanged_Handler SettingChanged;

        void LoadConfiguration(DataTable dt);

        void SaveConfiguration(DataTable dt);

        //string PageName { get; }

        //ImageSource Image { get; }

        //event SettingChanged_Handler SettingChanged;

        //void LoadConfiguration(DataTable dt);

        //void SaveConfiguration(DataTable dt);

    }

    public delegate void SaveRequest_Handler();
    public delegate void SettingChanged_Handler(string name, string oldVal, string newVal);

}
