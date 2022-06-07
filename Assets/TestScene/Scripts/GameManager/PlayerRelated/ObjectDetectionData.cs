using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetectionData : MonoBehaviour
{
    //yhis script is used to send data maninly to the Checkpoint and Items system, for the case that the colission detection is separate from these scripts
    public PlayerData playerData;
    public ItemCarUse itemData;
    public Transform kartTransform;
}
