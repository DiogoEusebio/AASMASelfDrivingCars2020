using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public int initialSpeed;
    public Graph graph;
    private int startingIndex;
    private int targetIndex;
    private int reachedNodes  = 0;
    public List<Node> path;
    private int speed;
    private bool collisionDetected;
    private Color collisionRayColor = Color.green;
    private float forwardOffset = 6.6f;
    private float widthOffset = 2.5f;        /* car width /2 */
    private float rayLenght = 5.5f;      /* ray length   */

    private bool deleteOnEnd;
    private bool ended = false;

    public void setStartingIndex(int id){
        startingIndex = id;
    }

    public void setTargetIndex(int id){
        targetIndex = id;
    }

    public void setDeleteOnEnd(bool del){
        deleteOnEnd = del;
    }

    public bool canDelete()
    {
        return ended;
    }

    public void setupMovement(int start, int target, bool delete){
        setStartingIndex(start);
        setTargetIndex(target);
        setDeleteOnEnd(delete);
        transform.position = graph.getNode(start).getPosition();
        dijkstra(start);
        path = reversePath(target);
    }

    // Start is called before the first frame update
    void Start()
    {
        //adjust ray position for car model
        forwardOffset = transform.GetComponent<BoxCollider>().center.z + transform.GetComponent<BoxCollider>().size.z / 2 + 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(path != null){
            if(reachedNodes < path.Count - 1){

                Node currentNode = path[reachedNodes];
                Node nextNode = path[reachedNodes+1];

                // In case of collision (distance of rayLenght units) -> speed = 0
                if (Physics.Raycast(transform.position + transform.forward * forwardOffset + transform.right * widthOffset, transform.forward * forwardOffset, rayLenght) || 
                    Physics.Raycast(transform.position + transform.forward * forwardOffset - transform.right * widthOffset, transform.forward * forwardOffset, rayLenght)){
                    //Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y, transform.position.z + 6));
                    speed = 0;
                    collisionDetected = true;
                } // Keep on going 
                else {
                    collisionDetected = false;
                    if (currentNode.getHasLight())
                    {
                        if (currentNode.isLightRed())
                        {
                            speed = 0;
                        } else
                        {
                            speed = initialSpeed;
                        }
                    } else
                    {
                        speed = initialSpeed;
                    }
                }

                transform.position = Vector3.MoveTowards(transform.position, nextNode.getPosition(), speed * Time.deltaTime);
                transform.LookAt(nextNode.getPosition());
                if (Vector3.Distance(transform.position, nextNode.getPosition()) < 0.01f)
                {
                    //Debug.Log("Reached Node " + nextNode.getPosition());
                    reachedNodes++;
                }
            } else if (deleteOnEnd)
            {
                ended = true;
            }
            else
            {
                transform.position = graph.getNode(startingIndex).getPosition();
                reachedNodes = 0;
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
        
        Debug.DrawRay(transform.position + transform.forward * forwardOffset + transform.right * widthOffset, transform.forward * rayLenght, collisionRayColor, Time.deltaTime, false);
        Debug.DrawRay(transform.position + transform.forward * forwardOffset - transform.right * widthOffset, transform.forward * rayLenght, collisionRayColor, Time.deltaTime, false);
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
            path.Insert(0, parent);
            node = parent;
        }
        //Debug.Log("Total distance: " + totalDistance);
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
                    float distance = current.getDistance() + Vector3.Distance(current.getPosition(), child.getPosition()); // change this later
                    if (distance < child.getDistance())
                    {
                        child.setDistance(distance);
                        child.setParent(current.getIndex());
                    }
                }
            }
        }
    }
}
