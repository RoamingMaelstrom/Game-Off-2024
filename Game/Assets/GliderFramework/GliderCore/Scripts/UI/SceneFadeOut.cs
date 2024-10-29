using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeOut : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float delayBeforeSceneLoad = 1f;
    [SerializeField] int loadSceneIndexOnEnd = -1;


    private void Awake() {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }


    public void FadeOutAndLoadSceneIfSet() => StartCoroutine(FadeOut(fadeDuration));


    private IEnumerator FadeOut(float fadeDuration)
    {
        float rateOfFade = 0.01f / fadeDuration;

        canvasGroup.blocksRaycasts = true;

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += rateOfFade;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        canvasGroup.alpha = 1;

        if (loadSceneIndexOnEnd >= 0){
            yield return new WaitForSecondsRealtime(delayBeforeSceneLoad);
            SceneManager.LoadScene(loadSceneIndexOnEnd);
        }    
    }
}
