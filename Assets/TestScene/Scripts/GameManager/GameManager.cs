using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class GameManager
{
    [Header("Players Info")]
    [NonSerialized] public const int MaxPlayers = 4;
    [NonSerialized] public static List<GameObject> playerCars = new List<GameObject>();
    [NonSerialized] public static List<InputData> playerInputs = new List<InputData>();
    [NonSerialized] public static GameObject[] finalResults;// 0 = first place, 1 = second place...
}
