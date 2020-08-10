using System;

[Serializable]
public class PlayerData {
    public string savedVersion;
    public int totalPlaneLanded;
    public string[] unlockedLevel;
    public string lastPlayedLevelID;
    public SettingData settingData = new SettingData ();
}

[Serializable]
public class SettingData {
    public bool useMusic = true;
    public bool useSoundFX = true;
    public bool useVibrate = true;
}