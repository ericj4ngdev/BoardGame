using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;
    private string loadSceneName;
    private float time = 0;

    // 버튼 이벤트에 넣기
    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        
        // StartCoroutine(FadeIn());
        loadSceneName = sceneName;
        StartCoroutine(Load(loadSceneName));
        // StartCoroutine(FadeOut());
        // SceneManager.sceneLoaded += OnSceneLoaded;      // 콜백
        // StartCoroutine(LoadSceneProcess());
    }

    private IEnumerator Load(string loadsceneName)
    {
        yield return StartCoroutine(FadeIn());
        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = true;
        // yield return StartCoroutine(FadeOut());
        // yield return null;
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        while (canvasGroup.alpha < 1f)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer);
            yield return null;
        }
        yield return null;
    }
    private IEnumerator FadeOut()
    {
        float timer = 0f;
        while (canvasGroup.alpha > 0f)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer);
            yield return null;
        }
        yield return null;
    }
    /*private IEnumerator LoadSceneProcess()
    {
        // yield return StartCoroutine(Fade(true));
        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = false;
        
        while (!op.isDone)
        {
            yield return null;
        }
        op.allowSceneActivation = true;
        yield return StartCoroutine(Fade(false)); // Fade Out
        
        // 불러오기가 끝나면 자동으로 다음 씬으로 이동할건가?
        // false로 하면 90%까지만 로드하고 true로 바뀌면 마저 로드한다. 
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == loadSceneName)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    
    IEnumerator Fade(bool isFadeIn)
    {
        Panel.gameObject.SetActive(true);
        Color alpha = Panel.color;
        float timer = 0f;
        while (alpha.a <= 1f)
        {
            yield return null;
            timer += Time.deltaTime * 3f;
            alpha.a = isFadeIn ? Mathf.Lerp(0f, 1f, timer) : Mathf.Lerp(1f, 0f, timer);
        }
        if(!isFadeIn)
            gameObject.SetActive(false);
    }*/
}
