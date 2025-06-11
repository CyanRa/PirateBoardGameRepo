using UnityEngine;

public class DisplaySystem : MonoBehaviour
{
    private static DisplaySystem current;
    public DisplayPanelBehavior displayPanelBehavior;

    void Awake()
    {
        current = this;
    }
    public static void SetAttDisplay(string fleet, string health, string money, string damage, string points)
    {
        current.displayPanelBehavior.SetAttStats(fleet, health, money, damage, points);
    }
    public static void SetDefDisplay(string fleet, string health, string money, string damage, string points)
    {
        current.displayPanelBehavior.SetDefStats(fleet, health, money, damage, points);
    }
}
