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


    private Vector3 _origionPosition = new Vector3();
    PlayerController Player = null;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindText();
        GameObject PlayerObject = GameObject.Find(strPlayerObject);
        Player = PlayerObject.GetComponent<PlayerController>();
        _origionPosition = transform.position;
        return true;
    }
    public void Invoke(float duration)
    {
        gameObject.SetActive(true);
         StartCoroutine(Active_LevelUI(duration));
    }
    private IEnumerator Active_LevelUI(float duration)
    {
        if (Player == null)
        {
            GameObject PlayerObject = GameObject.Find(strPlayerObject);
            Player = PlayerObject.GetComponent<PlayerController>();
            if(Player ==null)
                yield return null;
        }
        gameObject.SetActive(true);

        float elapsedTime = 0f;
        Vector3 startPosition = _origionPosition;
        Vector3 endPosition = _origionPosition + new Vector3(0.0f, 250f, 0.0f);
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        gameObject.SetActive(false);
        transform.position = _origionPosition;
    }
    private bool BindText()
    {
        BindText(typeof(TextType));
        return true;
    }
}
