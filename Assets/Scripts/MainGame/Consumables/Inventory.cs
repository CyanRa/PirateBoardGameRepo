using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;


public class Inventory : MonoBehaviour
{
    public Transform InventoryPanel;
    public GameObject consumablePrefab;
    [SerializeField]List<Consumable> allConsumables;
    public List<Consumable> myConsumables;
    public Consumable[] playerConsumables = new Consumable[6];
    FleetManager myFleet;
    MenuBehaviour myMenu;

    public void Awake()
    {
        myFleet = GetComponent<FleetManager>();
        myMenu = GameObject.Find("MenuSystem")?.GetComponent<MenuBehaviour>();
        LoadConsumables();
    }



    private void LoadConsumables()
    {
        Consumable tempConsumable = Resources.Load<Consumable>("Scriptables/StormCalling");
        allConsumables.Add(tempConsumable);
        tempConsumable = Resources.Load<Consumable>("Scriptables/GreekFire");
        allConsumables.Add(tempConsumable);
        tempConsumable = Resources.Load<Consumable>("Scriptables/Passage");
        allConsumables.Add(tempConsumable);
        tempConsumable = Resources.Load<Consumable>("Scriptables/Shipwright");
        allConsumables.Add(tempConsumable);
        tempConsumable = Resources.Load<Consumable>("Scriptables/CaptainsDecree");
        allConsumables.Add(tempConsumable);
    }

    public void AddConsumable(){
        List<int> possibleOutcomes = new List<int>();
        myConsumables?.Add(allConsumables[UnityEngine.Random.Range(0, allConsumables.Count)]);  
    }

    public void InstantiateConsumables(){
        myMenu.consumablePanel.GetComponent<ConsumableMenuBehaviour>().InstantiateConsumables(myConsumables);
    }
    public void Update(){
    
        if(Input.GetKeyDown(KeyCode.K)){
           AddConsumable();
        }
        
    }

}
