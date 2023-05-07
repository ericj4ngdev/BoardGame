using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private Image Penal;
    [SerializeField]
    private string loadSceneName;
    [SerializeField]
    private BinaryInfo binaryInfo;
    
    public float Ratio;
    public GameObject MainImage;
    public GameObject NextImage;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print("OnSceneLoaded");
        if (scene.name == "ProtoType3D")
        {
            print("ProtoType3D");
            // 찾기
            binaryInfo = FindObjectOfType<BinaryInfo>();
            if (binaryInfo == null)
            {
                Debug.LogError("BinaryInfo component not found in scene.");
            }
            else
            {
                // 대입
                binaryInfo.ratio = Ratio;
                Debug.Log("Ratio value set to BinaryInfo.ratio");
            }
        }
    }

    public void ActiveImage()
    {
        NextImage.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SelectDiff(float ratio)
    {
        Ratio = ratio;
    }

    // 버튼 이벤트에 넣기
    public void LoadScene(string sceneName)
    {
        gameObject.SetActive(true);
        loadSceneName = sceneName;
        StartCoroutine(Load(loadSceneName));
    }
    
    private IEnumerator Load(string loadsceneName)
    {
        yield return StartCoroutine(FadeIn());
        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName);
        op.allowSceneActivation = true;
    }

    private IEnumerator FadeIn()
    {
        Penal.gameObject.SetActive(true);
        Color alpha = Penal.color;

        float timer = 0f;
        while (alpha.a < 1f)
        {
            timer += Time.deltaTime;
            alpha.a = Mathf.Lerp(0f, 1f, timer);
            yield return null;
        }
        yield return null;
    }
}