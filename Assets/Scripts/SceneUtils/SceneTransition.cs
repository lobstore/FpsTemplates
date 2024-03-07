using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneTransition : MonoBehaviour
{
    public TextMeshProUGUI loadingPercentage;
    public Image loadingProgressBar;
    private static bool shouldPlayOperationAnimation = false;
    private Animator componentAnimator;
    private AsyncOperation loadSceneOperation;
    public void SwitchToScene(string sceneName)
    {
        componentAnimator.SetTrigger("Start");
        loadSceneOperation = SceneManager.LoadSceneAsync(sceneName);
        loadSceneOperation.allowSceneActivation = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        componentAnimator = GetComponent<Animator>();
        if (shouldPlayOperationAnimation) componentAnimator.SetTrigger("End");
    }

    private void Update()
    {
        if (loadSceneOperation != null)
        {
            loadingPercentage.text = loadSceneOperation.progress * 100 + "%";
            loadingProgressBar.fillAmount = Mathf.Lerp(loadSceneOperation.progress, loadSceneOperation.progress, Time.deltaTime * 5);
        }
    }

    public void OnAnimationOver()
    {
        shouldPlayOperationAnimation = true;
        loadSceneOperation.allowSceneActivation = true;
    }
}
