using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObject : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 5f;

    private CanvasGroup canvasGroup;

    /// <summary>
    /// UI Objects need Canvas Group to change their transparency.
    /// </summary>    

    public IEnumerator HandleFadeEffect(GameObject obj, float startAlpha, float targetAlpha)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();

        canvasGroup.alpha = startAlpha;

        while (!Mathf.Approximately(canvasGroup.alpha, targetAlpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
    }
}