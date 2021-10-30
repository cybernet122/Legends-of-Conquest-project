using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEnter : MonoBehaviour
{
    public string transitionAreaName;

    // Start is called before the first frame update
    void Start()
    {
        if (transitionAreaName == Player.instance.transitionName && !QuestManager.instance.CheckIfComplete("Speak to Innkeeper"))
        {
            Player.instance.transform.position = transform.position;
        }
    }

}
