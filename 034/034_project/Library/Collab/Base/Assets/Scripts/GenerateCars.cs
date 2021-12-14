using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCars : MonoBehaviour
{
    public GameObject carsList;
    public int carQuantity;
    public Graph graph;
    private int startingIndex;
    private int targetIndex;
    private List<Node> path;
    private int ncarsCreated = 0;
    private Transform[] originalCars; //this will contain all childs of carsList, including prefabs (which we dont want)
    private List<Transform> ogCars = new List<Transform>();
    private GameObject currentCar;
    private List<Transform> carsGenerated = new List<Transform>();
    private List<Transform> carsToDelete = new List<Transform>();
    public bool deleteOnEnd = false; // change to allow deleteOnEnd (public to allow changing that setting on the unity object inspector)
    public float spawnSpeed;

    // Start is called before the first frame update
    void Start()
    {
        originalCars = carsList.GetComponentsInChildren<Transform>();

        foreach (Transform obj in originalCars)
        {
            if (obj.tag == "Car")
            {
                ogCars.Add(obj);
            }
        }

        targetIndex = 98;//Random.Range(1, 216);
        StartCoroutine(Generate());
        StartCoroutine(Delete());
    }

    IEnumerator Generate()
    {
        while (ncarsCreated < carQuantity)
        {
            //randomize Starting and Target positions
            startingIndex = Random.Range(1, 216);
            int carType = 2;
            currentCar = Instantiate(ogCars[carType].gameObject, graph.getNode(startingIndex).getPosition(), Quaternion.identity, carsList.transform);
            currentCar.GetComponent<CarMovement>().setupMovement(startingIndex, targetIndex, deleteOnEnd);
            currentCar.GetComponent<CarMovement>().setCanDrive(true);
            carsGenerated.Add(currentCar.transform);

            ncarsCreated += 1;
            yield return new WaitForSeconds(spawnSpeed); // Waits for 1 second before running Coroutine again
        }
    }

    IEnumerator Delete()
    {
        // Checks if ended
        while (true)
        {
            if (deleteOnEnd)
            {
                foreach (Transform obj in carsGenerated)
                {
                    if (obj.GetComponent<CarMovement>().canDelete()) // Same as "Has car ended path"
                    {
                        carsToDelete.Add(obj);
                    }
                }

                foreach (Transform obj in carsToDelete)
                {
                    carsGenerated.Remove(obj);
                    Destroy(obj.gameObject);
                }
                carsToDelete = new List<Transform>(); // reset list

            }
            yield return new WaitForSeconds(0.1f); // Waits for 1 second before running Coroutine again
        }
    }

}
