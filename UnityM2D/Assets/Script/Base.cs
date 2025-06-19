using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Base : MonoBehaviour
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

    /// <summary>
    /// enum에 정의된 이름을 기준으로 현재 게임 오브젝트의 자식에서 원하는 타입의 컴포넌트를 찾아 바인딩합니다.
    /// </summary>
    protected void BindByEnum<T>(Type enumType) where T : UnityEngine.Object
    {
        // enum의 모든 값을 가져옵니다.
        Array enumValues = Enum.GetValues(enumType);

        foreach (Enum key in enumValues)
        {
            string name = key.ToString();
            T obj = null;

            // 타입에 따라 GameObject와 그 외 컴포넌트 검색 분기
             if (typeof(T) == typeof(GameObject))
                obj = Setting.FindChild(gameObject, name, true) as T;
            else
                obj = Setting.FindChild<T>(gameObject, name, true);

            if (obj == null)
            {
                Debug.Log($"[BindByEnum] Failed BindByEnum :  {name}");
            }
            else
            {
                _uiObjects[key] = obj;
            }
        }
    }

    /// <summary>
    /// 바인딩된 UI 오브젝트를 enum 키를 통해 가져옵니다.
    /// </summary>
    protected T GetByEnum<T>(Enum key) where T : UnityEngine.Object
    {
        if (_uiObjects.TryGetValue(key, out UnityEngine.Object obj))
            return obj as T;

        return null;
    }


    protected GameObject GetObject(Enum _enum) { return GetByEnum<GameObject>(_enum); }
    protected TextMeshProUGUI GetText(Enum _enum) { return GetByEnum<TextMeshProUGUI>(_enum); }
    protected Button GetButton(Enum _enum) { return GetByEnum<Button>(_enum); }
    protected Image GetImage(Enum _enum) { return GetByEnum<Image>(_enum); }

    protected List<GameObject> FindChild(GameObject _parent, string targetChildName)
    {
        List<GameObject> foundChildren = new List<GameObject>();

        foreach (Transform childTransform in _parent.transform)
        {
            if (childTransform.name == targetChildName)
            {
                foundChildren.Add(childTransform.gameObject); // GameObject로 변환하여 리스트에 추가
            }
        }

        return foundChildren;
    }

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