using UnityEngine;

using Alteruna;
using Unity.VisualScripting;
using System.Data;
using System;
using UnityEditor;
using RTS_Cam;
using UnityEngine.UIElements;
using System.ComponentModel.Design;
using TMPro;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq.Expressions;
using UnityEngine.EventSystems;
using System.Collections;





public class Ship : AttributesSync
{
    private static LTDescr delay;
    public bool isMoving = false;
    [SynchronizableField]public bool isFlagship = false;
    public MapPieceBehaviour occupyingMapPiece;
    [SynchronizableField] public string occupyingMapPieceName;
    [SynchronizableField] public string myFleetName;
    [SynchronizableField] public int shipGold;
    public TextMeshProUGUI goldDisplay;
    public FleetManager myFleet;
    public RTS_Camera myCamera;
    private Transform mapPieceAnchor;
    public float Speed = 100.0f;
    public LayerMask MovementLayer;
    public Alteruna.Avatar fleetsAvatar;
    public AudioSource myAudioSource;
    public AudioClip selectShipAudioClip;
    public AudioClip shipBellRingAudioClip;
    public int movementPoints;
    public int actionPoints;
    public bool beingTargeted;
    [SynchronizableField] public int healthPoints;
    //First digit for fleet position, Second for ship number position
    public List<int> offsetPosition;
    private GameObject myMovementIcon;
    private GameObject myActionIcon;
    public bool isInsideStorm = false;
    public bool selectingShip;
    public Button button1;
    public bool usingGreekFire;
    [SynchronizableField]public int damageBoost = 0;
    public bool hasRetal = false;

    private bool myBool;

    
    
    private bool playingShipSelectSound = false;

    private void Awake(){
        myCamera = GameObject.Find("RTS_Camera_var1").GetComponent<RTS_Camera>();   
        actionPoints = 1;
        movementPoints = 1;     
        healthPoints = 2;
        shipGold = 0;
        offsetPosition.Add(0);
        offsetPosition.Add(0);    
    }
    public void Start(){
        if(myFleet.myShips.Contains(this.gameObject)){
            myFleetName = myFleet.Multiplayer.GetUser();
        }      
    }

    public void UpdateGoldDisplay(){
        goldDisplay.text = shipGold.ToString();
    }
    public void SpendGold(int _price){
        shipGold -= _price;
        if(Multiplayer.GetAvatar().GetComponent<FleetManager>().myShips.Contains(gameObject)){
            UpdateGoldDisplay();
        }
        UpdateShipDisplayIcon();
        Commit();
    }
    public void GetGold(int _goldAmount){
        shipGold += _goldAmount;
        if(Multiplayer.GetAvatar().GetComponent<FleetManager>().myShips.Contains(gameObject)){
            UpdateGoldDisplay();
        }
        Commit();
    }
    public void SpendActionPoints(int _amountOfActionPoints){
        actionPoints -= _amountOfActionPoints;
        UpdateShipDisplayIcon();
    }

    public void SetMovementIcon(GameObject _icon){
        myMovementIcon = _icon;
    }
    public void SetActionIcon(GameObject _icon){
        myActionIcon = _icon;
    }
    public void UpdateShipDisplayIcon()
    {
        if (movementPoints > 0)
        {
            myMovementIcon.GetComponent<UnityEngine.UI.Image>().color = new Color32(0, 255, 0, 255);
        }
        else
        {
            myMovementIcon.GetComponent<UnityEngine.UI.Image>().color = new Color32(0, 0, 0, 255);
        }

        if (actionPoints > 0)
        {
            myActionIcon.GetComponent<UnityEngine.UI.Image>().color = new Color32(255, 255, 0, 255);
        }
        else
        {
            myActionIcon.GetComponent<UnityEngine.UI.Image>().color = new Color32(0, 0, 0, 255);
        }
        
    }

