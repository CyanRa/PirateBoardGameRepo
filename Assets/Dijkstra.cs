using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.InputSystem;
using Unity.Collections.LowLevel.Unsafe;
using Alteruna;


public class Dijkstra : MonoBehaviour
{   

    [SerializeField]Dictionary<MapPieceBehaviour, MapPieceBehaviour> Predecessors = new Dictionary<MapPieceBehaviour, MapPieceBehaviour>();
    [SerializeField]Dictionary<MapPieceBehaviour, int> MinHeapQueue = new Dictionary<MapPieceBehaviour, int>();
    List<MapPieceBehaviour> nodesToProcess = new List<MapPieceBehaviour>();
    List<MapPieceBehaviour> savedNodes = new List<MapPieceBehaviour>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(Transform child in transform){
            savedNodes.Add(child.GetComponent<MapPieceBehaviour>());
        }
        nodesToProcess = savedNodes.ToList();
        List<MapPieceBehaviour> list = CalculateShortestPathDijkstra(transform.GetChild(0).GetComponent<MapPieceBehaviour>(), transform.GetChild(40).GetComponent<MapPieceBehaviour>());
        foreach(MapPieceBehaviour map in list){
            Debug.Log(map.name);
        }

    }

    public List<MapPieceBehaviour> CalculateShortestPathDijkstra(MapPieceBehaviour startPiece, MapPieceBehaviour endPiece){
        //Initialize mean-heap queueu from start node neighbours r = 2
        Predecessors.Clear();
        MinHeapQueue.Clear();
        nodesToProcess = savedNodes.ToList();

        foreach(MapPieceBehaviour connectedNode in startPiece.neighboringTerrain){
            foreach(MapPieceBehaviour connectedNode2 in connectedNode.neighboringTerrain){
                if(!MinHeapQueue.ContainsKey(connectedNode2)){
                    MinHeapQueue.Add(connectedNode2, ConnectedNodeValue(connectedNode2));
                }
                if(!Predecessors.ContainsKey(connectedNode2)){
                    Predecessors.Add(connectedNode2, startPiece);
                }      
            }         
        }
        nodesToProcess.Remove(startPiece);

        while(nodesToProcess.Count != 0){
            foreach(KeyValuePair<MapPieceBehaviour, int> key in MinHeapQueue.ToList()){
            if(nodesToProcess.Contains(key.Key)){
                foreach(MapPieceBehaviour connectedNode in key.Key.neighboringTerrain){
                    foreach(MapPieceBehaviour connectedNode2 in connectedNode.neighboringTerrain){
                        if(!MinHeapQueue.ContainsKey(connectedNode2)){
                            MinHeapQueue.Add(connectedNode2, ConnectedNodeValue(connectedNode2,MinHeapQueue[key.Key]));
                            Predecessors[connectedNode2] = key.Key;
                        }else if(MinHeapQueue.ContainsKey(connectedNode2)){
                            if(MinHeapQueue[key.Key] + ConnectedNodeValue(connectedNode2) < MinHeapQueue[connectedNode2]){
                                MinHeapQueue[connectedNode2] = ConnectedNodeValue(connectedNode2,MinHeapQueue[key.Key]);
                                Predecessors[connectedNode2] = key.Key;
                            }
                        }else{
                        }
                    }         
                }
                nodesToProcess.Remove(key.Key);
            }
        }
        }
        
        
        //var keyR = MinHeapQueue.Min(kvp => kvp.Value);
        //var myKey = MinHeapQueue.FirstOrDefault(x => x.Value == keyR).Key;
        //ProcessNode(MinHeapQueue.FirstOrDefault(x => x.Value == keyR).Key);

        
        return FormKnotListToReturn(endPiece, startPiece);

    }

    private List<MapPieceBehaviour> FormKnotListToReturn(MapPieceBehaviour endMapPiece, MapPieceBehaviour startMapPiece)
    {
        List<MapPieceBehaviour> KnotListToEndPiece = new List<MapPieceBehaviour>();
        KnotListToEndPiece.Add(endMapPiece);
        
        
        do{
            KnotListToEndPiece.Add(Predecessors[endMapPiece]);
            endMapPiece = Predecessors[endMapPiece];
            
        }while(endMapPiece != startMapPiece);
         
        
        return KnotListToEndPiece;
    }


    //Assigns value of 2 to hostile territories, and 1 to all other
    private int ConnectedNodeValue(MapPieceBehaviour map)
    {
        if(map.myMapStatus == MapPieceBehaviour.MapStatus.Hostile){
            return 2;
        }else{
            return 1;
        }
    }

    private int ConnectedNodeValue(MapPieceBehaviour map, int minheapqueueint)
    {
        if(map.myMapStatus == MapPieceBehaviour.MapStatus.Hostile){
            return minheapqueueint + 2;
        }else{
            return minheapqueueint + 1;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
