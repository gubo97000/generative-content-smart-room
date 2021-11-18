using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class ThemeSchema
{
    public string id;
    public string sound_associated;
    public string title;
    public string background_floor;
    public string background_front;
    public string background_sound;
    public string fragrance;
    public string fixed_light;
    public ThemeObjects[] objs;
}

[Serializable]
public class ThemeObjects {
    public string objname;
    public ImageBlock images;
    public string audio;
    public string video;
    public string tags;
    public string tagRFID;
}

[Serializable]
public class ImageBlock {
    public string defaults;
}

public class ThemeManager{
    public static ThemeSchema activeTheme;

    public static bool allowOnlyFullThemes = false;

    private static Dictionary<string,ThemeSchema> _listOfThemes;

    private static int themeIndex = 0;

    public static void StartUp()
    {
        _listOfThemes = new Dictionary<string, ThemeSchema>();
        Debug.LogError("theme directory " + MagicRoomManager.instance.systemConfiguration.resourcesPath);
        string themedirectory = MagicRoomManager.instance.systemConfiguration.resourcesPath + "/Themes";// "C://LUDOMI/Themes";
        Debug.Log(themedirectory);
        if (Directory.Exists(themedirectory))
        {
            foreach (string s in Directory.GetFiles(themedirectory))
            {

                string json = File.ReadAllText(s);
                ThemeSchema tm = JsonUtility.FromJson<ThemeSchema>(json);
                _listOfThemes.Add(tm.id, tm);
            }
        }
        else {
            themedirectory = MagicRoomManager.instance.systemConfiguration.resourcesPath + "/themes";// "C://LUDOMI/Themes";
            Debug.Log(themedirectory);
            if (Directory.Exists(themedirectory))
            {
                foreach (string s in Directory.GetFiles(themedirectory))
                {

                    string json = File.ReadAllText(s);
                    ThemeSchema tm = JsonUtility.FromJson<ThemeSchema>(json);
                    _listOfThemes.Add(tm.id, tm);
                }
            }
        }
        if (allowOnlyFullThemes) {
            do
            {
                themeIndex = (themeIndex + 1) % _listOfThemes.Count;
            } while (allowOnlyFullThemes && (_listOfThemes[_listOfThemes.Keys.ElementAt(themeIndex)].sound_associated != "1" || themeIndex == _listOfThemes.Count));
        }
        else
        {
            themeIndex = 0;
        }
        activeTheme = _listOfThemes[_listOfThemes.Keys.ElementAt(themeIndex)];
    }

    public static string[] getListOfThemes() {
        return _listOfThemes.Keys.ToArray();
    }


    public static ThemeSchema getNextThemeName() {
        int oldIndex = themeIndex;
        do {
            themeIndex = (themeIndex + 1) % _listOfThemes.Count;
        } while (allowOnlyFullThemes && (_listOfThemes[_listOfThemes.Keys.ElementAt(themeIndex)].sound_associated != "1" || themeIndex == oldIndex));
        string name = _listOfThemes.Keys.ElementAt(themeIndex);
        activeTheme = _listOfThemes[name];
        return _listOfThemes[name];
    }

    public static ThemeSchema getPreviousThemeName()
    {
        int oldIndex = themeIndex;
        do
        {
            themeIndex = (themeIndex - 1) % _listOfThemes.Count;
            if (themeIndex < 0)
            {
                themeIndex += _listOfThemes.Count;
            }
        } while (allowOnlyFullThemes && (_listOfThemes[_listOfThemes.Keys.ElementAt(themeIndex)].sound_associated != "1" || themeIndex == oldIndex));
        string name = _listOfThemes.Keys.ElementAt(themeIndex);
        activeTheme = _listOfThemes[name];
        return _listOfThemes[name];
    }

    public static void SetThemeByName(string name)
    {
        foreach (var t in _listOfThemes)
        {
            if (t.Key == name)
            {
                activeTheme = t.Value;
                return;
            }
        }
    }
}
