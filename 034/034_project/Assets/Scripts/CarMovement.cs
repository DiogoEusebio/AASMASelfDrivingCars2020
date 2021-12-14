using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CarMovement : MonoBehaviour
{
    public Graph graph;
    public List<Node> path;
    public float staticEdgeCost;
    public float dynamicEdgeCost;
    public int initialSpeed;
    private int startingIndex;
    private int targetIndex;
    private int reachedNodes  = 0;
    private int speed;
    private bool collisionDetected;
    private Color collisionRayColor = Color.green;
    private float forwardOffset = 4.6f;
    private float widthOffset = 1.5f;        /* car width /2 */
    private float rayLenght = 3f;      /* forward ray length   */
    private float diagonalRayLenght = 5f; /* diagonal ray length */

    private bool deleteOnEnd;
    private bool ended = false;
    private bool canDrive = false;
    private bool stopped = false;
    private bool finalPrint = true;
    private int cameFrom;
    private bool updateEdgeCostOnStart = true;
    private bool updateEdgeCostOnEnd = false;

    // RANDOM PATH VARIABLES
    public bool randomPath;
    private bool nextRandomNode = false;
    private Node nodeRamdomPath;
    private Node nextNodeRandomPath;

    /*AGENT PROPERTIES*/
    private float gas = 0.0f;
    private float initialTime;
    private float endTime;
    private float tripTime;
    private float distance;
    public float gasMultiplier;
    public int passengerCap;

    public void setStartingIndex(int id){
        startingIndex = id;
    }

    public void setTargetIndex(int id){
        targetIndex = id;
    }

    public void setDeleteOnEnd(bool del){
        deleteOnEnd = del;
    }

    public void setCanDrive(bool can){
        canDrive = can;
    }

    public bool canDelete(){
        return ended;
    }

    public void setupMovement(int start, int target, bool delete){
        setStartingIndex(start);
        setTargetIndex(target);
        setDeleteOnEnd(delete);
        transform.position = graph.getNode(start).getPosition();
        if(!randomPath)
        {
            dijkstra(start);
            path = reversePath(target);
            initialTime = Time.time;
            foreach(Node n in graph.getNodes())
            {
                if (n.getConnections().Contains(start))
                {
                    cameFrom = n.getIndex();
                }
            }
        }
        else
        {
            nodeRamdomPath = graph.getNode(start);
        }
    }

    // Update is called once per frame
    void Update()
    {        
        if (canDrive)
        {
            if (path != null && randomPath == false)
            {

                /*UPDATING PROPERTIES VALUES TROUGHOUT EXECUTION*/
                if (stopped && speed == 0)
                {
                    gas += 1 * Time.deltaTime * gasMultiplier;
                }
                if (stopped && speed == initialSpeed)
                {
                    stopped = false;
                    gas += 8 * Time.deltaTime * gasMultiplier;
                }
                if (!stopped && speed == initialSpeed)
                {
                    gas += 5 * Time.deltaTime * gasMultiplier;
                }
                /*--------------*/

                if (reachedNodes < path.Count - 1)
                {
                    Node previousNode;
                    Node currentNode = path[reachedNodes];
                    Node nextNode = path[reachedNodes + 1];

                    if (reachedNodes == 0)
                    {
                        previousNode = graph.getNode(cameFrom);
                    }
                    else
                    {
                        previousNode = path[reachedNodes - 1];
                    }
                    if (updateEdgeCostOnStart)
                    { 
                        if (currentNode.getConnections().Count > 1)
                        //we want to recalculate path
                        {
                            dijkstra(currentNode.getIndex());
                            path = reversePath(targetIndex);
                            reachedNodes = 0;
                            nextNode = path[reachedNodes+1];
                        }
                        updateEdgeCost(currentNode, nextNode, dynamicEdgeCost);
                        updateEdgeCostOnStart = false;
                        updateEdgeCostOnEnd = true;
                    }
                    // Debug.Log(currentNode.getEdgeCostMultipliers()[0]);



                    // In case of collision (distance of rayLenght units) -> speed = 0
                    if (Physics.Raycast(transform.position + transform.right * widthOffset, transform.forward, rayLenght) ||
                        Physics.Raycast(transform.position - transform.right * widthOffset, transform.forward, rayLenght))
                    {
                        //Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + 6));
                        speed = 0;
                        collisionDetected = true;
                    } // Keep on going 
                    else
                    {
                        collisionDetected = false;
                        if (currentNode.getHasLight())
                        {
                            if (Physics.Raycast(transform.position, Quaternion.Euler(0, 45, 0) * transform.forward * diagonalRayLenght, rayLenght) ||
                                Physics.Raycast(transform.position, Quaternion.Euler(0, 45, 0) * transform.forward * diagonalRayLenght, rayLenght))
                            {
                                speed = 0;
                            }
                            
                            if (currentNode.isLightRed())
                            {
                                if (Vector3.Distance(transform.position, currentNode.getPosition()) < 0.01f)
                                {
                                    speed = 0;
                                    stopped = true;
                                }
                                else
                                {
                                    speed = initialSpeed;
                                }
                            }
                            else
                            {
                                speed = initialSpeed;
                                transform.LookAt(nextNode.getPosition());
                            }
                        }
                        else
                        {
                            speed = initialSpeed;
                            transform.LookAt(nextNode.getPosition());
                        }
                    }

                    if(speed != 0){
                        transform.position = Vector3.MoveTowards(transform.position, nextNode.getPosition(), speed * Time.deltaTime);
                    }

                    if (Vector3.Distance(transform.position, nextNode.getPosition()) < 0.01f)
                    {
                        //remove current edge cost  if (updateEdgeCostOnStart)
                        if(updateEdgeCostOnEnd)
                        {
                            updateEdgeCost(currentNode, nextNode, -dynamicEdgeCost);
                            updateEdgeCostOnStart = true;
                            updateEdgeCostOnEnd = false;
                        }
                        reachedNodes++;
                    }
                }
                else if (deleteOnEnd)
                {
                    // Debug.Log("ENDING");
                    ended = true;
                    if (finalPrint)
                    {
                        endTime = Time.time;
                        tripTime = endTime - initialTime;
                        // Write each directory name to a file.
                        using (StreamWriter sw = new StreamWriter("Results/TestResults.csv", append: true))
                        {
                            sw.WriteLine(tripTime + ";" + gas + ";" + distance + ";" + passengerCap + ";");
                        }   

                    // Debug.Log("time: " + tripTime + "| gas: " + gas + "| distance: " + distance + "| passengerCap: ");
                    finalPrint = false; //this prevents update to print multiple times for same car
                    }
                }
                else
                {
                    // Debug.Log("STARTING");
                    transform.position = graph.getNode(startingIndex).getPosition();
                    reachedNodes = 0;
                }
            } 
            // RANDOM MODE
            else
            {
                Node currentNode = nodeRamdomPath;
                if(!nextRandomNode)
                {
                    nextNodeRandomPath = graph.getNode(currentNode.getRandomConnection());
                    nextRandomNode = true;
                }
                if (updateEdgeCostOnStart)
                {
                    updateEdgeCost(currentNode, nextNodeRandomPath, dynamicEdgeCost);
                    updateEdgeCostOnStart = false;
                    updateEdgeCostOnEnd = true;
                }
                // Debug.Log(currentNode.getEdgeCostMultipliers()[0]);
                // In case of collision (distance of rayLenght units) -> speed = 0
                if (Physics.Raycast(transform.position + transform.right * widthOffset, transform.forward, rayLenght) ||
                    Physics.Raycast(transform.position - transform.right * widthOffset, transform.forward, rayLenght))
                    //Physics.Raycast(transform.position + transform.right * widthOffset, Quaternion.Euler(0, 30, 0) * transform.forward,  diagonalRayLenght*2) ||
                    //Physics.Raycast(transform.position + transform.right * widthOffset, Quaternion.Euler(0, 45, 0) * transform.forward,  diagonalRayLenght*3) ||
                    //Physics.Raycast(transform.position + transform.right * widthOffset, Quaternion.Euler(0, 65, 0) * transform.forward,  diagonalRayLenght*3))
                {
                    speed = 0;
                    collisionDetected = true;

                } // Keep on going 
                else
                {
                    collisionDetected = false;
                    if (currentNode.getHasLight())
                    {
                        if (currentNode.isLightRed())
                        {
                            if (Vector3.Distance(transform.position, currentNode.getPosition()) < 0.01f)
                            {
                                speed = 0;
                                stopped = true;
                            }
                            else
                            {
                                speed = initialSpeed;
                            }
                        }
                        else
                        {
                            speed = initialSpeed;
                            transform.LookAt(nextNodeRandomPath.getPosition());
                        }
                    }
                    else
                    {
                        speed = initialSpeed;
                        transform.LookAt(nextNodeRandomPath.getPosition());
                    }
                }

                if(speed != 0){
                    transform.position = Vector3.MoveTowards(transform.position, nextNodeRandomPath.getPosition(), speed * Time.deltaTime);
                }

                if (Vector3.Distance(transform.position, nextNodeRandomPath.getPosition()) < 0.01f)
                {
                    //remove current edge cost  if (updateEdgeCostOnStart)
                    if (updateEdgeCostOnEnd)
                    {
                        updateEdgeCost(currentNode, nextNodeRandomPath, -dynamicEdgeCost);
                        updateEdgeCostOnStart = true;
                        updateEdgeCostOnEnd = false;
                    }
                    nodeRamdomPath = nextNodeRandomPath;
                    nextRandomNode = false;
                }
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        if (collisionDetected)
        {
            collisionRayColor = Color.red;
        }
        else
        {
            collisionRayColor = Color.green;
        }
        
        Debug.DrawRay(transform.position + transform.right * widthOffset, transform.forward * rayLenght, collisionRayColor, Time.deltaTime, false);
        Debug.DrawRay(transform.position - transform.right * widthOffset, transform.forward * rayLenght, collisionRayColor, Time.deltaTime, false);
        //Debug.DrawRay(transform.position + transform.right * widthOffset, Quaternion.Euler(0, 30, 0) * transform.forward * diagonalRayLenght*2, collisionRayColor, Time.deltaTime, false);
        //Debug.DrawRay(transform.position + transform.right * widthOffset, Quaternion.Euler(0, 45, 0) * transform.forward * diagonalRayLenght*3, collisionRayColor, Time.deltaTime, false);
        //Debug.DrawRay(transform.position + transform.right * widthOffset, Quaternion.Euler(0, 65, 0) * transform.forward * diagonalRayLenght*3, collisionRayColor, Time.deltaTime, false);
    }

    private int existIn(int id, List<Node> list)
    {
        foreach (Node node in list)
        {
            if (node.getIndex() == id)
            {
                return 1;
            }
        }
        return 0;
    }

    private List<Node> reversePath(int targetIndex)
    {
        List<Node> path = new List<Node>();
        float totalDistance = 0;
        Node node = graph.getNode(targetIndex);
        path.Insert(0, node);
        while (node.getParent() != -1)
        {
            totalDistance = totalDistance + node.getDistance();
            Node parent = graph.getNode(node.getParent());
            parent.addToEdgeCostMultiplier(parent.getSonIndex(node.getIndex()), staticEdgeCost);
            path.Insert(0, parent);
            node = parent;
        }
        distance = totalDistance;
        return path;
    }

    private void dijkstra(int startingIndex)
    {

        List<Node> queue = new List<Node>();

        foreach (Node node in graph.getNodes())
        {
            if (node.getIndex() == startingIndex)
            {
                node.setDistance(0);
            }
            else
            {
                node.setDistance(999999999);
            }
            node.setParent(-1);
            queue.Add(node);
        }

        while (queue.Count != 0)
        {

            // find min distance in queue
            float min = 9999999;
            int minIndex = 0;
            int i = 0;
            foreach (Node node in queue)
            {
                if (node.getDistance() < min)
                {
                    min = node.getDistance();
                    minIndex = i;
                }
                i++;
            }
            // remove node from queue
            Node current = queue[minIndex];
            queue.RemoveAt(minIndex);

            foreach (int connection in current.getConnections())
            {

                Node child = graph.getNode(connection);

                if (existIn(connection, queue) == 1)
                { //only those that are still in Q
                    float edgeMultiplier = 1.0f;
                    for(int j = 0; j < current.getConnections().Count; j++)
                    {
                        if(child.getIndex() == current.getConnections()[j])
                        {
                            edgeMultiplier = current.getEdgeCostMultipliers()[j];
                        }
                    } 
                    float distance = current.getDistance() + Vector3.Distance(current.getPosition(), child.getPosition()) * edgeMultiplier; // change this later
                    if (distance < child.getDistance())
                    {
                        child.setDistance(distance);
                        child.setParent(current.getIndex());
                    }
                }
            }
        }
    }
    public void updateEdgeCost(Node currentNode, Node nextNode, float valueToAdd)
    {
        int costIndex = 0;
        for (int i = 0; i < currentNode.getConnections().Count; i++)
        {
            if (currentNode.getConnections()[i] == nextNode.getIndex())
            {
                costIndex = i;
            }
        }
        currentNode.addToEdgeCostMultiplier(costIndex, valueToAdd);
    }
}
