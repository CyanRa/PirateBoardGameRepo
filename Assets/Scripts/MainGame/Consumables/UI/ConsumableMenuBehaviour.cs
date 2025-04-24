using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableMenuBehaviour : MonoBehaviour
{
public GameObject consumablePrefab;
public GameObject consumableInspectorPanel;
public GameObject consumableButtonHolder;

    public void InstantiateConsumables(List<Consumable> consumables){
        foreach(Consumable consumable in consumables){
            GameObject newConsumableButton = Instantiate(consumablePrefab);
            newConsumableButton.transform.SetParent(consumableButtonHolder.transform);
            newConsumableButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(consumable.image);
            newConsumableButton.GetComponent<Button>().onClick.RemoveAllListeners();
            newConsumableButton.GetComponent<Button>().onClick.AddListener(() => ShowConsumableInspector(consumable));          
        }      
    }
    public  void DeleteConsumables(){
        foreach(Transform child in consumableButtonHolder.transform){
            Destroy(child.gameObject);
        }
    }
    public void HideConsumableInspector(){
        consumableInspectorPanel.GetComponent<ConsumableBehaviour>().CloseConsumableInspector();
    }
    private void ShowConsumableInspector(Consumable consumable){  
        consumableInspectorPanel.SetActive(true);     
        consumableInspectorPanel.GetComponent<ConsumableBehaviour>().LoadConsumableInspector(consumable);
    }
}
