using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OnlineModuleSettings", menuName = "Exoa/New OnlineModuleSettings")]
public class OnlineModuleSettings : ScriptableObject
{
    public bool useOnlineMode;
    public string scriptUrl;

    private static OnlineModuleSettings instance;

    public static OnlineModuleSettings GetSettings()
    {
        if (instance == null)
            instance = Resources.Load<OnlineModuleSettings>("OnlineModuleSettings");

        if (instance == null)
            Debug.LogError("Could not find any OnlineModuleSettings instance in Resouces/");
        return instance;
    }
}
