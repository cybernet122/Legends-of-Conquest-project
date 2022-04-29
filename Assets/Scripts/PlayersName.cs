using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
public class PlayersName : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button nextButton;
    public void SetPlayersName()
    {
        PlayerPrefs.SetString("Players_name_", inputField.text);
    }

    public void CheckIfEmpty()
    {
        bool x = inputField.text.Length > 0 ? nextButton.interactable = true : nextButton.interactable = false;        
    }
}
