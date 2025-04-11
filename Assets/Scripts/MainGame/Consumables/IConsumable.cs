using UnityEngine;

public interface IConsumable 
{
    public void UseConsumable();
    string Name { get; }
    Sprite Icon { get; }
}
