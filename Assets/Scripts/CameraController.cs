using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private TileGridController grid;
    private Camera cam;

    private void Awake()
    {
        grid = FindFirstObjectByType<TileGridController>();
        cam = GetComponent<Camera>();
    }

    public void SetCamPositionAndSize()
    {
        var positionX = (float)grid.GridSizeX;
        positionX /= 2;
        positionX -= 0.5f;
        var positionY = (float)grid.GridSizeY;
        positionY /= 2;
        positionY -= 0.5f;
        var position = new Vector3(positionX, positionY, -10f);
        transform.position = position;
        var size = (float)grid.GridSizeY;
        size *= 0.5f;
        cam.orthographicSize = size;
    }
}
