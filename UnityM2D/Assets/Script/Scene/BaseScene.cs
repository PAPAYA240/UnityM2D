using UnityEngine;

public class BaseScene : MonoBehaviour
{
    public Defines.Scene sceneType = Defines.Scene.None;

    protected bool _start = false;
    void Start()
    {
        Init();
    }

    protected virtual bool Init()
    {
        if (_start)
            return false;

        _start = true;
        GameObject go = GameObject.Find("EventSystem");

      //  if (go == null)
       //     Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";

        return true;
    }
    public virtual void Clear() { }
}
