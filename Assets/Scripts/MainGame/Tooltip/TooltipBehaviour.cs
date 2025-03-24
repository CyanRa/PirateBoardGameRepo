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
        float pivotY = position.y/Screen.height;
        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
}
