using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public abstract class Base : MonoBehaviour
{
    // Enum 키로 dictionary 정의
    protected Dictionary<Enum, UnityEngine.Object> _uiObjects = new Dictionary<Enum, UnityEngine.Object>();
    protected bool _init = false;

    protected void BindObject(Type type) { BindByEnum<GameObject>(type); }
    protected void BindImage(Type type) { BindByEnum<Image>(type); }
    protected void BindText(Type type) { BindByEnum<TextMeshProUGUI>(type); }
    protected void BindButton(Type type) { BindByEnum<Button>(type); }

    void Start()
    {
        Init();
    }

    public virtual bool Init()
    {
        if (_init)
            return false;

        return _init = true;
    }

    private void BindByEnum<T>(Type enumType) where T : UnityEngine.Object
    {
        Array enumValues = Enum.GetValues(enumType);

        foreach (Enum key in enumValues)
        {
            string name = key.ToString();
            T obj = null;

            if (typeof(T) == typeof(GameObject))
                obj = Setting.FindChild(gameObject, name, true) as T;
            else
                obj = Setting.FindChild<T>(gameObject, name, true);

            if (obj == null)
            {
                return;
            }
            else
            {
                _uiObjects[key] = obj;
            }
        }
    }

    private T GetByEnum<T>(Enum key) where T : UnityEngine.Object
    {
        if (_uiObjects.TryGetValue(key, out UnityEngine.Object obj))
            return obj as T;

        return null;
    }

    protected GameObject GetObject(Enum _enum) { return GetByEnum<GameObject>(_enum); }
    protected TextMeshProUGUI GetText(Enum _enum) { return GetByEnum<TextMeshProUGUI>(_enum); }
    protected Button GetButton(Enum _enum) { return GetByEnum<Button>(_enum); }
    protected Image GetImage(Enum _enum) { return GetByEnum<Image>(_enum); }

    public static void BindEvent(GameObject go, Action action, Defines.Input type = Defines.Input.Click)
    {
        Input_Manager _event = Setting.GetOrAddComponent<Input_Manager>(go);

        switch (type)
        {
            case Defines.Input.Click:
                _event.OnClickHandler -= action;
                _event.OnClickHandler += action;
                break;
            case Defines.Input.Pressed:
                _event.OnPressedHandler -= action;
                _event.OnPressedHandler += action;
                break;
            case Defines.Input.PointerDown:
                _event.OnPointerDownHandler -= action;
                _event.OnPointerDownHandler += action;
                break;
            case Defines.Input.PointerUp:
                _event.OnPointerUpHandler -= action;
                _event.OnPointerUpHandler += action;
                break;
        }
    }
  
}