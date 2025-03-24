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
}
