using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class Graph : MonoBehaviour
{
    private bool colorChange = true;
    private List<Node> nodes = new List<Node>();

    private void Awake()
    {
        initializeGraph();
    }

    public List<Node> getNodes(){
        return nodes;
    }

    public Node getNode(int id){
        return nodes[id];
    }
    
    //Initialize graph structure
    private void initializeGraph()
    {   
        Transform[] pathTransforms = GetComponentsInChildren<Transform>();

        StreamReader reader = new StreamReader("Assets/Scripts/FilesToLoad/nodes.txt", true);
        StreamReader greenLightsReader = new StreamReader("Assets/Scripts/FilesToLoad/greenLights.txt", true);
        StreamReader redLightsReader = new StreamReader("Assets/Scripts/FilesToLoad/redLights.txt", true);

        List<int> greenLights = new List<int>();
        List<int> redLights = new List<int>();

        int nodeIndexAux;
        while (!greenLightsReader.EndOfStream)
        {
            string[] line = greenLightsReader.ReadLine().Split(',');
            foreach(string str in line)
            {
                int.TryParse(str, out nodeIndexAux);
                greenLights.Add(nodeIndexAux);
            }
        }
        while (!redLightsReader.EndOfStream)
        {
            string[] line = redLightsReader.ReadLine().Split(',');
            foreach (string str in line)
            {
                int.TryParse(str, out nodeIndexAux);
                redLights.Add(nodeIndexAux);
            }
        }

        int nodeIndex = 0;
        int connectionIndex;

        while (!reader.EndOfStream)
        {
            Node node;
            if (greenLights.Contains(nodeIndex))
            {
                node = new Node(nodeIndex, pathTransforms[nodeIndex + 1].position, false); //last argument is false because light starts green
            }
            else if(redLights.Contains(nodeIndex))
            {
                node = new Node(nodeIndex, pathTransforms[nodeIndex + 1].position, true); //last argument is true because light starts red
            }
            else
            {
                node = new Node(nodeIndex, pathTransforms[nodeIndex + 1].position); // +1 because first element is parent element
            }
            

            string[] line = reader.ReadLine().Split(',');
            foreach(string str in line)
            {
                int.TryParse(str, out connectionIndex);
                node.addConnection(connectionIndex);
            }
            nodes.Add(node);
            nodeIndex += 1;
        }
    }
    
    private void OnDrawGizmos()
    {
        foreach(Node node in nodes)
        {   
            Vector3 currentNode = node.getPosition();
            for (int i = 0; i < node.getConnections().Count; i++) // id in node.getConnections())
            {
                Vector3 nextNode = getNode(node.getConnections()[i]).getPosition();

                Gizmos.color = Color.green;
                if (node.getEdgeCostMultipliers()[i] > 1.0f)
                {
                    Gizmos.color = Color.yellow;
                }
                if (node.getEdgeCostMultipliers()[i] > 1.3f)
                {
                    Gizmos.color = Color.red;
                }
                
                // Gizmos.DrawLine(currentNode, nextNode);
                Handles.DrawBezier(currentNode, nextNode, currentNode, nextNode, Gizmos.color, null, 1);
            }
        }
    }
}
