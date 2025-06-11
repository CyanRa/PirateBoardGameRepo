using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DisplayPanelBehavior : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DefFleet;
    [SerializeField] TextMeshProUGUI DefHealth;
    [SerializeField] TextMeshProUGUI DefMoney;
    [SerializeField] TextMeshProUGUI DefDamage;
    [SerializeField] TextMeshProUGUI DefPoints;
    [SerializeField] Image DefImage;

    [SerializeField] TextMeshProUGUI AttFleet;
    [SerializeField] TextMeshProUGUI AttHealth;
    [SerializeField] TextMeshProUGUI AttMoney;
    [SerializeField] TextMeshProUGUI AttDamage;
    [SerializeField] TextMeshProUGUI AttPoints;
    [SerializeField] Image AttImage;


    public void SetAttStats(string fleet, string health, string money, string damage, string points)
    {
        
        AttFleet.text = fleet;
        AttHealth.text = health;
        AttMoney.text = money;
        AttDamage.text = damage;
        AttPoints.text = points;

    }

    public void SetDefStats(string fleet, string health, string money, string damage, string points)
    { 
        DefFleet.text = fleet;
        DefHealth.text = health;
        DefMoney.text = money;
        DefDamage.text = damage;
        DefPoints.text = points;
    }
}
