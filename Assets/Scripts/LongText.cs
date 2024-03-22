using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;

public class LongText : MonoBehaviour
{
    [SerializeField] private GameObject GameOverWindow;
    [SerializeField] private GameObject InputField;
    [SerializeField] private GameObject Keyboard;

    [SerializeField] private Slider slider;

    [SerializeField] private TMP_Text YourAnswer;
    [SerializeField] private TMP_Text RightAnswer;
    [SerializeField] private TMP_Text GeneratedSymbols;
    private TMP_Text gameOver;

    [SerializeField] private GameObject ShowSymbols;
    [SerializeField] private GameObject AnswerSymbols;

    private float fillTime = 1f;

    private int maxValue = 1;
    private int currentRank = 1;
    
    private string allSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private string pickedSymbols;

    private int totalGamesPlayedLT;
    private int maxLevelReachedLT;
    private const string TotalGamesLongText = "GPC2";
    private const string MaxLevelLongText = "MLC2";


    void Start()
    {
        Time.timeScale = 1f;
        slider.maxValue = maxValue;

        totalGamesPlayedLT = PlayerPrefs.GetInt(TotalGamesLongText);
        totalGamesPlayedLT++;
        PlayerPrefs.SetInt(TotalGamesLongText, totalGamesPlayedLT);
        UpdateStat();

        RoundStart();
    }

    private void UpdateStat()
    {
        totalGamesPlayedLT = PlayerPrefs.GetInt(TotalGamesLongText);
        maxLevelReachedLT = PlayerPrefs.GetInt(MaxLevelLongText);

        if (currentRank > maxLevelReachedLT)
            maxLevelReachedLT = currentRank;

        PlayerPrefs.SetInt(TotalGamesLongText, totalGamesPlayedLT);
        PlayerPrefs.SetInt(MaxLevelLongText, maxLevelReachedLT);
    }

    public void StartGame()
    {
        GameOverWindow.SetActive(false);
        AnswerSymbols.SetActive(false);
        ShowSymbols.SetActive(true);

        totalGamesPlayedLT = PlayerPrefs.GetInt(TotalGamesLongText);
        totalGamesPlayedLT++;
        PlayerPrefs.SetInt(TotalGamesLongText, totalGamesPlayedLT);
        UpdateStat();

        currentRank = 1; fillTime = 1f; maxValue = 1;

        Time.timeScale = 1f;
        slider.maxValue = maxValue;

        InputField.GetComponent<TMP_InputField>().text = "";

        RoundStart();
    }

    private void RoundStart()
    {
        pickedSymbols = "";

        for (int i = 0; i < currentRank; i++)
        {
            int index = Random.Range(0, allSymbols.Length);
            pickedSymbols += allSymbols[index];
        }

        GeneratedSymbols.text = pickedSymbols;
        Keyboard.SetActive(false);

        StartCoroutine(FillValue());
    }

    private IEnumerator FillValue()
    {
        var estimateTime = fillTime;

        while (estimateTime > 0) //Slider smooth animation
        {
            estimateTime -= Time.deltaTime;
            slider.value = Mathf.Lerp(0, fillTime, estimateTime / fillTime);
            yield return null;
        }

        ShowSymbols.SetActive(false);
        AnswerSymbols.SetActive(true);
        Keyboard.SetActive(true);

        TMP_InputField inputField = InputField.GetComponent<TMP_InputField>();
        inputField.Select();
        inputField.ActivateInputField();
    }

    public void SubmitClicked()
    {
        string submitted = InputField.GetComponent<TMP_InputField>().text;
        
        if (submitted == pickedSymbols)
        {
            if (currentRank % 3 == 0 && maxValue < 8) //Raise max value of slider
            {
                maxValue++;
                slider.maxValue = maxValue;
                fillTime++;
            }

            currentRank++;

            InputField.GetComponent<TMP_InputField>().text = "";

            AnswerSymbols.SetActive(false);
            ShowSymbols.SetActive(true);

            RoundStart();
        }
        else 
            GameOver(submitted);
    }

    private void GameOver(string submitted)
    {
        YourAnswer.text = submitted;
        RightAnswer.text = pickedSymbols;

        GameOverWindow.SetActive(true);
        Time.timeScale = 0f;

        gameOver = GameObject.Find("ResultText").GetComponent<TMP_Text>();
        gameOver.text = $"Reached level {currentRank}\r\nDo you want to play again?";
    }

    public void BackMenuClicked()
    {
        UpdateStat();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }
}
