using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TrackSetUP : MonoBehaviour
{
    [SerializeField] private Transform[] playerPositions = new Transform[4];
    [SerializeField] private LayerMask[] playerCameraLayers = new LayerMask[4];
    private void Awake()
    {
        for (int i = 0; i < GameSelectionManager.playerCars.Count; i++)
        {
            CarControler script = Instantiate(GameSelectionManager.playerCars[i], playerPositions[i].position, Quaternion.identity, null).GetComponent<CarControler>();
            script.inputManager.inputData = GameSelectionManager.playerInputs[i];
            SetCameraLens(script.gameObject.GetComponentInChildren<Camera>(), script.gameObject.GetComponentInChildren<CinemachineVirtualCamera>().gameObject, i);
        }
    }
    private void SetCameraLens(Camera camera, GameObject cinemachine, int currentPlayer)
    {
        switch (GameSelectionManager.playerCars.Count)
        {
            default:
                return;
            case 2:
                switch (currentPlayer)
                {
                    case 0:
                        camera.rect = new Rect(0, 0, 1, 1);
                        break;
                    case 1:
                        camera.rect = new Rect(0, .5f, 1, 1);
                        break;
                }
                break;
            case int i when i >= 3:
                switch (currentPlayer)
                {
                    case 0:
                        //upper left
                        camera.rect = new Rect(-.5f, .5f, 1, 1);
                        break;
                    case 1:
                        //upper right
                        camera.rect = new Rect(.5f, .5f, 1, 1);
                        break;
                    case 2:
                        //bottom left
                        camera.rect = new Rect(-.5f, -.5f, 1, 1);
                        break;
                    case 3:
                        //bottom right
                        camera.rect = new Rect(.5f, -.5f, 1, 1);
                        break;
                }
                break;
        }
        SetCameraTags(camera, cinemachine, currentPlayer);
    }
    private void SetCameraTags(Camera camera, GameObject cinemachine, int currentPlayer)
    {
        switch (currentPlayer)
        {
            case 0:
                cinemachine.layer = 15;
                camera.cullingMask = playerCameraLayers[0];
                break;
            case 1:
                cinemachine.layer = 16;
                camera.cullingMask = playerCameraLayers[1];
                break;
            case 2:
                cinemachine.layer = 17;
                camera.cullingMask = playerCameraLayers[2];
                break;
            case 3:
                cinemachine.layer = 18;
                camera.cullingMask = playerCameraLayers[3];
                break;
        }
    }
}
