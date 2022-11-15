using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CharacterDamageGUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] float lifeTime = 1f, moveSpeed = 1f,textVibration = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, lifeTime);
        transform.position += new Vector3(0f, moveSpeed * Time.deltaTime);
    }

    public void SetDamage(int damageAmount,bool isCritical, bool heal)
    {
        if (damageAmount == 0)
        {
            damageText.color = Color.red;
            damageText.text = "Miss!";
            float jitterAmount = Random.Range(-textVibration, +textVibration);
            transform.position += new Vector3(jitterAmount, jitterAmount, 0f);
        }
        else if (isCritical)
        {
            damageText.color = Color.red;
            damageText.text = "Critical Strike " + damageAmount.ToString() + "!";
            float jitterAmount = Random.Range(-textVibration, +textVibration);
            transform.position += new Vector3(jitterAmount, jitterAmount, 0f);
            lifeTime++;
        }else
        {
            damageText.color = Color.red;
            damageText.text = damageAmount.ToString();
            float jitterAmount = Random.Range(-textVibration, +textVibration);
            transform.position += new Vector3(jitterAmount, jitterAmount, 0f);
        }
        if (heal)
        {
            damageText.color = Color.green;
            damageText.text = "+" + damageAmount.ToString();
            float jitterAmount = Random.Range(-textVibration, +textVibration);
            transform.position += new Vector3(jitterAmount, jitterAmount, 0f);
        }
    }

    public void SetDamage(string text)
    {
        damageText.text = text;
        float jitterAmount = Random.Range(-textVibration, +textVibration);
        transform.position += new Vector3(jitterAmount, jitterAmount, 0f);
    }
}
