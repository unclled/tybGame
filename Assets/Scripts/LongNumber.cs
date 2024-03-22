using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;

public class LongNumber : MonoBehaviour
{
    [SerializeField] private GameObject GameOverWindow;
    [SerializeField] private GameObject InputField;
    [SerializeField] private GameObject Keyboard;

    [SerializeField] private Slider slider;

    [SerializeField] private TMP_Text YourAnswer;
    [SerializeField] private TMP_Text RightAnswer;
    [SerializeField] private TMP_Text GeneratedNumber;
    private TMP_Text gameOver;

    [SerializeField] private GameObject ShowNumber;
    [SerializeField] private GameObject AnswerNumber;

    private float fillTime = 1f;

    private int maxValue = 1;
    private int currentRank = 1;

    private long randomNumber;

    private int totalGamesPlayedLN;
    private int maxLevelReachedLN;
    private const string TotalGamesLongNum = "GPC1";
    private const string MaxLevelLongNum = "MLC1";


    void Start()
    {
        Time.timeScale = 1f;
        slider.maxValue = maxValue;

        totalGamesPlayedLN = PlayerPrefs.GetInt(TotalGamesLongNum);
        totalGamesPlayedLN++;
        PlayerPrefs.SetInt(TotalGamesLongNum, totalGamesPlayedLN);
        UpdateStat();

        RoundStart();
    }

    private void UpdateStat()
    {
        totalGamesPlayedLN = PlayerPrefs.GetInt(TotalGamesLongNum);
        maxLevelReachedLN = PlayerPrefs.GetInt(MaxLevelLongNum);

        if (currentRank > maxLevelReachedLN)
            maxLevelReachedLN = currentRank;

        PlayerPrefs.SetInt(TotalGamesLongNum, totalGamesPlayedLN);
        PlayerPrefs.SetInt(MaxLevelLongNum, maxLevelReachedLN);
    }

    public void StartGame()
    {
        GameOverWindow.SetActive(false);
        AnswerNumber.SetActive(false);
        ShowNumber.SetActive(true);

        totalGamesPlayedLN = PlayerPrefs.GetInt(TotalGamesLongNum);
        totalGamesPlayedLN++;
        PlayerPrefs.SetInt(TotalGamesLongNum, totalGamesPlayedLN);
        UpdateStat();

        currentRank = 1; fillTime = 1f; maxValue = 1;

        Time.timeScale = 1f;
        slider.maxValue = maxValue;
        InputField.GetComponent<TMP_InputField>().text = "";

        RoundStart();
    }

    private void RoundStart()
    {
        if (currentRank != 19)
        {
            long lowerBorder = BinaryPow(10, currentRank - 1);
            long higherBorder = BinaryPow(10, currentRank);

            randomNumber = (long)Random.Range(lowerBorder, higherBorder);
        }
        else
        {
            long lowerBorder = BinaryPow(10, currentRank - 1);
            long higherBorder = long.MaxValue;

            randomNumber = (long)Random.Range(lowerBorder, higherBorder);
        }

        GeneratedNumber.text = randomNumber.ToString();
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

        ShowNumber.SetActive(false);
        AnswerNumber.SetActive(true);
        Keyboard.SetActive(true);

        TMP_InputField inputField = InputField.GetComponent<TMP_InputField>();
        inputField.Select();
        inputField.ActivateInputField();
    }

    public void SubmitClicked()
    {
        long submitted = long.Parse(InputField.GetComponent<TMP_InputField>().text);

        if (submitted == randomNumber)
        {
            if (currentRank % 3 == 0 && maxValue < 8) //Raise max value of slider
            {
                maxValue++;
                fillTime++;

                slider.maxValue = maxValue;
            }
            currentRank++;

            InputField.GetComponent<TMP_InputField>().text = "";

            AnswerNumber.SetActive(false);
            ShowNumber.SetActive(true);

            RoundStart();
        }
        else
            GameOver(submitted);
    }

    private void GameOver(long submitted)
    {
        YourAnswer.text = submitted.ToString();
        RightAnswer.text = randomNumber.ToString();

        GameOverWindow.SetActive(true);
        Time.timeScale = 0f;

        gameOver = GameObject.Find("ResultText").GetComponent<TMP_Text>();
        gameOver.text = $"Reached level {currentRank}\r\nDo you want to play again?";
    }

    long BinaryPow(long baseNumber, long exponent)
    {
        long result = 1;
        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
                result *= baseNumber;
            baseNumber *= baseNumber;
            exponent >>= 1;
        }
        return result;
    }

    public void BackMenuClicked()
    {
        UpdateStat();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }
}
