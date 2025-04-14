using UnityEngine;

public interface IBoardEvent
{
    //Should broadcast to all users
    public void ProcessMyTurn();
}