    private void OnMouseEnter()
    {
        if (selectingShip)
        {
            Debug.Log("Moused over ship: " + name);
            GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);
            DisplaySystem.SetDefDisplay(transform.parent.name, healthPoints.ToString(), shipGold.ToString(), damageBoost.ToString(), GetComponentInParent<FleetManager>().victoryPoints.ToString());
        }
        delay = LeanTween.delayedCall(0.2f, () =>
        {
            TooltipSystem.SetAllignmentTopLeft();
            TooltipSystem.Show(shipGold + " Gold\n" + healthPoints + " HP\n" + damageBoost + " Att\n" + GetComponentInParent<Hand>().myFleetCrewCount + " Crew Cards", myFleetName + " 's Fleet");
        });
        if (myFleet.SelectedShip == null)
        { 
            ActivateSelectedAnimation(true);
        }
        
                    
    }
    private void OnMouseExit()
    {
        if (selectingShip)
        {
            GetComponent<Ship>().ChangeShipColour(transform.GetComponentInParent<FleetManager>().fleetColour);
        }
        LeanTween.cancel(delay.uniqueId);
        TooltipSystem.Hide();
        if (!myFleet.SelectedShip == gameObject) { 
            ActivateSelectedAnimation(false);
        }
        
                
    }
    public void ActivateSelectedAnimation(bool a)
    { if (a)
        {
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        { 
            gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }      
    }
 
    void Update(){
    if(!myFleet.avatar.IsMe) return;
    if(myFleet.choosingStorm) return;
    //Initial ship movement. Will be replaced with spawn                  
        if(occupyingMapPiece == null){
            if (Input.GetMouseButtonDown(1) && !isMoving ){ 
                offsetPosition[0] = myFleet.fleetPositionIndex;       	
		        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit;
		
		        if( Physics.Raycast( ray, out hit, 1000, MovementLayer ) ){		            
                    mapPieceAnchor = hit.transform.GetChild(0).transform;
                    occupyingMapPiece = hit.transform.GetComponent<MapPieceBehaviour>();
                    occupyingMapPieceName = occupyingMapPiece.name;
                    //OccupyMapPiece(true);
                    occupyingMapPiece.EnterMapPiece(GetComponent<Ship>());
                    isMoving = true;
                }    
            }     
        }
    //Movement when unit is selected and is registered to a map piece        
            if (Input.GetMouseButtonDown(1) && !isMoving && occupyingMapPiece != null && movementPoints > 0){ 
                offsetPosition[0] = myFleet.fleetPositionIndex;         	
		        Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit;
                occupyingMapPiece.GetComponent<MapPieceBehaviour>().DeHighlightNeighbours();
		
		        if( Physics.Raycast( ray, out hit, 1000, MovementLayer )){
                    List<MapPieceBehaviour> _mapPieces = new List<MapPieceBehaviour>(); 
                    bool onlyHostileNeighbours = true;
		            foreach(MapPieceBehaviour _map in occupyingMapPiece.neighboringTerrain){                       
                        _mapPieces.AddRange(_map.neighboringTerrain);
                        foreach(MapPieceBehaviour secondMapPiece in _map.neighboringTerrain){
                            foreach(MapPieceBehaviour connectingMapPiece in secondMapPiece.neighboringTerrain){
                                if(occupyingMapPiece.neighboringTerrain.Contains(connectingMapPiece) && connectingMapPiece.myMapStatus != MapPieceBehaviour.MapStatus.Hostile){
                                    onlyHostileNeighbours = false;
                                }
                            }
                            if(onlyHostileNeighbours == true){
                                _mapPieces.Remove(secondMapPiece);                               
                            }
                            onlyHostileNeighbours = true;
                        }
                        
                    }
                    if(occupyingMapPiece.neighboringTerrain.Contains(hit.transform.GetComponent<MapPieceBehaviour>())||_mapPieces.Contains(hit.transform.GetComponent<MapPieceBehaviour>())){  
                        if(occupyingMapPiece == hit.transform.GetComponent<MapPieceBehaviour>())return;                     
                        MoveFromAMapPieceToAMapPiece(hit);
                    }else{
                        myFleet.DeselectAll();
                    }                    
                }
            } 
        
    //Deselection when left clicking
            if(Input.GetMouseButtonDown(0) && !isMoving){              
                Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		        RaycastHit hit;
                occupyingMapPiece?.GetComponent<MapPieceBehaviour>().DeHighlightNeighbours();            
                if( Physics.Raycast( ray, out hit, 1000, MovementLayer )){
                    myFleet.DeselectAll();
                    EnableUnitMovement(this.gameObject, false);
                }                
            }               
    }
    public void InitSpawnMove(Transform initTransform)
    {
        mapPieceAnchor = initTransform;

        occupyingMapPiece = initTransform.GetComponentInParent<MapPieceBehaviour>();
        occupyingMapPieceName = occupyingMapPiece.name;
        occupyingMapPiece.EnterMapPiece(GetComponent<Ship>());
        StartCoroutine(StartShipMovementCo(initTransform));
        //isMoving = true;  

    }

    public void MoveFromAMapPieceToAMapPiece(RaycastHit _hit){
        myFleet.myPointer.BroadCastHidePath();
        occupyingMapPiece.GetComponentInParent<GameEventManager>().shipMoving = true;
        myFleet.MenuController.GetComponent<MenuBehaviour>().ResetInteractablePanel();
        occupyingMapPiece.HandleFleetFormation(myFleet, offsetPosition[1]);
        occupyingMapPiece.BroadCastRemoveOccupyingShip(gameObject.name);
        occupyingMapPiece.HandleMapPieceStatus();
        
        mapPieceAnchor = _hit.transform.GetChild(0).transform;
        occupyingMapPiece = _hit.transform.GetComponent<MapPieceBehaviour>();
        occupyingMapPieceName = occupyingMapPiece.name;
        occupyingMapPiece.EnterMapPiece(GetComponent<Ship>());
        occupyingMapPiece.defenderShip = GetComponent<Ship>();
        isMoving = true;
        gameObject.GetComponent<Ship>().PlayShipBellRingAudioClip();
        movementPoints -= 1;
        UpdateShipDisplayIcon();
        StartCoroutine(StartShipMovementCo(mapPieceAnchor));
        occupyingMapPiece.ResetMaterial();                       
    }
    //For spawning from being bought 
    public void SpawnShipFromHarbour(Transform _hit)
    {
        mapPieceAnchor = _hit.GetChild(0).transform;
        occupyingMapPiece = _hit.GetComponent<MapPieceBehaviour>();
        occupyingMapPieceName = occupyingMapPiece.name;
        occupyingMapPiece.EnterMapPiece(GetComponent<Ship>(), true);
        occupyingMapPiece.defenderShip = GetComponent<Ship>();
        isMoving = true;
        actionPoints -= 1;
        movementPoints -= 1;
        UpdateShipDisplayIcon();
        occupyingMapPiece.ResetMaterial();
        myFleet.MenuController.GetComponent<MenuBehaviour>().ResetInteractablePanel();
        ChangeShipColour(myFleet.fleetColour);
        myFleet.SelectByClicking(gameObject);
        myFleet.DeselectAll();
        StartCoroutine(StartShipMovementCo(mapPieceAnchor));

                       
    }
    public void MoveToAMapPiece(Transform _mapPiece){       
        mapPieceAnchor = _mapPiece.GetChild(0).transform;
        occupyingMapPiece = _mapPiece.transform.GetComponent<MapPieceBehaviour>();
        occupyingMapPieceName = occupyingMapPiece.name;
        occupyingMapPiece.EnterMapPiece(GetComponent<Ship>());
        occupyingMapPiece.defenderShip = GetComponent<Ship>();
        isMoving = true;
        StartCoroutine(StartShipMovementCo(_mapPiece));
        gameObject.GetComponent<Ship>().PlayShipBellRingAudioClip();

    }
    private IEnumerator StartShipMovementCo(Transform anchor)
    {
        Debug.Log("Starter movement coroutine");
        hasRetal = false;
        while(GetComponent<Transform>().position.x != anchor.position.x && GetComponent<Transform>().position.z != anchor.position.z)
        {
            Debug.Log("CO MOVING");
            GetComponent<Transform>().position = Vector3.MoveTowards(GetComponent<Transform>().position, mapPieceAnchor.position, Speed * Time.deltaTime);
            GetComponent<Transform>().forward = mapPieceAnchor.position - GetComponent<Transform>().position;
            yield return null;
        }
               
            isMoving = false;
            OffsetThisShip();
            myFleet.DeselectAll();
            occupyingMapPiece.GetComponentInParent<GameEventManager>().shipMoving = false;
            EnableUnitMovement(this.gameObject, false);
            myCamera.ResetTarget();
            
        
    }

    //TO BE REPLACED WITH SPLINE MOVEMENT
    public void MoveToAnchor(Transform transform)
    {
        hasRetal = false;
        if (GetComponent<Transform>().position.x != transform.position.x && GetComponent<Transform>().position.z != transform.position.z)
        {
            GetComponent<Transform>().position = Vector3.MoveTowards(GetComponent<Transform>().position, mapPieceAnchor.position, Speed * Time.deltaTime);
            GetComponent<Transform>().forward = mapPieceAnchor.position - GetComponent<Transform>().position;
        }
        else
        {
            isMoving = false;
            OffsetThisShip();
            myFleet.DeselectAll();
            occupyingMapPiece.GetComponentInParent<GameEventManager>().shipMoving = false;
            EnableUnitMovement(this.gameObject, false);
            myCamera.ResetTarget();
        }
    }

    
    public void EnableUnitMovement(GameObject unit, bool shouldMove){   
        if(unit.GetComponent<Ship>().occupyingMapPiece != null && shouldMove == true){
            unit.GetComponent<Ship>().occupyingMapPiece.HighlightNeighbours(unit.GetComponent<Ship>());
        }

        if(unit.GetComponent<Ship>().occupyingMapPiece != null && shouldMove == false){
            unit.GetComponent<Ship>().occupyingMapPiece.DeHighlightNeighbours();
        }
        unit.GetComponent<Ship>().enabled = shouldMove;
    }

    public void ChangeShipHealth(int damage){
        myFleet = GetComponentInParent<FleetManager>();
        healthPoints -= damage;
        BroadcastRemoteMethod("CheckShipStatus");

    }

    [SynchronizableMethod]
    private void CheckShipStatus()
    {

        if (occupyingMapPiece == null)
        {
            occupyingMapPiece = GameObject.Find(occupyingMapPieceName)?.GetComponent<MapPieceBehaviour>();
        }
        if (healthPoints < 1)
        {
            if (GetComponentInParent<FleetManager>().myShips.Contains(gameObject))
            {
                RemoveShipFromFleetPanel();
                myFleet.myShips.Remove(gameObject);
            }
            occupyingMapPiece?.BroadCastRemoveOccupyingShip(name);
            Destroy(transform.gameObject);
        }
        if (healthPoints == 1)
        {
            GetComponentInChildren<ParticleSystem>().Play();
        }
        if (healthPoints == 2) { 
            GetComponentInChildren<ParticleSystem>().Stop();
        }
    }

    private void RemoveShipFromFleetPanel()
    {
        if(!myFleet.myShips.Contains(gameObject))return;
        Destroy(myFleet.MenuController.GetComponent<MenuBehaviour>().FleetPanel.transform.GetChild(myFleet.myShips.IndexOf(gameObject)).gameObject);    
    }

    //SELECTING SHIPS FROM FLEET PANEL ICONS
    public void SelectShipFromItsIcon(GameObject shipToSelect){
        if(myFleet.Multiplayer.Me.Name == myFleet.MenuController.GetComponent<MenuBehaviour>().turnOwner){
            MoveCameraFromIconSelection(shipToSelect);
            myFleet.SelectByClicking(shipToSelect);
            shipToSelect.GetComponent<Ship>().StartCoroutine(Co_PlaySelectShipAudioClip()); 
        }                   
    }
    public IEnumerator Co_PlaySelectShipAudioClip(){
        if(playingShipSelectSound == false){
            playingShipSelectSound = true;
            myAudioSource.PlayOneShot(selectShipAudioClip);
            yield return new WaitForSeconds(1.5f);
            playingShipSelectSound = false;
        }
    }
    public void PlayShipBellRingAudioClip(){
        myAudioSource.PlayOneShot(shipBellRingAudioClip);
    }
    //NEEDS SOME BUFFER 
    private void MoveCameraFromIconSelection(GameObject shipToSelect){
        myCamera.transform.position = new Vector3(shipToSelect.transform.position.x+7, 535, shipToSelect.transform.position.z+7);    
        myCamera.transform.LookAt(shipToSelect.transform);
    }

    public void BroadcastChangeShipColour(int tempColourID){
        BroadcastRemoteMethod("ChangeShipColour", tempColourID);       
    }

    [SynchronizableMethod]
    public void ChangeShipColour(string tempColour){
        Renderer tempRenderer = gameObject.GetComponent<Renderer>();
       
        switch(tempColour){
            case "Red": tempRenderer.material.SetColor("_BaseColor", Color.red); break;
            case "Blue": tempRenderer.material.SetColor("_BaseColor", Color.blue); break;
            case "Green": tempRenderer.material.SetColor("_BaseColor", Color.green); break;
            case "Yellow": tempRenderer.material.SetColor("_BaseColor", Color.yellow); break;
            default:print("Something went wrong choosing colour"); break;
            }
    }


    public void OffsetThisShip(){
        if(offsetPosition[0] == 0){
            switch(offsetPosition[1]){
                case 0:
                    transform.position = new UnityEngine.Vector3(transform.position.x,transform.position.y,transform.position.z -1.5f);
                break;
                case 1:
                    transform.position = new UnityEngine.Vector3(transform.position.x -0.7f,transform.position.y,transform.position.z -3);
                break;
                case 2:
                    transform.position = new UnityEngine.Vector3(transform.position.x +0.7f,transform.position.y,transform.position.z -3);
                break;
                case 3:
                    transform.position = new UnityEngine.Vector3(transform.position.x ,transform.position.y,transform.position.z -6);
                break;
                case 4:
                    transform.position = new UnityEngine.Vector3(transform.position.x +1.4f,transform.position.y,transform.position.z -6);
                break;
                default:
                Debug.Log("ERROR OFFSETTING SHIP");
                break;
            }
        }else if(offsetPosition[0] == 1){
            transform.Rotate(0,180,0);
            switch(offsetPosition[1]){
                case 0: 
                    transform.position = new UnityEngine.Vector3(transform.position.x ,transform.position.y,transform.position.z+1.5f );
                break;
                case 1:
                    transform.position = new UnityEngine.Vector3(transform.position.x -0.7f,transform.position.y,transform.position.z +3);
                break;
                case 2:
                    transform.position = new UnityEngine.Vector3(transform.position.x +0.7f,transform.position.y,transform.position.z +3);
                break;
                case 3:
                    transform.position = new UnityEngine.Vector3(transform.position.x ,transform.position.y,transform.position.z +6);
                break;
                case 4:
                    transform.position = new UnityEngine.Vector3(transform.position.x -2,transform.position.y,transform.position.z +6);
                break;
                default:
                Debug.Log("ERROR OFFSETTING SHIP");
                break;
            }    
        }else if(offsetPosition[0] == 2){
            transform.Rotate(0,90,0);
            switch(offsetPosition[1]){
                case 0: 
                    transform.position = new UnityEngine.Vector3(transform.position.x-1.5f ,transform.position.y,transform.position.z);
                break;
                case 1:
                    transform.position = new UnityEngine.Vector3(transform.position.x-3 ,transform.position.y,transform.position.z-0.7f );
                break;
                case 2:
                    transform.position = new UnityEngine.Vector3(transform.position.x-3,transform.position.y,transform.position.z+0.7f );
                break;
                case 3:
                    transform.position = new UnityEngine.Vector3(transform.position.x-6 ,transform.position.y,transform.position.z );
                break;
                case 4:
                    transform.position = new UnityEngine.Vector3(transform.position.x-6 ,transform.position.y,transform.position.z+2 );
                break;
                default:
                Debug.Log("ERROR OFFSETTING SHIP");
                break;
            }    
        }else if(offsetPosition[0] == 3){
            transform.Rotate(0,270,3);
            switch(offsetPosition[1]){
                case 0: 
                    transform.position = new UnityEngine.Vector3(transform.position.x+1.5f ,transform.position.y,transform.position.z );
                break;
                case 1:
                    transform.position = new UnityEngine.Vector3(transform.position.x+3,transform.position.y,transform.position.z-0.7f );
                break;
                case 2:
                    transform.position = new UnityEngine.Vector3(transform.position.x+3,transform.position.y,transform.position.z+0.7f );
                break;
                case 3:
                    transform.position = new UnityEngine.Vector3(transform.position.x+6 ,transform.position.y,transform.position.z );
                break;
                case 4:
                    transform.position = new UnityEngine.Vector3(transform.position.x+6,transform.position.y,transform.position.z-2 );
                break;
                default:
                Debug.Log("ERROR OFFSETTING SHIP");
                break;
            }    
        }
    }

    //WAITING FOR DEFENDER TO SEND BACK ACTION
    public void WaitForDefenderShipReaction(int attackerID, string attackerName){
        myFleet = GetComponentInParent<FleetManager>();
        InvokeRemoteMethod("AskOwnerForAction", myFleet.avatar.Owner.Index, attackerID, attackerName);           
    }

    [SynchronizableMethod]
    private void AskOwnerForAction(int attackerID, string attackerName)
    {
        myFleet.MenuController.GetComponent<MenuBehaviour>().DisplayInfoTab();
        Ship attackingShip = GameObject.Find(attackerName).GetComponent<Ship>();
        DisplaySystem.SetAttDisplay(attackingShip.transform.parent.name, attackingShip.healthPoints.ToString(), attackingShip.shipGold.ToString(), attackingShip.damageBoost.ToString(), attackingShip.GetComponentInParent<FleetManager>().victoryPoints.ToString());
        DisplaySystem.SetDefDisplay(transform.parent.name, healthPoints.ToString(), shipGold.ToString(), damageBoost.ToString(), GetComponentInParent<FleetManager>().victoryPoints.ToString());

        GameObject tempPanel;
        tempPanel = myFleet.MenuController.GetComponent<MenuBehaviour>().defendingShipOptionsPanel;
        tempPanel.SetActive(true);
        UnityEngine.UI.Button tempButton = tempPanel.transform.GetChild(0).GetComponent<UnityEngine.UI.Button>();
        tempButton.onClick.RemoveAllListeners();
        tempButton.onClick.AddListener(() => InvokeStartCombat(attackerName, name, attackerID, tempButton));
        tempButton.onClick.AddListener(() => CloseDecisionPanel(tempButton));
        bool a = false;

        tempButton = tempPanel.transform.GetChild(1).GetComponent<UnityEngine.UI.Button>();
        tempButton.onClick.RemoveAllListeners();
        foreach (Consumable consumable in GetComponentInParent<Inventory>().myConsumables)
        {
            if (consumable.consumableIndex == 2 && isFlagship)
            {
                tempButton.onClick.AddListener(() => UsePassageInDefence(tempButton, consumable, (ushort)attackerID));
                tempButton.onClick.AddListener(() => CloseDecisionPanel(tempButton));
                tempButton.gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f);
                a = true;
            }
        }
        if (!a)
        {
            tempButton.gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color(96f / 255, 79f / 255, 58f / 255);

        }
        else
        {
            a = false;
        }

        tempButton = tempPanel.transform.GetChild(2).GetComponent<UnityEngine.UI.Button>();
        tempButton.onClick.RemoveAllListeners();
        foreach (Consumable consumable in GetComponentInParent<Inventory>().myConsumables)
        {
            if (consumable.consumableIndex == 4)
            {
                tempButton.onClick.AddListener(() => UseGreekFireInDefence(tempButton, consumable, (ushort)attackerID));
                tempButton.onClick.AddListener(() => CloseDecisionPanel(tempButton));
                tempButton.gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f);
                a = true;

            }
        }
        if (!a)
        {
                tempButton.gameObject.GetComponent<UnityEngine.UI.Image>().color = new Color(96f / 255, 79f / 255, 58f / 255);
        }       
    }
    [SynchronizableMethod]
    private void DeselectAfterCombatPrevention(ushort attackerID){
        Multiplayer.GetAvatar(attackerID).GetComponent<FleetManager>().DeselectAll();
    }
    private void RemoveAllListeners(){
        
    }

    [SynchronizableMethod]
    private void StartCombat(string attacker, string defender){
        Multiplayer.GetAvatar().GetComponent<FleetManager>().EnterCombat(attacker, defender);
    }
    
    private void InvokeStartCombat(string attacker, string defender, int attackerID, UnityEngine.UI.Button button){
        GetComponentInParent<FleetManager>().InitDefenderBattleManager();
        InvokeRemoteMethod("StartCombat", (ushort)attackerID,attacker,defender);
        button.onClick.RemoveAllListeners();
    }
    private void UsePassageInDefence(UnityEngine.UI.Button button, Consumable consumable, ushort attID){
        consumable.UseConsumable(GetComponentInParent<FleetManager>());
        InvokeRemoteMethod("DeselectAfterCombatPrevention", attID, attID);
    }
    private void UseGreekFireInDefence(UnityEngine.UI.Button button, Consumable consumable, ushort attID){
        usingGreekFire = true;
        consumable.UseConsumable(GetComponentInParent<FleetManager>());
        InvokeRemoteMethod("DeselectAfterCombatPrevention", attID, attID);
    }

    private void CloseDecisionPanel(UnityEngine.UI.Button button){
        GameObject tempPanel;
        tempPanel = myFleet.MenuController.GetComponent<MenuBehaviour>().defendingShipOptionsPanel;
        button.onClick.RemoveAllListeners();
        tempPanel.SetActive(false);
    }

  
}
