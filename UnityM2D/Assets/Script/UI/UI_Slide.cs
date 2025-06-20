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
        ManaBar, // 필요하다면 추가
    }

    enum Texts
    {
        Level_Text,
    }

    private SlideTargetType slideType = SlideTargetType.HpBar; // 인스펙터에서 설정

    // Lerp 애니메이션에 사용할 변수
    private float _currentDisplayedValue; // 현재 슬라이더에 표시되고 있는 값
    private float _targetValue;           // 슬라이더가 도달해야 할 목표 값
    private float _animationTimer;        // 애니메이션 타이머
    private float _animationDuration = 0.5f; // 애니메이션 지속 시간 (조정 가능)

    private Coroutine _currentSlideAnimationCoroutine; // 중복 애니메이션 방지

    // 이 슬라이더가 추적할 BaseController 인스턴스를 직접 참조하도록 변경
    // SetInfo를 통해 외부에서 주입받는 방식이 가장 깔끔합니다.
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
        if (_currentSlideAnimationCoroutine != null) StopCoroutine(_currentSlideAnimationCoroutine);
        _currentSlideAnimationCoroutine = StartCoroutine(AnimateSliderValue());

        return true;
    }

    public void RegisterInfo(SlideTargetType _slideType)
    {
        slideType = _slideType;
    }
    public void SetTarget(GameObject _parent, BaseController baseCon, float animationDuration = 0.5f)
    {
        base.SetInfo(_parent, true); // UI_Base의 SetInfo 호출 (bUpdate는 여기서 true로 설정)
        _targetBaseController = baseCon;
        _animationDuration = animationDuration;

        if (slider != null && _targetBaseController != null && _targetBaseController.State != null)
        {
            switch (slideType)
            {
                case SlideTargetType.HpBar:
                    slider.maxValue = _targetBaseController.State.MaxHp;
                    break;
                case SlideTargetType.ExpBar:
                    slider.maxValue = _targetBaseController.State.LevelCount;
                    break;
                    // 다른 타입 추가
            }
            // 초기 슬라이더 값 설정
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

    // 슬라이더의 현재 목표 값을 가져오는 헬퍼 함수
    private float GetCurrentTargetValue()
    {
        if (_targetBaseController == null || _targetBaseController.State == null)
                return _currentDisplayedValue;

        switch (slideType)
        {
            case SlideTargetType.HpBar:
                return _targetBaseController.State.Hp;
            case SlideTargetType.ExpBar:
                UpdateLevel();
                return _targetBaseController.State.LevelCount;
            default:
                return _currentDisplayedValue;
        }
    }

    // 슬라이더 값을 부드럽게 업데이트하는 코루틴
    private IEnumerator AnimateSliderValue()
    {
        while (true) // 게임이 활성화되어 있는 동안 계속 실행
        {
            if (slider == null || _targetBaseController == null || _targetBaseController.State == null)
            {
                if (_targetBaseController == null || _targetBaseController.State == null)
                    if ((_targetBaseController = GameObject.Find("Player").GetComponent<BaseController>()) == null)
                    { 
                        yield return new WaitForSeconds(0.1f); // 잠시 기다렸다가 다시 시도
                        continue;
                    }
            }

            float actualCurrentValue = GetCurrentTargetValue(); // BaseController에서 실제 현재 값

            // 실제 값이 현재 표시값과 다르면 애니메이션 시작
            if (Mathf.Abs(_currentDisplayedValue - actualCurrentValue) > 0.01f) // 부동 소수점 오차 감안
            {
                // 목표 값이 변경되었을 때 애니메이션 타이머 리셋 및 시작 값 갱신
                if (_targetValue != actualCurrentValue)
                {
                    _animationTimer = 0f;
                    _targetValue = actualCurrentValue; // 새로운 목표 값 설정
                }

                _animationTimer += Time.deltaTime;
                float t = _animationTimer / _animationDuration;
                t = Mathf.Clamp01(t);

                _currentDisplayedValue = Mathf.Lerp(_currentDisplayedValue, _targetValue, t);
                slider.value = _currentDisplayedValue; // 슬라이더에 현재 표시 값을 할당

                // 애니메이션이 거의 끝나면 정확히 목표 값으로 설정하여 정밀도 문제 방지
                if (t >= 1.0f - float.Epsilon) // t가 거의 1에 도달했을 때
                {
                    _currentDisplayedValue = _targetValue;
                    slider.value = _currentDisplayedValue;
                    _animationTimer = 0f; // 타이머 리셋
                }
            }
            else // 실제 값과 표시 값이 같으면 애니메이션 중지 (정확한 값으로 설정)
            {
                _currentDisplayedValue = actualCurrentValue;
                slider.value = _currentDisplayedValue;
                _animationTimer = 0f; // 타이머 리셋
            }

            yield return null; // 다음 프레임까지 대기
        }
    }

    void UpdateLevel()
    {
        if (slideType != SlideTargetType.ExpBar)
            return;

        GetText(Texts.Level_Text).text = string.Format($"LEVEL: {_targetBaseController.State.Level}");
    }    
}