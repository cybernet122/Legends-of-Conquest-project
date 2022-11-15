using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField, contentField;
    public LayoutElement layoutElement;
    public int characterWrapLimit;
    public RectTransform rectTransform;
    [SerializeField]private Vector3 offset;
    [SerializeField]private float padding;
    private Canvas canvas;
    Vector3 position;
    bool pointer;

    private void OnEnable()
    {
        TooltipTrigger.ShowAtPosition += SetPosition;
    }
    private void OnDisable()
    {
        TooltipTrigger.ShowAtPosition -= SetPosition;        
    }

    private void SetPosition(Vector3 vector3)
    {
        position = vector3;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void SetText(bool pointer,string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }
        contentField.text = content;
        this.pointer = pointer;
        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;
        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor)
        {
            int headerLength = headerField.text.Length;
            int contentLength = contentField.text.Length;
            layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
        }
        Vector2 position;
        if (pointer)
            position = Input.mousePosition + offset;
        else
            position = this.position + offset;
        float rightEdgeToScreenEdgeDistance = Screen.width - (position.x + rectTransform.rect.width * canvas.scaleFactor / 2) - padding;
        if(rightEdgeToScreenEdgeDistance < 0)
        {
            position.x += rightEdgeToScreenEdgeDistance;
        }
        float leftEdgeToScreenEdgeDistance = 0 - (position.x + rectTransform.rect.width * canvas.scaleFactor / 2) + padding;
        if (leftEdgeToScreenEdgeDistance > 0)
        {
            position.x += leftEdgeToScreenEdgeDistance;
        }
        float topEdgeToScreenEdgeDistance = Screen.height - (position.y + rectTransform.rect.height * canvas.scaleFactor) - padding;
        if (topEdgeToScreenEdgeDistance < 0)
        {
            position.x += topEdgeToScreenEdgeDistance;
        }        
        transform.position = position;

        /*
                Vector2 position = Input.mousePosition;

                float pivotX = position.x / Screen.width;
                float pivotY = position.y / Screen.height;

                rectTransform.pivot = new Vector2(pivotX, pivotY);
                transform.position = position;*/
    }

    public void ClearText()
    {
        headerField.text = "";
        contentField.text = "";
    }
}
