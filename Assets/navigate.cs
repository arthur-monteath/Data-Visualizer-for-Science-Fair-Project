using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class navigate : MonoBehaviour
{
    public void Navigate(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
