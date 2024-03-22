using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TestMemoryGame : MonoBehaviour
{
    private Button[] buttons; 

    public GameObject GameOverWindow;

    [SerializeField] private Color originalColor;
    [SerializeField] private Color targetColor;
    [SerializeField] private Color gameOverColor;

    [SerializeField] private TMP_Text currentLevel;
    private TMP_Text gameOver;

    [SerializeField] private Animator animator;

    private List<int> BlinkSequence;

    private int currentIndex = 0;
    private int level = 1;

    private bool isOver = false;

    private int totalGamesPlayedTM;
    private int maxLevelReachedTM;
    private const string TotalGamesTestMem = "GPC0";
    private const string MaxLevelTestMem = "MLC0";


    void Start()
    {
        Time.timeScale = 1f;
        buttons = GetComponentsInChildren<Button>();

        StartGame();
    }

    private void UpdateStat()
    {
        totalGamesPlayedTM = PlayerPrefs.GetInt(TotalGamesTestMem);
        maxLevelReachedTM = PlayerPrefs.GetInt(MaxLevelTestMem);

        if (level > maxLevelReachedTM)
            maxLevelReachedTM = level;

        PlayerPrefs.SetInt(TotalGamesTestMem, totalGamesPlayedTM);
        PlayerPrefs.SetInt(MaxLevelTestMem, maxLevelReachedTM);
    }

    private IEnumerator WaitBeforeStartAndStartGame()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(PlayNextSequence());
    }

    private void StartGame()
    {
        totalGamesPlayedTM = PlayerPrefs.GetInt(TotalGamesTestMem);
        totalGamesPlayedTM++;
        PlayerPrefs.SetInt(TotalGamesTestMem, totalGamesPlayedTM);
        UpdateStat();

        for (int i = 0; i < 9; i++)
            buttons[i].GetComponent<Image>().color = originalColor;

        GameOverWindow.SetActive(false);
        currentIndex = 0; level = 1; isOver = false; 
        currentLevel.text = $"{level}";

        Time.timeScale = 1f;

        BlinkSequence = new List<int>();
        BlinkSequence.Clear();

        StartCoroutine(WaitBeforeStartAndStartGame());
    }

    private IEnumerator PlayNextSequence()
    {
        BlinkSequence.Add(Random.Range(0, 9));

        int sequenceLength = BlinkSequence.Count;

        for (int i = 0; i < sequenceLength; i++)
        {
            Button currentButton = buttons[BlinkSequence[i]];
            Image buttonImage = currentButton.GetComponent<Image>();

            StartCoroutine(ButtonBlink(buttonImage, 0.5f));

            bool isLastButton = i == sequenceLength - 1;

            if (!isLastButton)
                yield return new WaitForSeconds(0.6f);
        }

        currentIndex = 0;
        isOver = false;
    }

    private IEnumerator ButtonBlink(Image buttonImage, float time)
    {
        buttonImage.color = targetColor;
        yield return new WaitForSeconds(time);
        buttonImage.color = originalColor;
    }

    public void OnButtonClick(int buttonIndex)
    {
        if (isOver) return; //Ignore button presses

        if (buttonIndex == BlinkSequence[currentIndex]) //Right button pressed
        {
            Button pressedButton = buttons[buttonIndex];
            Image buttonImage = pressedButton.GetComponent<Image>();

            StartCoroutine(ButtonBlink(buttonImage, 0.2f));

            currentIndex++;

            if (currentIndex == BlinkSequence.Count)
            {
                currentIndex = 0; //The player correctly reproduced the entire sequence
                isOver = true; //Waiting for new sequence

                level++;
                currentLevel.text = $"{level}";
                animator.SetTrigger("Change");

                StartCoroutine(WaitBeforeNewSequence());
            }
        }
        else
        {
            buttons[buttonIndex].GetComponent<Image>().color = gameOverColor;
            isOver = true;

            GameOver(level);
        }
    }

    public void GameOver(int level)
    {
        GameOverWindow.SetActive(true);

        Time.timeScale = 0f;
        gameOver = GameObject.Find("ResultText").GetComponent<TMP_Text>();
        gameOver.text = $"Reached level {level}\r\nDo you want to play again?";
    }

    private IEnumerator WaitBeforeNewSequence()
    {
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(PlayNextSequence());
    }

    public void BackMenuClicked()
    {
        UpdateStat();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }
}


