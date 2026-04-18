using UnityEngine;
using TMPro;
using System.Collections;
using System;

public class CountDown : MonoBehaviour
{
    [SerializeField]private int startSize=72;
    [SerializeField]private int endSize=36;
    [SerializeField]private TextMeshProUGUI text;
    private float timePerNumber = 1f;

    public void StartCountdown(int startNum = 10, Action onFinished = null)
    {
        Debug.Log("Starting Countdown");
        StartCoroutine(CountdownRoutine(startNum, onFinished));
    }

    IEnumerator CountdownRoutine(int startNum, Action onFinished)
    {
        for (int i = startNum; i >= 1; i--) 
        { 
            yield return AnimateNumber(i);
        }

        onFinished?.Invoke();
    }

    IEnumerator AnimateNumber(int number)
    {
        float time = 0f;

        text.text = number.ToString();

        while (time < timePerNumber)
        {
            time += Time.deltaTime;
            float t = time / timePerNumber;

            text.fontSize = Mathf.Lerp(startSize, endSize, t);

            yield return null;
        }

        text.fontSize = endSize;
    }
}
