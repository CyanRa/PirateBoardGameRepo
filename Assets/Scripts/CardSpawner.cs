using System;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;

    public Transform spawnLocation;

    public CrewCard krakenScriptableObject;
    public CrewCard swingingSwashbucklerScriptableObject;
    public CrewCard doubleTeamScriptableObject;
    public CrewCard seaDogScriptableObject;
    public CrewCard plunderBundleScriptableObject;
    public CrewCard fullRaidScriptableObject;

    public Canvas cardCanvas;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnKrakenCard();
        SpawnSwingingSwashbuckler();
        SpawnDoubleTeam();
        SpawnSeaDog();
        SpawnPlunderBundle();
        SpawnFullRaid();
        
    }

    private void SpawnFullRaid()
    {
        GameObject spawnedCard = Instantiate(cardPrefab, spawnLocation.position, Quaternion.identity);

        spawnedCard.GetComponent<CardDisplayScript>().card = fullRaidScriptableObject;

        spawnedCard.name = fullRaidScriptableObject.name;

        spawnedCard.transform.SetParent(cardCanvas.transform);
    }

    private void SpawnPlunderBundle()
    {
        GameObject spawnedCard = Instantiate(cardPrefab, spawnLocation.position, Quaternion.identity);

        spawnedCard.GetComponent<CardDisplayScript>().card = plunderBundleScriptableObject;

        spawnedCard.name = plunderBundleScriptableObject.name;

        spawnedCard.transform.SetParent(cardCanvas.transform);
    }

    private void SpawnSeaDog()
    {
        GameObject spawnedCard = Instantiate(cardPrefab, spawnLocation.position, Quaternion.identity);

        spawnedCard.GetComponent<CardDisplayScript>().card = seaDogScriptableObject;

        spawnedCard.name = seaDogScriptableObject.name;

        spawnedCard.transform.SetParent(cardCanvas.transform);
    }

    public void SpawnDoubleTeam()
    {
        GameObject spawnedCard = Instantiate(cardPrefab, spawnLocation.position, Quaternion.identity);

        spawnedCard.GetComponent<CardDisplayScript>().card = doubleTeamScriptableObject;

        spawnedCard.name = doubleTeamScriptableObject.name;

        spawnedCard.transform.SetParent(cardCanvas.transform);
    }

    public void SpawnSwingingSwashbuckler()
    {
        GameObject spawnedCard = Instantiate(cardPrefab, spawnLocation.position, Quaternion.identity);

        spawnedCard.GetComponent<CardDisplayScript>().card = swingingSwashbucklerScriptableObject;

        spawnedCard.name = swingingSwashbucklerScriptableObject.name;

        spawnedCard.transform.SetParent(cardCanvas.transform);
    }

    public void SpawnKrakenCard()
    {
        GameObject spawnedCard = Instantiate(cardPrefab, spawnLocation.position, Quaternion.identity);

        spawnedCard.GetComponent<CardDisplayScript>().card = krakenScriptableObject;

        spawnedCard.name = krakenScriptableObject.name;

        spawnedCard.transform.SetParent(cardCanvas.transform);
       
    }    
   
}
