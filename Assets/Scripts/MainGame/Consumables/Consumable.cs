using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Microsoft.Unity.VisualStudio.Editor;

public abstract class Consumable : ScriptableObject
{  
    public abstract void UseConsumable(FleetManager ConsumableUser);
}


[CreateAssetMenu(fileName = "StormCalling", menuName = "Consumable/StormCalling", order = 1)]
public class StormCalling : Consumable
{    
    string image = "StormCallingImage";
    public override void UseConsumable(FleetManager ConsumableUser)
    {
       Debug.Log("Storm Calling");
    }
}
[CreateAssetMenu(fileName = "TributeToTheOldGods", menuName = "Consumable/TributeToTheOldGods", order = 1)]
public class TributeToTheOldGods : Consumable
{
    string image = "TributeToGodsImage";
    public override void UseConsumable(FleetManager ConsumableUser)
    {
        Debug.Log("Tribute to The Old Gods");
    }
}

[CreateAssetMenu(fileName = "Passage", menuName = "Consumable/Passage", order = 1)]
public class Passage : Consumable
{
    string image = "Passage";
    public override void UseConsumable(FleetManager ConsumableUser)
    {
       Debug.Log("PASSAGE USED");      
    }
}

[CreateAssetMenu(fileName = "GreekFire", menuName = "Consumable/GreekFire", order = 1)]
public class GreekFire : Consumable
{
    string image = "GreekFire";
    public override void UseConsumable(FleetManager ConsumableUser)
    {
       Debug.Log("Greek Fire");      
    }
}

[CreateAssetMenu(fileName = "CaptainsDecree", menuName = "Consumable/CaptainsDecree", order = 1)]
public class CaptainsDecree : Consumable
{
    string image = "CaptainsDecree";
    public override void UseConsumable(FleetManager ConsumableUser)
    {
       Debug.Log("Captains Decree");       
    }
}


[CreateAssetMenu(fileName = "TalesOfTheFlagShip", menuName = "Consumable/TalesOfTheFlagShip", order = 1)]
public class TalesOfTheFlagship : Consumable
{
    string image = "TalesOfTheFlagShip";
    public override void UseConsumable(FleetManager ConsumableUser)
    {
       Debug.Log("Tales of The Flagship");
       
    }
}

[CreateAssetMenu(fileName = "PirateAlliance", menuName = "Consumable/PirateAlliance", order = 1)]
public class PirateAlliance : Consumable
{
    string image = "PirateAlliance";
    public override void UseConsumable(FleetManager ConsumableUser)
    {
       Debug.Log("Pirate Alliance");       
    }
}

[CreateAssetMenu(fileName = "Shipwright", menuName = "Consumable/Shipwright", order = 1)]
public class Shipwright : Consumable
{
    string image = "Shipwright";
    public override void UseConsumable(FleetManager ConsumableUser)
    {
       
       Debug.Log("Shipwright");       
    }
}