using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class TrackSetUP : MonoBehaviour
{
    [SerializeField] private Transform[] playerPositions = new Transform[4];
    [SerializeField] private LayerMask[] playerCameraLayers = new LayerMask[4];
    private void Awake()
    {
        for (int i = 0; i < GameManager.playerCars.Count; i++)
        {
            PlayerData Data = Instantiate(GameManager.playerCars[i], playerPositions[i].position, Quaternion.identity, null).GetComponent<PlayerData>();
            Data.inputManager.inputData = GameManager.playerInputs[i];
            Data.CarID = i;
            SetCameraLensAndUISize(Data.gameObject.GetComponent<CarControler>().cameras, Data.gameObject.GetComponent<CarControler>().screenSize, Data.gameObject.GetComponentInChildren<CinemachineVirtualCamera>().gameObject, i);
        }
    }
    private void SetCameraLensAndUISize(Camera[] camera, RectTransform screenSize, GameObject cinemachine, int currentPlayer)
    {
        switch (GameManager.playerCars.Count)
        {
            default:
                screenSize.sizeDelta = new Vector2(Screen.currentResolution.width * 2, Screen.currentResolution.height * 2);
                return;
            case 2:
                switch (currentPlayer)
                {
                    case 0:
                        foreach(var cam in camera) cam.rect = new Rect(0, .5f, 1, .5f);
                        screenSize.sizeDelta = new Vector2(Screen.currentResolution.width * 2, Screen.currentResolution.height);
                        break;
                    case 1:
                        foreach (var cam in camera) cam.rect = new Rect(0, -.5f, 1, 1);
                        screenSize.sizeDelta = new Vector2(Screen.currentResolution.width * 2, Screen.currentResolution.height);
                        break;
                }
                break;
            case int i when i >= 3:
                switch (currentPlayer)
                {
                    case 0:
                        //upper left
                        foreach (var cam in camera) cam.rect = new Rect(0, .5f, .5f, .5f);
                        screenSize.sizeDelta = new Vector2(Screen.currentResolution.width *2, Screen.currentResolution.height * 2);
                        break;
                    case 1:
                        //upper right
                        foreach (var cam in camera) cam.rect = new Rect(.5f, .5f, .5f, .5f);
                        screenSize.sizeDelta = new Vector2(Screen.currentResolution.width * 2, Screen.currentResolution.height * 2);
                        break;
                    case 2:
                        //bottom left
                        foreach (var cam in camera) cam.rect = new Rect(0, 0, .5f, .5f);
                        screenSize.sizeDelta = new Vector2(Screen.currentResolution.width * 2, Screen.currentResolution.height * 2);
                        break;
                    case 3:
                        //bottom right
                        foreach (var cam in camera) cam.rect = new Rect(.5f, 0, .5f, .5f);
                        screenSize.sizeDelta = new Vector2(Screen.currentResolution.width * 2, Screen.currentResolution.height * 2);
                        break;
                }
                break;
        }
        SetCameraTags(camera[0], cinemachine, currentPlayer);
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
