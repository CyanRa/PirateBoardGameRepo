using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;
    public TooltipBehaviour Tooltip;

    public void Awake()
    {
        current = this;
    }
    public static void Hide(){
        current.Tooltip.gameObject.SetActive(false);
    }
    public static void Show(string _content, string _header = ""){
        current.Tooltip.SetText(_content, _header);
        current.Tooltip.gameObject.SetActive(true);
    }
    public static void SetAllignmentTopLeft(){
        current.Tooltip.content.alignment = TMPro.TextAlignmentOptions.TopLeft;
    }
    public static void SetAllignmentMiddle(){
        current.Tooltip.content.alignment = TMPro.TextAlignmentOptions.Midline;

    }
}
