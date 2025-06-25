using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

public class UI_Slide : UI_Base // UI_Base를 상속받음
{
    private Slider slider;

    public enum SlideTargetType // 어떤 값을 추적할지 명확히 정의
    {
        None,
        HpBar,
        ExpBar,
        ManaBar, 
    }

    enum Texts
    {
        Level_Text,
    }

    private SlideTargetType slideType = SlideTargetType.HpBar; 

    private float _currentDisplayedValue; 
    private float _targetValue;  
    private float _animationTimer;     
    private float _animationDuration = 0.5f;

    private Coroutine _currentSlideAnimationCoroutine; 
    private BaseController _targetBaseController;

    void Start()
    {
        Init();
    }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText(typeof(Texts));

        // 슬라이더 컴포넌트 찾기
        slider = GetComponentInChildren<Slider>(true);
        if (slider == null)
        {
            Debug.LogError($"UI_Slide: {gameObject.name}에 Slider 컴포넌트가 없습니다!", this);
            return false;
        }

        _currentDisplayedValue = slider.value;
        _targetValue = slider.value;

        if (ParentObject != null && ParentObject.transform.parent != null)
        {
            _targetBaseController = ParentObject.transform.parent.gameObject.GetComponent<BaseController>();
            if (_targetBaseController == null)
            {
                Debug.LogWarning($"UI_Slide: {ParentObject.transform.parent.name}에서 BaseController를 찾을 수 없습니다.");
            }
        }
        SlideTargetType a = slideType;
        if (_currentSlideAnimationCoroutine != null) 
            StopCoroutine(_currentSlideAnimationCoroutine);
        _currentSlideAnimationCoroutine = StartCoroutine(AnimateSliderValue());

        return true;
    }

    public void RegisterInfo(SlideTargetType _slideType)
    {
        slideType = _slideType;
    }
    public void SetTarget(GameObject _parent, BaseController baseCon, float animationDuration = 0.5f)
    {
        base.SetInfo(_parent, true); 
        _targetBaseController = baseCon;
        _animationDuration = animationDuration;

        if (slider != null && _targetBaseController != null && _targetBaseController.data != null)
        {
            switch (slideType)
            {
                case SlideTargetType.HpBar:
                    slider.maxValue = _targetBaseController.data.MaxHp;
                    break;
                case SlideTargetType.ExpBar:
                    slider.maxValue = _targetBaseController.data.LevelCount;
                    break;
         
            }

            _currentDisplayedValue = GetCurrentTargetValue();
            _targetValue = _currentDisplayedValue;
            slider.value = _currentDisplayedValue;
        }
    }


    void LateUpdate()
    {
        if (bUpdate && ParentObject != null)
        {
            slider.transform.position = Camera.main.WorldToScreenPoint(ParentObject.transform.position + offset);
        }
    }

    private float GetCurrentTargetValue()
    {
        if (_targetBaseController == null || _targetBaseController.data == null)
                return _currentDisplayedValue;

        switch (slideType)
        {
            case SlideTargetType.HpBar:
                slider.maxValue = _targetBaseController.data.MaxHp;
                return _targetBaseController.data.Hp;
            case SlideTargetType.ExpBar:
                UpdateLevel();
                return _targetBaseController.data.LevelCount;
            default:
                return _currentDisplayedValue;
        }
    }

    private IEnumerator AnimateSliderValue()
    {
        while (true)
        {
            if (slider == null || _targetBaseController == null || _targetBaseController.data == null)
            {
                if (_targetBaseController == null || _targetBaseController.data == null)
                    if ((_targetBaseController = GameObject.Find(Defines.strPlayerObject).GetComponent<BaseController>()) == null)
                    { 
                        yield return new WaitForSeconds(0.1f); 
                        continue;
                    }
            }

            float actualCurrentValue = GetCurrentTargetValue(); 

            if (Mathf.Abs(_currentDisplayedValue - actualCurrentValue) > 0.01f) 
            {
                if (_targetValue != actualCurrentValue)
                {
                    _animationTimer = 0f;
                    _targetValue = actualCurrentValue; 
                }

                _animationTimer += Time.deltaTime;
                float t = _animationTimer / _animationDuration;
                t = Mathf.Clamp01(t);

                _currentDisplayedValue = Mathf.Lerp(_currentDisplayedValue, _targetValue, t);
                slider.value = _currentDisplayedValue; 

                if (t >= 1.0f - float.Epsilon) 
                {
                    _currentDisplayedValue = _targetValue;
                    slider.value = _currentDisplayedValue;
                    _animationTimer = 0f; 
                }
            }
            else 
            {
                _currentDisplayedValue = actualCurrentValue;
                slider.value = _currentDisplayedValue;
                _animationTimer = 0f; 
            }
            yield return null;
        }
    }

    void UpdateLevel()
    {
        if (slideType != SlideTargetType.ExpBar)
            return;

        GetText(Texts.Level_Text).text = string.Format($"LEVEL: {_targetBaseController.data.Level}");
    }    
}