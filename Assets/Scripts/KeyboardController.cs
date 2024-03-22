using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class KeyboardController : MonoBehaviour
{
    [SerializeField] private TMP_InputField InputField;

    public void OnButtonClick(string buttonSymbol)
    {
        InputField.text += buttonSymbol;
    }

    public void BackSpaceClick()
    {
        string str = InputField.text;
        str = str.Remove(str.Length - 1);

        InputField.text = str;
    }
}