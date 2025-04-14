using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Transform InventoryPanel;
    public GameObject consumablePrefab;
    [SerializeField]List<Consumable> allConsumables;
    public List<Consumable> myConsumables;
    FleetManager myFleet;
    MenuBehaviour myMenu;

    public void Awake()
    {
        myFleet = GetComponent<FleetManager>();
        myMenu = GameObject.Find("MenuSystem").GetComponent<MenuBehaviour>();
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
