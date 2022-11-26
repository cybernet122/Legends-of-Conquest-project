using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickToggle : MonoBehaviour
{
    [SerializeField] Image image;
    bool toggle = false;

    private void Start()
    {
        image.gameObject.SetActive(false);
        if (PlayerPrefs.HasKey("Joystick_"))
        {
            if (PlayerPrefs.GetInt("Joystick_") == 1)
            {
                image.gameObject.SetActive(true);
                toggle = true;
                if (Utilities.ReturnSceneName() != "Main Menu")
                    GameManager.instance.ToggleJoystickControls(true);
            }
            else
            {
                image.gameObject.SetActive(false);
                toggle = false;
                if (Utilities.ReturnSceneName() != "Main Menu")
                    GameManager.instance.ToggleJoystickControls(false);
            }
        }
    }

    public void Toggle()
    {
        toggle = !toggle;
        image.gameObject.SetActive(toggle);
        GameManager.instance.ToggleJoystickControls(toggle);
        if (toggle)
            PlayerPrefs.SetInt("Joystick_", 1);
        else
            PlayerPrefs.SetInt("Joystick_", 0);
    }

}
