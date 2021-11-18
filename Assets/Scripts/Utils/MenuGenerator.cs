using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MenuGenerator : MonoBehaviour
{
    public bool generateAtRuntime = false;

    public GameConfiguration configurationschema;

    public string configurationname;

    public string ThemePropertyName = "";

    [HideInInspector]
    public bool hastheme = false;

    public bool allowOnlyCompleteThemes = false;

    [HideInInspector]
    public Button playgame;

    [HideInInspector]
    public List<PropertyMenuBlockManager> blocks = new List<PropertyMenuBlockManager>();

    private PropertyMenuBlockManager theme;
    private GameObject grid;

    public int GameSceneIndex = 2;

    private void Start()
    {
        if (GameSetting.instance != null)
        {
            GameSetting.instance.configuration = (GameConfiguration)Activator.CreateInstance(Type.GetType(configurationname));
            configurationschema = GameSetting.instance.configuration;
            ThemeManager.allowOnlyFullThemes = allowOnlyCompleteThemes;
        }
        else
        {
            configurationschema = (GameConfiguration)Activator.CreateInstance(Type.GetType(configurationname));
        }
        if (generateAtRuntime)
        {
            generateMenu();
        }
        else
        {
            resetGrid();
            if (ThemePropertyName != "")
            {
                ThemeManager.StartUp();
                theme = GameObject.Find("MenuApplicationTitleAndTheme(Clone)").GetComponent<PropertyMenuBlockManager>();
                theme.loadCurrentThemeDetail();
            }
        }
        setMinValues();
    }

    private void setMinValues()
    {
        if (GameSetting.instance != null)
        {
            PropertyInfo[] props = GetProperties();
            foreach (PropertyInfo p in props)
            {
                string propertyname = p.Name;
                if (propertyname != ThemePropertyName)
                {
                    object[] attrs = p.GetCustomAttributes(true);
                    foreach (Attribute a in attrs)
                    {
                        if (a.GetType().ToString() == "PropertyRange")
                        {
                            PropertyRange range = a as PropertyRange;
                            p.SetValue(GameSetting.instance.configuration, range.min);
                        }
                        if (a.GetType().ToString() == "PropertyDefaultValue")
                        {
                            PropertyDefaultValue pdv = a as PropertyDefaultValue;
                            if (p.PropertyType == typeof(int))
                            {
                                p.SetValue(GameSetting.instance.configuration, pdv.intvalue);
                            }
                            if (p.PropertyType == typeof(float))
                            {
                                p.SetValue(GameSetting.instance.configuration, pdv.floatvalue);
                            }
                            if (p.PropertyType == typeof(string))
                            {
                                p.SetValue(GameSetting.instance.configuration, pdv.stringvalue);
                            }
                            if (p.PropertyType == typeof(Enum))
                            {
                                p.SetValue(GameSetting.instance.configuration, pdv.enumvalue);
                            }
                            if (p.PropertyType == typeof(bool))
                            {
                                p.SetValue(GameSetting.instance.configuration, pdv.boolvalue);
                            }
                        }
                    }
                    foreach (PropertyMenuBlockManager pm in GetComponentsInChildren<PropertyMenuBlockManager>())
                    {
                        if (pm.gameObject.name == p.Name)
                        {
                            pm.ResetToInitialValue(p.GetValue(configurationschema));
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        var type = Type.GetType(configurationname);
        configurationschema = (GameConfiguration)Activator.CreateInstance(type);
        ThemeManager.allowOnlyFullThemes = allowOnlyCompleteThemes;
    }

    private T GetPropValue<T>(string propName, object obj)
    {
        Type t = configurationschema.GetType();
        PropertyInfo p = t.GetProperty(propName);
        return (T)p.GetValue(obj);
    }

    private PropertyInfo[] GetProperties()
    {
        Type t = configurationschema.GetType();
        return t.GetProperties();
    }

    public void generateMenu()
    {
        blocks = new List<PropertyMenuBlockManager>();
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }

        grid = GameObject.Instantiate(Resources.Load("UI/PropertyPanel") as GameObject, transform);

        PropertyInfo[] props = GetProperties();

        GridLayoutGroup gridlayout = grid.GetComponent<GridLayoutGroup>();
        float screenwidth = generateAtRuntime ? Screen.width : 1920;
        float screenheight = generateAtRuntime ? Screen.height : 1080;
        float blocky = Mathf.Max(80, screenheight / (props.Length + 1));
        gridlayout.cellSize = new Vector2(screenwidth * 0.4f, blocky);
        gridlayout.spacing = new Vector2(screenwidth * 0.1f, -(screenheight - blocky * (props.Length + 1)) / 2);
        gridlayout.padding = new RectOffset((int)(screenwidth * 0.05f), 0, (int)blocky / 2, (int)blocky / 2);

        foreach (PropertyInfo p in props)
        {
            string propertyname = p.Name;
            string easyname = "";
            if (propertyname != ThemePropertyName)
            {
                object[] attrs = p.GetCustomAttributes(true);
                foreach (Attribute a in attrs)
                {
                    if (a.GetType().ToString() == "PropertyRename")
                    {
                        PropertyRename rnm = a as PropertyRename;
                        easyname = rnm.easyname;
                    }
                }
                GameObject g = null;
                if (p.PropertyType == typeof(int))
                {
                    g = GameObject.Instantiate(Resources.Load("UI/PropertyMenuBlockNumber") as GameObject, transform.GetChild(0));
                    g.GetComponent<PropertyMenuBlockManager>().SetUp<int>(propertyname, (int)p.GetValue(configurationschema), generateAtRuntime, easyname);

                    foreach (Attribute a in attrs)
                    {
                        if (a.GetType().ToString() == "PropertyRange")
                        {
                            PropertyRange r = a as PropertyRange;
                            g.GetComponent<PropertyMenuBlockManager>().SetUpLimits(r.min, r.max);
                        }
                    }
                }
                if (p.PropertyType == typeof(float))
                {
                    g = GameObject.Instantiate(Resources.Load("UI/PropertyMenuBlockNumber") as GameObject, transform.GetChild(0));
                    g.GetComponent<PropertyMenuBlockManager>().SetUp<float>(propertyname, (float)p.GetValue(configurationschema), generateAtRuntime, easyname);

                    foreach (Attribute a in attrs)
                    {
                        if (a.GetType().ToString() == "PropertyRange")
                        {
                            PropertyRange r = a as PropertyRange;
                            g.GetComponent<PropertyMenuBlockManager>().SetUpLimits(r.min, r.max);
                        }
                    }
                }
                if (p.PropertyType == typeof(string))
                {
                    foreach (Attribute a in attrs)
                    {
                        if (a.GetType().ToString() == "PropertyLimitedSet")
                        {
                            PropertyLimitedSet s = a as PropertyLimitedSet;
                            g = GameObject.Instantiate(Resources.Load("UI/PropertyMenuBlockDropdown") as GameObject, transform.GetChild(0));
                            List<string> ls = new List<string>();
                            ls.AddRange(s.values);
                            g.GetComponent<PropertyMenuBlockManager>().SetUp<string>(propertyname, s.values[0], generateAtRuntime, easyname, ls);
                        }
                        else if (a.GetType().ToString() == "PropertyReferenceFolder")
                        {
                            PropertyReferenceFolder prf = a as PropertyReferenceFolder;
                            g = GameObject.Instantiate(Resources.Load("UI/PropertyMenuBlockDropdown") as GameObject, transform.GetChild(0));
                            List<string> ls = new List<string>();
                            foreach (string f in Directory.GetFiles(Application.streamingAssetsPath + "/" + prf.folder))
                            {
                                string file = f.Split('/').Last().Split('\\').Last();
                                if (file.EndsWith(prf.extension))
                                {
                                    ls.Add(file);
                                }
                            }

                            g.GetComponent<PropertyMenuBlockManager>().SetUp<string>(propertyname, ls.First(), generateAtRuntime, easyname, ls);
                        }
                        else
                        {
                            g = GameObject.Instantiate(Resources.Load("UI/PropertyMenuBlockText") as GameObject, transform.GetChild(0));
                            g.GetComponent<PropertyMenuBlockManager>().SetUp<string>(propertyname, "", generateAtRuntime, easyname);
                        }
                    }
                }
                if (p.PropertyType == typeof(bool))
                {
                    g = GameObject.Instantiate(Resources.Load("UI/PropertyMenuBlockBool") as GameObject, transform.GetChild(0));
                    g.GetComponent<PropertyMenuBlockManager>().SetUp<bool>(propertyname, (bool)p.GetValue(configurationschema), generateAtRuntime, easyname);
                }
                if (p.PropertyType.IsEnum)
                {
                    g = GameObject.Instantiate(Resources.Load("UI/PropertyMenuBlockDropdown") as GameObject, transform.GetChild(0));
                    g.GetComponent<PropertyMenuBlockManager>().SetUp<System.Enum>(propertyname, (System.Enum)p.GetValue(configurationschema), generateAtRuntime, easyname);
                }

                blocks.Add(g.GetComponent<PropertyMenuBlockManager>());
            }
        }

        if (ThemePropertyName == "")
        {
            GameObject title = Instantiate(Resources.Load("UI/MenuApplicationTitle") as GameObject, transform);
            title.GetComponentInChildren<Text>().text = Application.productName;
        }
        else
        {
            GameObject title = Instantiate(Resources.Load("UI/MenuApplicationTitleAndTheme") as GameObject, transform);
            ThemeManager.StartUp();
            title.GetComponent<PropertyMenuBlockManager>().SetupTheme(ThemePropertyName, generateAtRuntime);
            theme = title.GetComponent<PropertyMenuBlockManager>();
            blocks.Add(theme);
        }

        GameObject PlayButton = Instantiate(Resources.Load("UI/PlayButton") as GameObject, transform);
        playgame = PlayButton.GetComponentInChildren<Button>();
        if (generateAtRuntime)
        {
            PlayButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { PlayGame(); });
        }
    }

    public void OnClickPlayGame(GameObject sender)
    {
        PlayGame();
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(GameSceneIndex);
        MagicRoomManager.instance.ExperienceManagerComunication.SendEvent("started");
    }

    public void resetGrid()
    {
        grid = transform.GetChild(0).gameObject;
        PropertyInfo[] props = GetProperties();
        GridLayoutGroup gridlayout = grid.GetComponent<GridLayoutGroup>();
        float screenwidth = Screen.width;
        float screenheight = Screen.height;
        float blocky = Mathf.Max(80, screenheight / (props.Length + 1));
        gridlayout.cellSize = new Vector2(screenwidth * 0.4f, blocky);
        gridlayout.spacing = new Vector2(screenwidth * 0.1f, -(screenheight - blocky * (props.Length + 1)) / 2);
        gridlayout.padding = new RectOffset((int)(screenwidth * 0.05f), 0, (int)blocky / 2, (int)blocky / 2);
    }
}

public static class ReflectiveEnumerator
{
    static ReflectiveEnumerator()
    {
    }

    public static List<T> GetEnumerableOfType<T>() where T : class
    {
        List<T> objects = new List<T>();
        foreach (Type type in
            Assembly.GetAssembly(typeof(T)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
        {
            objects.Add((T)Activator.CreateInstance(type));
        }
        objects.Sort();
        return objects;
    }
}