using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFade : MonoBehaviour
{
    [SerializeField] CanvasGroup alpha;
    // Start is called before the first frame update
    void Update()
    {
        if(alpha.alpha > Mathf.Epsilon)
        alpha.alpha -= 0.3f * Time.deltaTime;
    }


}
