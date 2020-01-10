using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertTextUI : MonoBehaviour
{
    [SerializeField] protected Text alertText;
    [SerializeField] protected float travelUpSpeed;
    [SerializeField] protected float duration;
    [SerializeField] protected Color[] startStopLerpColors;

    private void Awake()
    {
        alertText = GetComponent<Text>();
    }


    public void DisplayAlert(string value)
    {
        alertText.text = value;
        StartCoroutine(PlayDamageSequence());
    }

    IEnumerator PlayDamageSequence()
    {
        float durationLeft = duration;
        while(durationLeft != 0f)
        {
            transform.position += new Vector3(0f, travelUpSpeed * Time.deltaTime, 0f);
            alertText.color = Color.Lerp(startStopLerpColors[0], startStopLerpColors[1], 1f - (durationLeft / duration));
            durationLeft = Mathf.Clamp(durationLeft - Time.deltaTime, 0f, duration);
            yield return null;
        }
        StopCoroutine();
    }

    public void StopCoroutine()
    {
        StopAllCoroutines();
        alertText.text = "";
        gameObject.SetActive(false);
    }


}
