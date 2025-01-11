using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    public GameObject youWinPopUp;
    public GameObject youLosePopUp;

    // Start is called before the first frame update
    void Start()
    {
        youLosePopUp.SetActive(false);
        youWinPopUp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowYouWinPopUp()
    {
        Time.timeScale = 0;
        youWinPopUp.SetActive(true);
    }
    public void ShowLosePopUp() 
    {
        Time.timeScale = 0;
        youLosePopUp.SetActive(true);
    }


    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
