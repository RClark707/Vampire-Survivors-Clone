using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Obsolete("This has been replaced with UILevelSelect.")]
public class SceneController : MonoBehaviour
{
    public void SceneChange(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1f;
    }
}
