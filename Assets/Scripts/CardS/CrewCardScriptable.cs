using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.EventSystems;


/// <summary>
/// Catagories for Sciptable Crew Cards
/// </summary>
[CreateAssetMenu(fileName = "CrewCard", menuName = "CardData")]
public class CrewCardScriptable : ScriptableObject
{
    [field: SerializeField] public string crewCardName {  get; private set; }

    [field: SerializeField] public int power { get; private set; }

    [field: SerializeField] public Sprite image { get; private set; }
}
