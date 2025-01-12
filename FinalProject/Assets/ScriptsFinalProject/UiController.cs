using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiController : MonoBehaviour
{
    public GameObject youWinPopUp;
    public GameObject youLosePopUp;
    public TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    void Start()
    {
        youLosePopUp.SetActive(false);
        youWinPopUp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Zombie Points: " + GameManager.Instance.currentScore + "/" + GameManager.Instance.maxScore;
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
        Application.Quit();
    }
}
