using UnityEditor.Analytics;
using UnityEngine;
using static Defines;

public class UI_Base : Base
{
    protected float LastProjectCoolTime = 30f;
    protected float LastProjectTime = 0.0f;

    UIType uiType;

    protected bool bUpdate = false;
    protected GameObject ParentObject = null;

    public Vector3 offset = new Vector3(0, -0.14f, 0); 

    private RectTransform uiRectTransform;
    private void Start()
    {
        Init();
    }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    private void LateUpdate()
    {
      
    }

    // 어떠한 객체에 UI를 붙일 것인가? 말 것인가?
    public void SetInfo(GameObject _parent, bool _update) 
    {
        ParentObject = _parent;
        bUpdate = _update;
    }

    protected float GetProjectWaitRatio()
    {
        float playTime = Managers.PlayTime;
        float projectTime = LastProjectTime;

        float ratio = 1.0f;
        if (projectTime > 0 && projectTime < playTime)
            ratio = (playTime - projectTime) / LastProjectCoolTime;

        return ratio;
    }
}
