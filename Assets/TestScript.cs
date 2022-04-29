using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestScript : MonoBehaviour
{
    //public delegate void AnswerCallback(int damage);
    /*public static event UnityAction TakeDamage;
    public static void OnHit() => TakeDamage?.Invoke();

    public delegate void MyEvent();
    public static event MyEvent onHit;
    public static void FireOnHit() => onHit?.Invoke();*/

    public static event UnityAction<string> MoveName;

    private void OnEnable()
    {
        MoveName += PrintName;
    }

    private void PrintName(string value)
    {
        print(value);
    }

    void Start()
    {
        MoveName?.Invoke("Test");
    }

    private void OnDisable()
    {
        MoveName -= PrintName;
    }
}
