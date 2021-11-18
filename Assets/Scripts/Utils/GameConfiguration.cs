using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// class to be extended to define the parameters of the game
/// </summary>
[Serializable]
public abstract class GameConfiguration
{
    public string sessionactid;

    public abstract override string ToString();

    public bool IsValidConfiguration(GameConfiguration config) {
        Type t = config.GetType();
        foreach (PropertyInfo p in t.GetProperties()) {
            Debug.Log(p.Name + " " + p.GetValue(config).ToString());
            if (p.GetCustomAttribute(typeof(PropertyOptional)) == null)
            {
                if (p.PropertyType == typeof(int) || p.PropertyType == typeof(float))
                {
                    if (p.GetCustomAttribute(typeof(PropertyRange)) != null)
                    {
                        int min = (p.GetCustomAttribute(typeof(PropertyRange)) as PropertyRange).min;
                        int max = (p.GetCustomAttribute(typeof(PropertyRange)) as PropertyRange).max;
                        if (float.Parse(p.GetValue(config).ToString()) > max || float.Parse(p.GetValue(config).ToString()) < min)
                        {
                            return false;
                        }
                    }
                }
                if (p.PropertyType == typeof(string))
                {
                    
                    if (p.GetValue(config).ToString() == null || p.GetValue(config).ToString() == "")
                    {
                        Debug.Log("empty string");
                        return false;
                    }
                    if (p.GetCustomAttribute(typeof(PropertyLimitedSet)) != null)
                    {
                        string[] values = (p.GetCustomAttribute(typeof(PropertyLimitedSet)) as PropertyLimitedSet).values;
                        if (!values.Contains(p.GetValue(config).ToString()))
                        {
                            Debug.Log("value not in list");
                            return false;
                        }
                    }
                    if (p.GetCustomAttribute(typeof(PropertyReferenceFolder)) != null){
                        PropertyReferenceFolder prf = (PropertyReferenceFolder)p.GetCustomAttribute(typeof(PropertyReferenceFolder));
                        string filename = p.GetValue(config).ToString();
                        Debug.Log(Application.streamingAssetsPath + "/" + prf.folder + "/" + filename + "." + prf.extension);
                        Debug.Log(File.Exists(Application.streamingAssetsPath + "/" + prf.folder + "/" + filename + "." + prf.extension));
                        Debug.Log(MagicRoomManager.instance.systemConfiguration.resourcesPath + "\\" + prf.folder + "\\" + filename + "." + prf.extension);
                        Debug.Log(File.Exists(MagicRoomManager.instance.systemConfiguration.resourcesPath + "\\" + prf.folder + "\\" + filename + "." + prf.extension));
                        return File.Exists(Application.streamingAssetsPath + "/" + prf.folder + "/" + filename + "." + prf.extension) || File.Exists(MagicRoomManager.instance.systemConfiguration.resourcesPath + "\\" + prf.folder + "\\" + filename + "." + prf.extension);
                    }
                    if (p.GetCustomAttribute(typeof(PropertyReferenceFolders)) != null)
                    {
                        PropertyReferenceFolders prf = (PropertyReferenceFolders)p.GetCustomAttribute(typeof(PropertyReferenceFolders));
                        string filename = p.GetValue(config).ToString();
                        foreach (string f in prf.folder) {
                            if (File.Exists(Application.streamingAssetsPath + "/" + f + "/" + filename + "." + prf.extension) || File.Exists(MagicRoomManager.instance.systemConfiguration.resourcesPath + "\\" + f + "\\" + filename + "." + prf.extension))
                            {
                                return true;
                            }

                        }
                        return false;
                    }
                }
            }
            else {
                if (p.GetCustomAttribute(typeof(PropertyDefaultValue)) != null) {
                    PropertyDefaultValue pdv = (PropertyDefaultValue)p.GetCustomAttribute(typeof(PropertyDefaultValue));
                    if (p.PropertyType == typeof(int)) {
                        p.SetValue(config, pdv.intvalue);
                    }
                    if (p.PropertyType == typeof(float))
                    {
                        p.SetValue(config, pdv.floatvalue);
                    }
                    if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(config, pdv.stringvalue);
                    }
                    if (p.PropertyType == typeof(Enum))
                    {
                        p.SetValue(config, pdv.enumvalue);
                    }
                    if (p.PropertyType == typeof(bool))
                    {
                        p.SetValue(config, pdv.boolvalue);
                    }
                }
            }
        }
        return true;
    }

    public void SetPropValue(string propName, JToken content, GameConfiguration config)
    {
        Type t = config.GetType();
        PropertyInfo p = t.GetProperty(propName);
        
        p.SetValue(this, Convert.ChangeType(content, p.PropertyType));
    }

    public void SetPropValue<T>(string propName, T content)
    {
        Type t = this.GetType();
        PropertyInfo p = t.GetProperty(propName);
        p.SetValue(this, content);
    }

    public static void SetConfigFromObject(JObject serialized, GameConfiguration config) {
        foreach (var x in serialized)
        {
            Debug.Log(x.Key + " " + x.Value);
            config.SetPropValue(x.Key, x.Value, config);
        }
    }
}


[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Property)]
public class PropertyRange : System.Attribute
{
    public int min;
    public int max;

    public PropertyRange(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Property)]
public class PropertyEnumReference : System.Attribute { 
    public System.Enum enumtype;

    public PropertyEnumReference(Enum enumtype)
    {
        this.enumtype = enumtype;
    }
}

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Property)]
public class PropertyLimitedSet : System.Attribute
{
    public string[] values;

    public PropertyLimitedSet(string[] values)
    {
        this.values = values;
    }
}

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Property)]
public class PropertyOptional : System.Attribute
{
    public bool values;

    public PropertyOptional()
    {
        this.values = true;
    }
}

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Property)]
public class PropertyRename : System.Attribute
{
    public string easyname;

    public PropertyRename(string rename)
    {
        easyname = rename;
    }
}

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Property)]
public class PropertyReferenceFolder : System.Attribute
{
    public string folder;
    public string extension;

    public PropertyReferenceFolder(string folder, string extension = "")
    {
        this.folder = folder;
        this.extension = extension;
    }
}


[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Property)]
public class PropertyReferenceFolders : System.Attribute
{
    public string[] folder;
    public string extension;

    public PropertyReferenceFolders(string[] folder, string extension = "")
    {
        this.folder = folder;
        this.extension = extension;
    }
}



[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Property)]
public class PropertyDefaultValue : System.Attribute
{
    public bool boolvalue;
    public int intvalue;
    public string stringvalue;
    public float floatvalue;
    private string _enumvalue;
    private Type _enumType;
    public Enum enumvalue { get {
            return (Enum)Enum.Parse(_enumType, _enumvalue);
        } }
    public PropertyDefaultValue(int value)
    {
        intvalue = value;
    }
    public PropertyDefaultValue(string value)
    {
        stringvalue = value;
    }
    public PropertyDefaultValue(float value)
    {
        floatvalue = value;
    }
    public PropertyDefaultValue(string value, Type enumType)
    {
        _enumType = enumType;
        _enumvalue = value;
    }
    public PropertyDefaultValue(bool value) {
        boolvalue = value;
    }
}