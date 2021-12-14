using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    bool hasToStop;
    GameObject light;

    public TrafficLight(bool initialValue, Vector3 pos)
    {
        hasToStop = initialValue;
        // Make a game object
        light = new GameObject("TrafficLight");

        // Add the light component
        Light lightComp = light.AddComponent<Light>();

        // Set color
        if (hasToStop)
        {
            lightComp.color = Color.red;
        }
        else
        {
            lightComp.color = Color.green;
        }
        //Set intensity
        lightComp.intensity = 10;
        // Set the position
        light.transform.position = pos + Vector3.up;
    }

    public GameObject getGameObject()
    {
        return light;
    }

    public void changeColor()
    {
        Light aux = light.GetComponent<Light>();

        if (aux.color.Equals(Color.red))
        {
            aux.color = Color.green;
        } else
        {
            aux.color = Color.red;
        }
    }

    public void changeToGreen()
    {
        Light aux = light.GetComponent<Light>();
        aux.color = Color.green;
    }

    public void changeToRed()
    {
        Light aux = light.GetComponent<Light>();
        aux.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
