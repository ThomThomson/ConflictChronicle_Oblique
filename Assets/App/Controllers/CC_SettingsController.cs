using System.IO;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using UnityEngine;

public static class CC_SettingsController
{
    public static CC_SettingsModel gameSettings = InitializeSettings();

    private static CC_SettingsModel InitializeSettings()
    {
        string savedSettingsLocation = Application.dataPath + @"/App/SavedSettings.json";
        CC_SettingsModel settings;
        if (File.Exists(savedSettingsLocation))
        {
            settings = JsonConvert.DeserializeObject<CC_SettingsModel>(File.ReadAllText(savedSettingsLocation));
        }
        else
        {
            settings = new CC_SettingsModel(true);
            File.AppendAllText(savedSettingsLocation, JsonConvert.SerializeObject(settings));
        }
        return settings;
    }
}