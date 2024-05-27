using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{
    private AsyncOperation operation;
    private static bool shouldPlayOpeningAnimation = false;

    public Image loadingImg;
    public Text progressText;

    public static SceneLoading instance = null;
    private Animator animator;
    private float chekProgress;
    private void Start()
    {
        animator = GetComponent<Animator>();
        if (instance == null) instance = this;
        if (shouldPlayOpeningAnimation) animator.SetTrigger("Open");
    }

    public static void LoadScene(int level)
    {
        instance.animator.SetTrigger("Load");
        instance.operation = SceneManager.LoadSceneAsync(level);
        instance.operation.allowSceneActivation = false;
    }

    private void Update()
    {
        if (operation != null)
        {
            float progress = operation.progress / 0.9f;
            loadingImg.fillAmount = progress;
            progressText.text = string.Format("{0:0}", progress * 100f);
        }
    }

    public void OnAnimatorOver()
    {
        shouldPlayOpeningAnimation = true;
        Invoke("OpenScene", 0.5f);
    }

    public void OpenScene()
    {
        operation.allowSceneActivation = true;
    }

}
