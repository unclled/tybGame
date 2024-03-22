using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TraingleRotationGame : MonoBehaviour
{
    [SerializeField] private GameObject triangle;
    [SerializeField] private GameObject GameOverWindow;

    [SerializeField] private TMP_Text currentLevel;
    private TMP_Text gameOver;

    [SerializeField] private Color originalColor;
    [SerializeField] private Color targetColor;
    [SerializeField] private Color gameOverColor;

    [SerializeField] private Animator animator;

    private List<int> rotateSequence = new List<int>(); //0 - RotateLeft, 1 - RotateRight

    private int currentIndex = 0;
    private int buttonClickedCounter = 0;
    private int level = 1;

    private float speed = 0.5f;
    private float timeBeforeNewSpeen = 1.1f;

    private bool isOver = false;
    private bool isGameOver = false;

    private int totalGamesPlayedTr;
    private int maxLevelReachedTr;
    private const string TotalGamesTriangle = "GPC3";
    private const string MaxLevelTriangle = "MLC3";

    void Start()
    {
        Time.timeScale = 1f;

        totalGamesPlayedTr = PlayerPrefs.GetInt(TotalGamesTriangle);
        totalGamesPlayedTr++;
        PlayerPrefs.SetInt(TotalGamesTriangle, totalGamesPlayedTr);
        UpdateStat();

        StartCoroutine(WaitBeforeStartAndStartGame());
    }

    private void UpdateStat()
    {
        totalGamesPlayedTr = PlayerPrefs.GetInt(TotalGamesTriangle);
        maxLevelReachedTr = PlayerPrefs.GetInt(MaxLevelTriangle);

        if (level > maxLevelReachedTr)
            maxLevelReachedTr = level;

        PlayerPrefs.SetInt(TotalGamesTriangle, totalGamesPlayedTr);
        PlayerPrefs.SetInt(MaxLevelTriangle, maxLevelReachedTr);
    }

    private IEnumerator WaitBeforeStartAndStartGame()
    {
        yield return new WaitForSeconds(1);
        StartCoroutine(PlayNextSequence());
    }

    public void StartGame()
    {
        totalGamesPlayedTr = PlayerPrefs.GetInt(TotalGamesTriangle);
        totalGamesPlayedTr++;
        PlayerPrefs.SetInt(TotalGamesTriangle, totalGamesPlayedTr);
        UpdateStat();

        triangle.GetComponent<SpriteRenderer>().color = originalColor;

        GameOverWindow.SetActive(false);

        currentIndex = 0; level = 1; isOver = false; isGameOver = false;
        currentLevel.text = $"{level}";
        rotateSequence = new List<int>();
        rotateSequence.Clear();

        Time.timeScale = 1f;
        triangle.transform.rotation = Quaternion.Euler(0, 0, 0);

        StartCoroutine(WaitBeforeStartAndStartGame());
    }

    private IEnumerator PlayNextSequence()
    {
        rotateSequence.Add(Random.Range(0, 2));

        int sequenceLength = rotateSequence.Count;

        for (int i = 0; i < sequenceLength; i++)
        {
            if (rotateSequence[i] == 0)
                StartCoroutine(RotateLeft(false, buttonClickedCounter));
            else
                StartCoroutine(RotateRight(false, buttonClickedCounter));

            bool isLastRotate = i == sequenceLength - 1;

            if (!isLastRotate)
                yield return new WaitForSeconds(speed + 0.5f);
        }
       
        currentIndex = 0;
        isOver = false;
    }

    public void RotateLeftButtonClicked()
    {
        if (isOver) return;

        buttonClickedCounter++;

        if (rotateSequence[currentIndex] == 0)
        {
            triangle.GetComponent<SpriteRenderer>().color = targetColor;

            StartCoroutine(RotateLeft(true, buttonClickedCounter));

            currentIndex++;

            if (currentIndex == rotateSequence.Count)
            {
                isOver = true;
                level++;
                currentLevel.text = $"{level}";
                animator.SetTrigger("Change");

                if (level % 3 == 0 && speed > 0.15f && timeBeforeNewSpeen > 0.54f)
                {
                    speed -= 0.05f;
                    timeBeforeNewSpeen -= 0.8f;
                }

                StartCoroutine(WaitASecond());
            }
        }
        else
        {
            triangle.GetComponent<SpriteRenderer>().color = gameOverColor;
            isGameOver = true;

            GameOver();
        }
    }

    public void RotateRightButtonClicked()
    {
        if (isOver) return;

        buttonClickedCounter++;

        if (rotateSequence[currentIndex] == 1)
        {
            triangle.GetComponent<SpriteRenderer>().color = targetColor;

            StartCoroutine(RotateRight(true, buttonClickedCounter));

            currentIndex++;

            if (currentIndex == rotateSequence.Count)
            {
                isOver = true;
                level++;
                currentLevel.text = $"{level}";
                animator.SetTrigger("Change");

                if (level % 3 == 0 && speed > 0.15f) speed -= 0.05f;

                StartCoroutine(WaitASecond());
            }
        }
        else
        {
            triangle.GetComponent<SpriteRenderer>().color = gameOverColor;
            isGameOver = true;

            GameOver();
        }
    }

    private IEnumerator WaitASecond()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(PlayNextSequence());
    }

    public IEnumerator RotateLeft(bool changeColor, int buttonClicked)
    {
        if (triangle == null) yield break;

        if (changeColor)
            triangle.GetComponent<SpriteRenderer>().color = targetColor;

        float targetAngle = 360f;
        float currentAngle = 0f;
        float rotateSpeed = 360f / speed;

        while (currentAngle < targetAngle)
        {
            if (triangle == null || buttonClicked != buttonClickedCounter) yield break;

            currentAngle += rotateSpeed * Time.deltaTime;
            triangle.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        triangle.transform.rotation = Quaternion.Euler(0, 0, 360);

        if (!isGameOver)
            triangle.GetComponent<SpriteRenderer>().color = originalColor;
        else
            triangle.GetComponent<SpriteRenderer>().color = gameOverColor;
    }

    public IEnumerator RotateRight(bool changeColor, int buttonClicked)
    {
        if (triangle == null) yield break;

        if (changeColor)
            triangle.GetComponent<SpriteRenderer>().color = targetColor;

        float targetAngle = 0f;
        float currentAngle = 360f;
        float rotateSpeed = 360f / speed; 

        while (currentAngle > targetAngle)
        {
            if (triangle == null || buttonClicked != buttonClickedCounter) yield break;

            currentAngle -= rotateSpeed * Time.deltaTime;
            triangle.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            yield return null;
        }

        triangle.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (!isGameOver)
            triangle.GetComponent<SpriteRenderer>().color = originalColor;
        else
            triangle.GetComponent<SpriteRenderer>().color = gameOverColor;
    }

    public void GameOver()
    {
        GameOverWindow.SetActive(true);

        Time.timeScale = 0f;

        gameOver = GameObject.Find("ResultText").GetComponent<TMP_Text>();
        gameOver.text = $"Reached level {level}\r\nDo you want to play again?";
    }

    public void BackMenuClicked()
    {
        UpdateStat();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }
}