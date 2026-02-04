using GoogleMobileAds.Api;
using System.Collections;
using TMPro;
using UnityEngine;
using static Defines;

public class UI_LevelUp : UI_Base
{
    enum TextType
    {
        StateText,
    }


    private PlayerController Player = null;
    [SerializeField] Animator _animator;
    private Camera _mainCamera = null; // 카메라 캐싱용

    void Start() => Init();

    public override bool Init()
    {
        if (base.Init() == false) return false; // base.Init 실패 시 중단

        BindText();

        // strPlayerObject가 정의되어 있다고 가정
        GameObject PlayerObject = GameObject.Find(strPlayerObject);
        if (PlayerObject != null)
            Player = PlayerObject.GetComponent<PlayerController>();

        _animator = GetComponentInChildren<Animator>();
        _mainCamera = Camera.main; // 카메라 미리 찾아놓기

        return true;
    }
    Vector3 offset = new Vector3(0, 1.0f,0);
    void Update()
    {
        GetText(TextType.StateText).transform.position =
            Camera.main.WorldToScreenPoint(_parentObject.transform.position + offset);
    }
    public void Invoke(float duration)
    {
        gameObject.SetActive(true);

        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        // 코루틴 시작
        StartCoroutine(Active_LevelUI(duration));
    }

    private IEnumerator Active_LevelUI(float duration)
    {
       yield return new WaitForSeconds(duration); 

        gameObject.SetActive(false);
    }

    private bool BindText()
    {
        BindText(typeof(TextType));
        return true;
    }
}