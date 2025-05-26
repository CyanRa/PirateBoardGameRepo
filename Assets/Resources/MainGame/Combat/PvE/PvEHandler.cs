using System;
using System.Collections;
using System.Collections.Generic;
using Alteruna;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PvEHandler : MonoBehaviour
{
    [SerializeField] Button InitiatePvECombatButton;
    int myTotalPower;
    int enemyPower;
    List<int> enemyRolls = new List<int>();
    public Sprite victorySprite;
    public Sprite failureSprite;
    bool cardsSelected = false;
    public FleetManager myFleet;
    private Ship ship;
    [SerializeField] List<Sprite> diceSides;
    [SerializeField] List<Sprite> enemySprites;
    List<GameObject> Dice = new List<GameObject>();
    public GameObject DicePrefab;
    public GameObject PvEResultDisplayPanel;
    private string victoryString;
    private string failureString;
    public bool done = false;
    MapPieceBehaviour map;
    



    public void InitiateEncounter(string enemy, Ship _ship, MapPieceBehaviour _map)
    {
        ship = _ship;
        map = _map;
        Dice.Clear();
        enemyRolls.Clear();
        switch (enemy)
        {
            case "Sirens":

                victoryString = "You cut of the sharks' fins and let your men have their way with still warm remains. This display of relentless barbarism convinces the stranded ship to join you. You also gain like 2 victory points or something";
                failureString = "The sharks overwhelm your men. In hindsight diving into the water to have honourable combat was not a great idea. Your ship takes 1 point of damage";
                GameObject Die = Instantiate(DicePrefab);
                transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = enemySprites[0];
                Die.transform.SetParent(transform.GetChild(2).GetChild(1));
                Die.GetComponent<Image>().sprite = diceSides[5];
                Dice.Add(Die);
                enemyRolls.Add(6);
                break;
            default: break;
        }
    }

    public void Confirm()
    {
        done = true;
    }

    IEnumerator WaitForPlayerCardSelect()
    {
        while (!cardsSelected)
        {
            yield return null;
        }

    }

    public void LockInCardsAndStartCombat()
    {
        foreach (Transform Card in transform.GetChild(0))
        {
            CMBehaviour _card = Card.GetComponent<CMBehaviour>();
            if (_card.isSelected)
            {
                myTotalPower += _card.crewMember.crewMemberPower;
                myFleet.gameObject.GetComponent<Hand>().myFleetCrew.Remove(_card.crewMember);
            }
            else
            {
                Destroy(_card.gameObject);
            }
        }
        cardsSelected = true;
        RollMonsterDamage();
    }

    private void RollMonsterDamage()
    {
        int index = 0;
        foreach (int roll in enemyRolls)
        {

            int _roll = UnityEngine.Random.Range(1, roll);
            enemyPower += _roll;
            StartCoroutine(DisplayRoll(_roll, index));
            index++;
        }
    }
    IEnumerator DisplayRoll(int roll, int index)
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject rollingDie = transform.GetChild(2).GetChild(1).GetChild(index).gameObject;
            rollingDie.GetComponent<Image>().sprite = diceSides[UnityEngine.Random.Range(0, 5)];
            yield return new WaitForSeconds(0.1f);
        }

        transform.GetChild(2).GetChild(1).GetChild(index).gameObject.GetComponent<Image>().sprite = diceSides[roll - 1];
        yield return new WaitForSeconds(3f);

        StartCoroutine(DisplayResultScreen());
    }

    IEnumerator DisplayResultScreen()
    {
        PvEResultDisplayPanel.SetActive(true);
        if (myTotalPower >= enemyPower)
        {
            myFleet.GetVictoryPoints(2);
            if(myFleet.myShips.Count < 5){
                ship.GetComponentInParent<FleetManager>().MainSpawner.SpawnShip(ship.occupyingMapPiece.gameObject.transform.GetChild(0), true);
            }
            map.RemoveSirens();
            PvEResultDisplayPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = victoryString;
            PvEResultDisplayPanel.transform.GetChild(1).GetComponent<Image>().sprite = victorySprite;
        }
        else
        {
            ship.ChangeShipHealth(1);
            PvEResultDisplayPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = failureString;
            PvEResultDisplayPanel.transform.GetChild(1).GetComponent<Image>().sprite = failureSprite;
        }
        while (!done)
        {
            yield return null;
        }
        done = false;
        PvEResultDisplayPanel.SetActive(false);
        PurgeAndCloseThePvECombatPanel();
    }

    private void PurgeAndCloseThePvECombatPanel()
    {
        myTotalPower = 0;
        enemyPower = 0;
        foreach (Transform child in transform.GetChild(2).GetChild(1)) {
            Destroy(child.gameObject);
        }       
        enemyRolls.Clear();
        gameObject.SetActive(false);
    }
}
