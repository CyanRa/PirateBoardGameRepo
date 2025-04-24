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
        myConsumables.Add(tempConsumable);
        tempConsumable = Resources.Load<Consumable>("Scriptables/GreekFire");
        myConsumables.Add(tempConsumable);
        tempConsumable = Resources.Load<Consumable>("Scriptables/Passage");
        myConsumables.Add(tempConsumable);
        tempConsumable = Resources.Load<Consumable>("Scriptables/Shipwright");
        myConsumables.Add(tempConsumable);
    }

    public void AddConsumable(){
        List<int> possibleOutcomes = new List<int>();
        if(myConsumables.Count < 6){
            foreach(Consumable consumable in myConsumables){
                if(consumable==null){
                    possibleOutcomes.Add(consumable.consumableIndex);
                }
            }
        }
        myConsumables?.Add(allConsumables[possibleOutcomes[UnityEngine.Random.Range(0, possibleOutcomes.Count)]]);  
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
