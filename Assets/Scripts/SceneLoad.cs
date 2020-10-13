using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    [SerializeField] string m_loadSceneName;

    public void LoadScene()
    {
        SceneManager.LoadScene(m_loadSceneName);
    }

    public void LoadSceneDelay(float delay)
    {
        StartCoroutine(Delay(delay, m_loadSceneName));
    }

    private IEnumerator Delay(float delay,string sceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
