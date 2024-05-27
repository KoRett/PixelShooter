using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling : MonoBehaviour
{


    public float paralaxSpeedX;
    public float paralaxSpeedY;
    public float backgroundSize;

    private Transform cameraTransform;
    public Transform[] layers;
    private float viewZone = 10f;
    private int leftIndex;
    private int rightIndex;

    private float lastCameraX;
    private float lastCameraY;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraX = cameraTransform.position.x;

        layers = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
        }
        leftIndex = 0;
        rightIndex = layers.Length - 1;
    }


    private void Update()
    {
        float deltaX = cameraTransform.position.x - lastCameraX;
        float deltaY = cameraTransform.position.y - lastCameraY;
        transform.position += Vector3.right * (deltaX * paralaxSpeedX);
        transform.position += Vector3.up * (deltaY * paralaxSpeedY);
        lastCameraX = cameraTransform.position.x;
        lastCameraY = cameraTransform.position.y;
        if (cameraTransform.position.x < (layers[leftIndex].transform.position.x + viewZone)) ScrollLeft();
        if (cameraTransform.position.x > (layers[rightIndex].transform.position.x - viewZone)) ScrollRight();
    }


    private void ScrollLeft()
    {
        layers[rightIndex].position = new Vector3(layers[leftIndex].position.x - backgroundSize, layers[leftIndex].position.y, 0f);
        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0) rightIndex = layers.Length - 1;
    }

    private void ScrollRight()
    {
        layers[leftIndex].position = new Vector3(layers[rightIndex].position.x + backgroundSize, layers[rightIndex].position.y, 0f); 
        rightIndex = leftIndex;
        leftIndex ++;
        if (leftIndex == layers.Length) leftIndex = 0;
    }

}
