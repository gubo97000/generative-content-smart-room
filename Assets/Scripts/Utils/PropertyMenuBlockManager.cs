using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PropertyMenuBlockManager : MonoBehaviour
{
    [HideInInspector]
    public Text propertyname;

    [HideInInspector]
    public Slider slider;

    [HideInInspector]
    public Text valueDisplayer;

    [HideInInspector]
    public InputField textfield;

    [HideInInspector]
    public Toggle boolfield;

    [HideInInspector]
    public Image showImage;

    [HideInInspector]
    public Button prevButton;

    [HideInInspector]
    public Button nextButton;

    [HideInInspector]
    public Dropdown optiondropdown;

    private Type enumtype;
    private string themePropertyName;
    private bool hasstringoptions = false;

    private void SetUpComponents()
    {
        propertyname = transform.GetChild(0).GetComponent<Text>();
        slider = transform.GetComponentInChildren<Slider>();
        textfield = transform.GetComponentInChildren<InputField>();
        boolfield = transform.GetComponentInChildren<Toggle>();
        showImage = transform.GetComponentInChildren<Image>();
        optiondropdown = transform.GetComponentInChildren<Dropdown>();
        foreach (Text t in transform.GetComponentsInChildren<Text>())
        {
            if (t.gameObject != propertyname.gameObject)
            {
                valueDisplayer = t;
            }
        }
        foreach (Button b in transform.GetComponentsInChildren<Button>())
        {
            if (prevButton == null)
            {
                prevButton = b;
            }
            else
            {
                nextButton = b;
            }
        }
    }

    public bool Hasstringoptions { get => hasstringoptions; }

    public void SetUp<T>(string propertyname, T propertyvalue, bool activateListeners, string easyname = "", List<string> optionblock = null)
    {
        SetUpComponents();
        gameObject.name = propertyname;
        this.propertyname.text = easyname == "" ? propertyname : easyname;
        if (propertyvalue.GetType() == typeof(int) || propertyvalue.GetType() == typeof(float))
        {
            slider.value = float.Parse(propertyvalue.ToString());
            slider.wholeNumbers = propertyvalue.GetType() == typeof(int) ? true : false;
            valueDisplayer.text = propertyvalue.ToString();
            if (activateListeners)
            {
                slider.onValueChanged.AddListener(delegate { ValueNumericChangeCheck(); });
            }
        }
        if (propertyvalue.GetType() == typeof(string))
        {
            if (optionblock != null)
            {
                optiondropdown.ClearOptions();
                optiondropdown.AddOptions(optionblock);
                if (activateListeners)
                {
                    optiondropdown.onValueChanged.AddListener(delegate { ValueStringOptionChangeCheck(); });
                }
                hasstringoptions = true;
            }
            else
            {
                if (activateListeners)
                {
                    textfield.onEndEdit.AddListener(delegate { ValueTextChangeCheck(); });
                }
            }
        }
        if (propertyvalue.GetType() == typeof(bool))
        {
            if (activateListeners)
            {
                boolfield.onValueChanged.AddListener(delegate { ValueBoolChangeCheck(); });
            }
        }
        if (propertyvalue.GetType().IsEnum)
        {
            List<string> options = new List<string>();
            foreach (T t in Enum.GetValues(propertyvalue.GetType()))
            {
                options.Add(t.ToString());
            }
            enumtype = propertyvalue.GetType();
            optiondropdown.ClearOptions();
            optiondropdown.AddOptions(options);
            if (activateListeners)
            {
                optiondropdown.onValueChanged.AddListener(delegate { ValueEnumChangeCheck(); });
            }
        }
    }

    public void SetupTheme(string themepropertyName, bool activateListeners)
    {
        propertyname.text = Application.productName;
        themePropertyName = themepropertyName;
        if (activateListeners)
        {
            prevButton.onClick.AddListener(delegate { ThemeManager.getPreviousThemeName(); loadCurrentThemeDetail(); });
            nextButton.onClick.AddListener(delegate { ThemeManager.getNextThemeName(); loadCurrentThemeDetail(); });
        }
        loadCurrentThemeDetail();
    }

    public void ResetToInitialValue<T>(T propertyvalue)
    {
        if (propertyvalue.GetType() == typeof(int) || propertyvalue.GetType() == typeof(float))
        {
            slider.value = float.Parse(propertyvalue.ToString());
        }
        if (propertyvalue.GetType() == typeof(bool))
        {
            boolfield.isOn = bool.Parse(propertyvalue.ToString());
        }
        if (propertyvalue.GetType().IsEnum)
        {
            optiondropdown.value = (int)Enum.Parse(propertyvalue.GetType(), propertyvalue.ToString());
            optiondropdown.RefreshShownValue();
        }
        if (propertyvalue.GetType() == typeof(string))
        {
            if (optiondropdown != null)
            {
                optiondropdown.value = optiondropdown.options.IndexOf(optiondropdown.options.FirstOrDefault(x => x.text == propertyvalue.ToString()));
                optiondropdown.RefreshShownValue();
            }
            else
            {
                textfield.text = propertyvalue.ToString();
            }
        }
    }

    public void loadCurrentThemeDetail()
    {
        ThemeSchema tm = ThemeManager.activeTheme;
        if (StreamingAssetManager.instance != null)
        {
            StreamingAssetManager.instance.LoadImageFromStreamingAsset("images", tm.objs[0].images.defaults, (tex) =>
            {
                showImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            });
        }
        valueDisplayer.text = tm.id;
    }

    public void SliderEvent(GameObject g)
    {
        ValueNumericChangeCheck();
    }

    public void InputTextEvent(GameObject g)
    {
        ValueTextChangeCheck();
    }

    public void TogglerEvent(GameObject g)
    {
        ValueBoolChangeCheck();
    }

    public void PrevButtonEvent(GameObject g)
    {
        ThemeManager.getPreviousThemeName();
        loadCurrentThemeDetail();
    }

    public void NextButtonEvent(GameObject g)
    {
        ThemeManager.getNextThemeName();
        loadCurrentThemeDetail();
    }

    public void DropdownEvent(GameObject g)
    {
        ValueEnumChangeCheck();
    }

    public void StringOptionEvent(GameObject g)
    {
        ValueStringOptionChangeCheck();
    }

    public void ValueNumericChangeCheck()
    {
        valueDisplayer.text = slider.value.ToString();
        if (GameSetting.instance != null)
        {
            if (slider.wholeNumbers)
            {
                GameSetting.instance.configuration.SetPropValue(gameObject.name, (int)slider.value);
            }
            else
            {
                GameSetting.instance.configuration.SetPropValue(gameObject.name, slider.value);
            }
        }
    }

    public void ValueTextChangeCheck()
    {
        if (GameSetting.instance != null)
        {
            GameSetting.instance.configuration.SetPropValue(gameObject.name, textfield.text);
        }
    }

    public void ValueBoolChangeCheck()
    {
        if (GameSetting.instance != null)
        {
            GameSetting.instance.configuration.SetPropValue(gameObject.name, boolfield.isOn);
        }
    }

    public void ValueEnumChangeCheck()
    {
        if (GameSetting.instance != null)
        {
            GameSetting.instance.configuration.SetPropValue(gameObject.name, optiondropdown.value);
        }
    }

    public void ValueStringOptionChangeCheck()
    {
        if (GameSetting.instance != null)
        {
            GameSetting.instance.configuration.SetPropValue(gameObject.name, optiondropdown.options[optiondropdown.value].text);
        }
    }

    public void SetUpLimits(int v1, int v2)
    {
        if (slider != null)
        {
            slider.minValue = v1;
            slider.value = v1;
            valueDisplayer.text = v1.ToString();
            slider.maxValue = v2;
        }
    }
}