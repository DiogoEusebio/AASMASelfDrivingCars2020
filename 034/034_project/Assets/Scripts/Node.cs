using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private int index;
    private Vector3 position;
    private List<int> connections = new List<int>();
    private List<float> edgeCostMultipliers = new List<float>();
    private TrafficLight trafficLight;
    private float distance;
    private int parent;
    public bool hasLight;
 
    public Node(int id, Vector3 pos)
    {
        index     = id;
        position = pos;
        hasLight = false;
    }
    public Node(int id, Vector3 pos, bool trafficLightState)
    {
        index     = id;
        position = pos;
        trafficLight = new TrafficLight(trafficLightState, pos);
        hasLight = true;
    }
    public int getIndex()
    {
        return index;
    }

    public Vector3 getPosition()
    {
        return position;
    }
    public int getSonIndex(int sonId)
    {
        for(int i = 0; i < connections.Count; i++)
        {
            if (connections[i] == sonId){
                return i;
            }
        }
        return -1;
    }
    public List<float> getEdgeCostMultipliers()
    {
        return edgeCostMultipliers;
    }
    public void setEdgeCostMultiplier(int id, float value)
    {
        edgeCostMultipliers[id] = value;
    }
    public void addToEdgeCostMultiplier(int id, float value)
    {
        edgeCostMultipliers[id] += value;
    }

    public void addConnection(int node)
    {
        connections.Add(node);
        edgeCostMultipliers.Add(1.0f);
    }
    public List<int> getConnections()
    {
        return connections;
    }
    
    public int getRandomConnection(){
        int random = Random.Range(0, connections.Count);
        return connections[random];
    }

    public void setDistance(float dist){
        distance = dist;
    }

    public float getDistance(){
        return distance;
    }

    public void setParent(int p){
        parent = p;
    }

    public int getParent(){
        return parent;
    }

    public bool getHasLight()
    {
        return hasLight;
    }

    public bool isLightRed()
    {
        return trafficLight.getGameObject().GetComponent<Light>().color.Equals(Color.red);
    }

    public void changeColor()
    {
        trafficLight.changeColor();
    }

    public void changeToRed()
    {
        trafficLight.changeToRed();
    }

    public void changeToGreen()
    {
        trafficLight.changeToGreen();
    }

    public void setColorGreen()
    {
        trafficLight.getGameObject().GetComponent<Light>().color = Color.green;
    }

    public float getEdgeCostMultiplier(int id)
    {
        return edgeCostMultipliers[id];
    }
}
