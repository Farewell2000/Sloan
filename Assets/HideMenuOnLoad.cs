using UnityEngine;
using UnityEngine.SceneManagement;

public class HideMenuOnLoad : MonoBehaviour
{
    public static HideMenuOnLoad Instance;

    void Awake()
    {
        Instance = this;
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
    }
}