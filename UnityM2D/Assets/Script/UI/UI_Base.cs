using UnityEditor.Analytics;
using UnityEngine;
using static Defines;

public class UI_Base : Base
{
    #region Field
    private UIType _uiType;
    
    protected float LastProjectCoolTime = 30f;
    protected float LastProjectTime = 0.0f;

    protected GameObject _parentObject;
    private RectTransform uiRectTransform;

    protected bool _bUpdate = false;
    #endregion

    private void Start() =>Init();
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    // 어떠한 객체에 UI를 붙일 것인가? 말 것인가?
    public void SetInfo(GameObject _parent, bool _update) 
    {
        _parentObject = _parent;
        _bUpdate = _update;
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
