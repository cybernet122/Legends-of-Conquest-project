using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class QuestUpdate : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questText;
    public void PlayUpdateAnimation(string quest)
    {
        Animator questPanel = GetComponent<Animator>();
        questText.text = "Quest update : " + quest;
        questPanel.Play("Quest Fade");
    }
}
