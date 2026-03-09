
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Enter : MonoBehaviour
{
    public UniversalRendererData urpAsset;
    public GameObject mainscreen;
    public GameObject spalsh;
    public GameObject onbording1;
    public GameObject onbording2;
    public GameObject onbording3;
    private void Start()
    {
        mainscreen.SetActive(false);
        onbording1.SetActive(false);
        onbording2.SetActive(false);
        onbording3.SetActive(false);
        spalsh.SetActive(true);

        if (urpAsset != null)
        {
            var rendererFeatures = urpAsset.rendererFeatures;
            var feature = rendererFeatures.Find(f => f is Underwater) as Underwater;
            if (feature != null)
            {
                feature.SetActive(false);
            }
        }
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
    public void ChangeSceneReef()
    {
        SceneManager.LoadScene(2);
    }
    public void openmain()
    {
        mainscreen.SetActive(true);
        spalsh.SetActive(false);
        onbording1.SetActive(false);
        onbording2.SetActive(false);
        onbording3.SetActive(false);
    }
    public void opensplash()
    {
        mainscreen.SetActive(false);
        spalsh.SetActive(true);
        onbording1.SetActive(false);
        onbording2.SetActive(false);
        onbording3.SetActive(false);
    }
    public void guidutilis()
    {
        mainscreen.SetActive(false);
        spalsh.SetActive(false);
        onbording1.SetActive(true);
        onbording2.SetActive(false);
        onbording3.SetActive(false);
    }
    public void continue1()
    {
        mainscreen.SetActive(false);
        spalsh.SetActive(false);
        onbording1.SetActive(false);
        onbording2.SetActive(true);
        onbording3.SetActive(false);
    }
    public void continue2()
    {
        mainscreen.SetActive(false);
        spalsh.SetActive(false);
        onbording1.SetActive(false);
        onbording2.SetActive(false);
        onbording3.SetActive(true);
    }

    public void ChangeSceneSpace()
    {
        SceneManager.LoadScene(3);
    }
    public void ChangeSceneCarpete()
    {
        SceneManager.LoadScene(4);
    }
    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
