using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{

    public Graph graph;
    public bool allGreen;
    public float changeTime;
    public float resetTime;
    private float timeLeft;
    private float resetTimeLeft;
    private List<List<int>> trafficList = new List<List<int>>();
    private List<int> greenIndexList = new List<int>();
    private Node greenToChange;
    private Node redToChange;
    // Start is called before the first frame update
    void Start()
    {
        timeLeft = changeTime;
        resetTimeLeft = resetTime;
        // Node that starts green is always the first one!
        trafficList.Add(new List<int>(new int[] { 41, 132, 133}));
        trafficList.Add(new List<int>(new int[] { 37, 44, 100}));
        trafficList.Add(new List<int>(new int[] { 154, 139, 126}));
        trafficList.Add(new List<int>(new int[] { 150, 105, 143, 181}));
        trafficList.Add(new List<int>(new int[] { 2, 96, 93}));
        trafficList.Add(new List<int>(new int[] { 5, 91, 89}));
        trafficList.Add(new List<int>(new int[] { 189, 109, 187, 177}));
        trafficList.Add(new List<int>(new int[] { 191, 184, 205, 203}));
        trafficList.Add(new List<int>(new int[] { 195, 160, 182, 120}));
        trafficList.Add(new List<int>(new int[] { 115, 213, 171, 166}));
        trafficList.Add(new List<int>(new int[] { 60, 57, 27}));
        trafficList.Add(new List<int>(new int[] { 65, 63, 24}));
        trafficList.Add(new List<int>(new int[] { 18, 74, 71}));
        trafficList.Add(new List<int>(new int[] { 15, 77, 80}));
        trafficList.Add(new List<int>(new int[] { 118, 162, 168, 167}));
        // Set greenIndexList
        for (int i = 0; i < 15; i++)
        {
            greenIndexList.Add(0);
        }

        if (allGreen)
        {
            foreach(List<int> lst in trafficList)
            {
                foreach(int node in lst)
                {
                    graph.getNode(node).setColorGreen();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (allGreen)
        {
            //do nothing
        }
        else
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                resetTimeLeft -= Time.deltaTime;

                if (resetTimeLeft < 0)
                {
                    for (int group = 0; group < 15; group++)
                    {
                        int greenIndex = greenIndexList[group];
                        int redIndex;
                        if (greenIndexList[group] + 1 == trafficList[group].Count)
                        {
                            redIndex = 0;
                            greenIndexList[group] = 0;
                        }
                        else
                        {
                            redIndex = greenIndex + 1;
                            greenIndexList[group] += 1;
                        }
                        redToChange = graph.getNode(trafficList[group][redIndex]);
                        redToChange.changeColor();
                    }
                    timeLeft = changeTime;
                    resetTimeLeft = resetTime;
                }
                else
                {
                    for (int group = 0; group < 15; group++)
                    {
                        int greenIndex = greenIndexList[group];
                        greenToChange = graph.getNode(trafficList[group][greenIndex]);
                        greenToChange.changeToRed();
                    }
                }

            }

        }
    }
}
