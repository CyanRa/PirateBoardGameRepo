using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

[ExecuteInEditMode()]
public class TooltipBehaviour : MonoBehaviour
{
    public TextMeshProUGUI header;
    public TextMeshProUGUI content;
    public LayoutElement layoutElement;
    public int wrapLimit;
    public RectTransform rectTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetText(string _content, string _header = ""){
        if(string.IsNullOrEmpty(_header)){
            header.gameObject.SetActive(false);
        }else{
            header.gameObject.SetActive(true);
            header.text = _header;

        }
        content.text = _content;
        int headerLength = header.text.Length;
        int contentLength = content.text.Length;
        layoutElement.enabled = (headerLength > wrapLimit || contentLength > wrapLimit);
    }
    public void Update()
    {
        Vector2 position = Input.mousePosition;
        float pivotX = position.x/Screen.width;
        if(pivotX <= 0.8f){pivotX =0;position.x += 20f;}else{pivotX = 1; position.x -= 20f;}
        float pivotY = position.y/Screen.height;
        if(pivotY <= 0.8f){pivotY =0;position.y += 20f;}else{pivotY = 1;position.y -=20f;}
        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
}
