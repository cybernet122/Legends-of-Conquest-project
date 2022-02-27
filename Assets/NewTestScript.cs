using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTestScript : MonoBehaviour
{
    private void OnEnable()
    {
        TestScript.onHit += TakeDamage;
        TestScript.OnHit();
        TestScript.TakeDamage += TakeDamage;
    }

    public void TakeDamage()
    {
    }

    private void OnDisable()
    {
        TestScript.onHit -= TakeDamage;

    }
}
