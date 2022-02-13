using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
public class PlayersName : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    public void SetPlayersName()
    {
        PlayerPrefs.SetString("Players_name_", inputField.text);
    }

}
