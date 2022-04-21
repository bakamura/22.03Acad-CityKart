using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerData : MonoBehaviour
{
    [Header("ManagerVariables")]
    [NonSerialized] public InputCar inputManager = null;
    [NonSerialized] public int CarID;
    [NonSerialized] public int PlayerScore;
}
