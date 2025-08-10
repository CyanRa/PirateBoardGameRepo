using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Alteruna;

public class PreGameNameDisplay : AttributesSync
{
    public static bool nameSet;
    
    public void BroadcastDisplayName()
    {
        if (!nameSet)
        {
            BroadcastRemoteMethod("DisplayName", Multiplayer.Instance.GetUser().Name);
            nameSet = true;
        } 
        
    }
    [SynchronizableMethod]
    private void DisplayName(string name)
    {
        GetComponent<TextMeshProUGUI>().text = name;
    }
}
