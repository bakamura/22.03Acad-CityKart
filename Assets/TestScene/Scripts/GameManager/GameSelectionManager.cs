using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class GameSelectionManager
{
    [Header("Players Info")]
    [NonSerialized] public static List<GameObject> playerCars = new List<GameObject>();
    [NonSerialized] public static List<InputData> playerInputs = new List<InputData>();
}
