using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeWindows : MonoBehaviour
{
    public GameObject GamesWindow;
    public GameObject ProfileWindow;

    private Button[] buttons;

    [SerializeField] private List<Image> rules = new List<Image>();
    [SerializeField] private List<TMP_Text> totalGames = new List<TMP_Text>();
    [SerializeField] private List<TMP_Text> maxLevel = new List<TMP_Text>();


    private void Awake()
    {
        Application.targetFrameRate = 500;
        buttons = GetComponentsInChildren<Button>();
        LoadStatistics();
    }

    public void ProfileButtonClicked()
    {
        foreach (Image rule in rules)
        {
            rule.transform.localScale = new Vector3(0f, 0f, 100f);
        }
        GamesWindow.SetActive(false);
        ProfileWindow.SetActive(true);
        LoadStatistics();
    }

    public void GamesWindowClicked()
    {
        ProfileWindow.SetActive(false);
        GamesWindow.SetActive(true);
    }

    public void ExitPressed()
    {
        Application.Quit();
        Debug.Log("Exit Pressed!");
    }

    public void TestMemoryPressed()
    {
        SceneManager.LoadScene("TestMemory");
    }

    public void LongNumberPressed()
    {
        SceneManager.LoadScene("LongNumber");
    }

    public void LongTextPressed()
    {
        SceneManager.LoadScene("LongText");
    }

    public void ColorMemoryPressed()
    {
        SceneManager.LoadScene("ColorMemory");
    }

    public void RightLeftPressed()
    {
        SceneManager.LoadScene("TriangleRotationSequence");
    }

    public void RulesButtonPressed(int buttonIndex)
    {
        rules[buttonIndex].GetComponent<Animator>().SetTrigger("HelpPressed");
    }

    public void LoadStatistics()
    {
        for (int i = 0; i < totalGames.Count; i++)
        {
            string GPC = "GPC" + i;
            string MPC = "MLC" + i;
            int totalGamesPlayed = PlayerPrefs.GetInt(GPC);
            int maxLevelReached = PlayerPrefs.GetInt(MPC);

            totalGames[i].text = totalGamesPlayed.ToString();
            maxLevel[i].text = maxLevelReached.ToString();
        }
    }

    public void ResetPressed()
    {
        for (int i = 0; i < totalGames.Count; i++)
        {
            string GPC = "GPC" + i;
            string MPC = "MLC" + i;
            PlayerPrefs.SetInt(GPC, 0);
            PlayerPrefs.SetInt(MPC, 0);
            totalGames[i].text = "0";
            maxLevel[i].text = "0";
        }
    }
}
