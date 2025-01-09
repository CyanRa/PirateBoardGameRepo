using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;


[CreateAssetMenu(fileName = "CrewCard", menuName = "Scriptable Objects/CrewCard")]
public class CrewCard : ScriptableObject
{
    public new string name;

    public int power;
    public Sprite image;

}
