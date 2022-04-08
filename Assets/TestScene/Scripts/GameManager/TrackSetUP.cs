using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSetUP : MonoBehaviour
{
    [SerializeField] private Transform[] playerPositions = new Transform[4];
    private void Awake()
    {
        for (int i = 0; i < GameSelectionManager.playerCars.Count; i++)
        {
            CarControler script = Instantiate(GameSelectionManager.playerCars[i], playerPositions[i]).GetComponent<CarControler>();
            script.inputManager.inputData = GameSelectionManager.playerInputs[i];
            SetCameraLens(script.gameObject.GetComponentInParent<Camera>(), i);
        }
    }
    private void SetCameraLens(Camera camera, int currentPlayer)
    {
        switch (GameSelectionManager.playerCars.Count)
        {
            default:
                return;
            case 2:
                switch (currentPlayer)
                {
                    case 1:
                        camera.rect = new Rect(0, 0, 1, 1);
                        break;
                    case 2:
                        camera.rect = new Rect(0, .5f, 1, 1);
                        break;
                }
                break;
            case int i when i >= 3:
                switch (currentPlayer)
                {
                    case 1:
                        //upper left
                        camera.rect = new Rect(-.5f, .5f, 1, 1);
                        break;
                    case 2:
                        //upper right
                        camera.rect = new Rect(.5f, .5f, 1, 1);
                        break;
                    case 3:
                        //bottom left
                        camera.rect = new Rect(-.5f, -.5f, 1, 1);
                        break;
                    case 4:
                        //bottom right
                        camera.rect = new Rect(.5f, -.5f, 1, 1);
                        break;
                }
                break;
        }
    }
}
