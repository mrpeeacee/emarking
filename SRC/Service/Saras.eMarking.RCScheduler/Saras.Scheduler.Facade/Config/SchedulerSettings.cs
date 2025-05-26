using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using Saras.SystemFramework.Core.Configuration;

namespace Saras.Scheduler.Facade.Config
{
    //public class SchedulerSettings : SerializableConfigurationSection
    //{
    //    /// <summary>
    //    /// Gets the configuration section name for the library.
    //    /// </summary>
    //    public const string SectionName = "saras.scheduler";
    //    private static SchedulerSettings settings = null;

    //    private const string settingsProperty = "settings";


    //    public static SchedulerSettings GetSettings(IConfigurationSource configurationSource)
    //    {
    //        return (SchedulerSettings)configurationSource.GetSection(SectionName);
    //    }

    //    public static SchedulerSettings GetSettings()
    //    {
    //        if (null == settings)
    //            settings = (SchedulerSettings)ConfigurationManager.GetSection(SectionName);
    //        return settings;
    //    }

    //    /// <summary>
    //    /// Initializes a new instance of an <see cref="SchedulerSettings"/> class.
    //    /// </summary>
    //    public SchedulerSettings()
    //    {
    //        this[settingsProperty] = new SchedulerSettingData();
    //    }



    //    [ConfigurationProperty(settingsProperty)]
    //    public SchedulerSettingData Settings
    //    {
    //        get { return (SchedulerSettingData)this[settingsProperty]; }
    //    }
    //}
}
