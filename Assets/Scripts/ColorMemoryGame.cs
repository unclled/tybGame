using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;
using random = System.Random;

public class ColorMemoryGame : MonoBehaviour
{
    private Button[] buttons;

    private List<Color> colors;
    private List<Color> saveColors;
    [SerializeField] List<Color> RandomColors = new List<Color>();

    [SerializeField] private GameObject GameOverWindow;
    [SerializeField] private List<GameObject> Levels = new List<GameObject>();

    [SerializeField] private TMP_Text currentLevel;
    private TMP_Text gameOver;

    [SerializeField] private Slider slider;

    [SerializeField] private Animator levelAnimate;

    private float fillTime = 5f;

    private int maxValue = 5;
    private int buttonPressedCounter = 1;
    private int levelsBeforeChange = 1;
    private int currentLevelForList = 1;
    private int level = 1;

    private bool isOver = false;
    private bool isGameOver = false;

    private int totalGamesPlayedColor;
    private int maxLevelReachedColor;
    private const string TotalGamesColorMemory = "GPC4";
    private const string MaxLevelColorMemory = "MLC4";

    void Start()
    {
        colors = new List<Color>();  
        buttons = Levels[0].GetComponentsInChildren<Button>();
        slider.maxValue = maxValue;

        Time.timeScale = 1.0f;

        totalGamesPlayedColor = PlayerPrefs.GetInt(TotalGamesColorMemory);
        totalGamesPlayedColor++;
        PlayerPrefs.SetInt(TotalGamesColorMemory, totalGamesPlayedColor);
        UpdateStat();

        RandomPaintButtons();
    }

    private void UpdateStat()
    {
        totalGamesPlayedColor = PlayerPrefs.GetInt(TotalGamesColorMemory);
        maxLevelReachedColor = PlayerPrefs.GetInt(MaxLevelColorMemory);

        if (level > maxLevelReachedColor)
            maxLevelReachedColor = level;

        PlayerPrefs.SetInt(TotalGamesColorMemory, totalGamesPlayedColor);
        PlayerPrefs.SetInt(MaxLevelColorMemory, maxLevelReachedColor);
    }

    private void RandomPaintButtons()
    {
        Shuffle(RandomColors);

        for (int i = 0; i < buttons.Length; i++)
        {
            Image buttonImage = buttons[i].GetComponent<Image>();
            buttonImage.color = RandomColors[i];
            colors.Add(buttonImage.color);
        }

        isOver = true; //Prohibit pressing buttons

        saveColors = colors.GetRange(0, colors.Count); //Copy colors for random buttons paint

        StartCoroutine(FillValue());
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);

            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
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

        slider.gameObject.SetActive(false);

        foreach (Button button in buttons)
        {
            button.GetComponent<Animator>().SetTrigger("FlipButton");
        }
        yield return new WaitForSeconds(0.5f);

        foreach (Button button in buttons)
        {
            Image buttonImage = button.GetComponent<Image>();
            int index = Random.Range(0, saveColors.Count);
            buttonImage.color = saveColors[index];
            saveColors.RemoveAt(index);
        }

        isOver = false; //Allow pressing buttons
    }

    public void ButtonPressed(int buttonIndex)
    {
        if (isOver || isGameOver) return;

        if (buttonPressedCounter <= buttons.Length)
        {
            Button pressedButton = buttons[buttonIndex];
            TMP_Text buttonText = pressedButton.GetComponentInChildren<TMP_Text>();

            if (buttonText.text == "")
            {
                buttonText.text = buttonPressedCounter.ToString();
                buttonText.GetComponent<Animator>().SetTrigger("NumberOut");
            }
            else
            {
                buttonText.text = "";
                buttonPressedCounter--;
                return;
            }

            buttonPressedCounter++;
        }
    }

    public void SubmitButtonClicked()
    {
        if (isOver || isGameOver) return;

        foreach (Button button in buttons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            Image buttonImage = button.GetComponent<Image>();

            try
            {
                int numberOfButton = int.Parse(buttonText.text);
                if (buttonImage.color != colors[numberOfButton - 1])
                    isGameOver = true;
            }
            catch 
            {
                isGameOver = false;
                return;
            }
        }

        if (isGameOver) GameOver();
        else
        {
            levelsBeforeChange--;
            level++;
            currentLevel.text = $"{level}";
            levelAnimate.SetTrigger("Change");

            if (levelsBeforeChange == 0)
            {
                currentLevelForList++;
                levelsBeforeChange = currentLevelForList;

                DisableEnableLevels(currentLevelForList);
            }
            else
                InitializeBeforeNewLevel();
        }
    }

    public void ClearAllPressed()
    {
        foreach (Button button in buttons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = "";
        }
        buttonPressedCounter = 1;
    }

    private void InitializeBeforeNewLevel()
    {
        buttonPressedCounter = 1;

        colors.Clear();
        saveColors.Clear();

        slider.gameObject.SetActive(true);

        foreach (Button button in buttons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = "";
        }

        RandomPaintButtons();
    }

    public void GameOver()
    {
        GameOverWindow.SetActive(true);

        Time.timeScale = 0f;

        gameOver = GameObject.Find("ResultText").GetComponent<TMP_Text>();
        gameOver.text = $"Reached level {level}\r\nDo you want to play again?";
    }

    public void StartNewGame()
    {
        GameOverWindow.SetActive(false);

        totalGamesPlayedColor = PlayerPrefs.GetInt(TotalGamesColorMemory);
        totalGamesPlayedColor++;
        PlayerPrefs.SetInt(TotalGamesColorMemory, totalGamesPlayedColor);
        UpdateStat();
        

        isOver = false; isGameOver = false;
        level = 1; maxValue = 5; levelsBeforeChange = 1; currentLevelForList = 1; buttonPressedCounter = 1;
        fillTime = 5f;

        currentLevel.text = $"{level}";

        colors.Clear();
        saveColors.Clear();

        slider.maxValue = maxValue;
        slider.gameObject.SetActive(true);

        foreach (GameObject level in Levels)
        {
            level.SetActive(false);
        }

        buttons = Levels[0].GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = "";
        }
        Levels[0].SetActive(true);

        Time.timeScale = 1f;

        RandomPaintButtons();
    }

    private void DisableEnableLevels(int currentLevel)
    {
        Levels[currentLevel - 2].SetActive(false);
        Levels[currentLevel - 1].SetActive(true);

        fillTime += 3;
        maxValue += 3;
        buttonPressedCounter = 1;

        colors.Clear();
        saveColors.Clear();

        slider.maxValue = maxValue;
        slider.gameObject.SetActive(true);

        buttons = Levels[currentLevel - 1].GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();
            buttonText.text = "";
        }

        RandomPaintButtons();
    }

    public void BackMenuClicked()
    {
        UpdateStat();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }
}
