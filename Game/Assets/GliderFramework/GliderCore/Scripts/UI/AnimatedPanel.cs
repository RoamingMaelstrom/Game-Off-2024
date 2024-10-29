using System.Collections;
using UnityEngine;

public class AnimatedPanel : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeOpenDuration = 0.2f;
    [SerializeField] float fadeCloseDuration = 0.2f;
    [SerializeField] float targetScale = 1f;
    [SerializeField] bool startClosed = true;

    Coroutine animationCoroutine;


    private void Awake() {
        if (startClosed) SetFullyClosed();
        else SetFullyOpen();
    }


    public void Close() {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(Animation(fadeCloseDuration, false));
    }

    public void Open() {
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(Animation(fadeOpenDuration, true));
    }

    public void Toggle() {
        if (canvasGroup.alpha == 1) Open();
        else Close();
    }


    private void SetFullyClosed() {
        canvasGroup.transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }

    private void SetFullyOpen() {
        canvasGroup.transform.localScale = Vector3.one * targetScale;
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
    }


    private IEnumerator Animation(float fadeDuration, bool openDirection = true)
    {
        float fixedDelta = 0.01f;
        float rateOfFade = fixedDelta / fadeDuration;
        float rateOfSizeChange = fixedDelta * targetScale / fadeDuration;

        rateOfFade *= openDirection ? 1 : -1;
        rateOfSizeChange *= openDirection ? 1 : -1;

        if (!openDirection) canvasGroup.interactable = false;

        while (canvasGroup.alpha * (openDirection ? 1: -1) < (openDirection ? 1 : 0))
        {
            canvasGroup.alpha += rateOfFade;
            canvasGroup.transform.localScale = canvasGroup.transform.localScale + (rateOfSizeChange * Vector3.one);

            yield return new WaitForSecondsRealtime(fixedDelta);
        }

        if (openDirection) SetFullyOpen();
        else SetFullyClosed();
    }
}
