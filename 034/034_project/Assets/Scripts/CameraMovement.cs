using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject carToFollow;
    public GameObject carsList;
    private int speed = 100;
    private bool followingCar = false;
    private int carToFollowId = 19;
    private List<Transform> carsToFollow;
    private Vector3 posOffset = new Vector3(0.0f, 100.0f, 0.0f);
    public void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Mouse ScrollWheel") * -speed, Input.GetAxis("Vertical"));
        transform.position += movement * speed * Time.deltaTime;
    }

    private void Update()
    {
        //shitty way of iterating through all cars, will cause error when cars are destroyed 
        carsToFollow = new List<Transform>(carsList.GetComponentsInChildren<Transform>());
        if(Input.GetKeyDown(KeyCode.N))
        {
            if(carToFollowId + 6 >= carsToFollow.Count) //go to initial car
            {
                carToFollowId = 19;
            }
            else
            {
                carToFollowId += 6; //number of child components of each car
            }
            carToFollow = carsToFollow[carToFollowId].gameObject;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (followingCar)
            {
                followingCar = false;
            }
            else
            {
                followingCar = true;
            }
        }
        if (followingCar)
        {
            transform.position = carToFollow.transform.position + posOffset;
        }
        else
        {
            Move();
        }
        
    }
}
