using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.Diagnostics;

public class UI_Manager : MonoBehaviour
{
    int _order = 20;

    Stack<UI_Base> uiStack = new Stack<UI_Base>();

    public T ShowUI<T>(string _name = null, Transform parent = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(_name))
            _name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/{_name}");

        T settingUI = Setting.GetOrAddComponent<T>(go);

        if (parent != null)
            go.transform.SetParent(parent);

        if (settingUI != null)
            uiStack.Push(settingUI);

        return settingUI;   
    }

    public T FindUI<T>() where T : UI_Base
    {
        return uiStack.Where(t => t != null && t.GetType() == typeof(T)).FirstOrDefault() as T;
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Setting.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public void ClosePopupUI(UI_Base ui)
    {
        if (uiStack.Count > 0)
            return;

        if(uiStack.Peek() != ui)
        {
            System.Console.WriteLine($"Failed Close : {ui.ToString()}");
            return;
        }

        CloseUI();
    }

    public void CloseUI()
    {
        if (uiStack.Count == 0)
            return;

        UI_Base popup = uiStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _order--;
    }

    public void ClaseAllUI()
    {
        while (uiStack.Count > 0) 
            CloseUI();
    }
}
