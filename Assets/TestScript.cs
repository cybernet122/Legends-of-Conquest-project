using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestScript : MonoBehaviour
{
    //public delegate void AnswerCallback(int damage);
    public static event UnityAction TakeDamage;
    public static void OnHit() => TakeDamage?.Invoke();

    public delegate void MyEvent();
    public static event MyEvent onHit;
    public static void FireOnHit() => onHit?.Invoke();

    private void OnEnable()
    {
        //OnUnityAnswers += QuestionAnswered;
    }

    

    void Start()
    {
        // Whenever Unity Answers, call QuestionAnswered.
        /*if (OnHit != null)
        {
            OnHit();
        }*/
    }

    // Calls, "raises" or "invokes" the event.
    /*public void RaiseAnswer(string message)
    {
        if (OnUnityAnswers != null)
            OnUnityAnswers(message);
    }*/

    // Callback signature
    /*public void QuestionAnswered(string message)
    {
        // Some dummy example code.
        Debug.Log(message);
    }*/

    private void OnDisable()
    {
       // OnUnityAnswers -= QuestionAnswered;
    }
}
