using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inputs", menuName = "InputData")]
public class InputData : ScriptableObject
{
    public enum inputTypes
    {
        FowardMovment,
        BackwardMovment,
        LeftMovment,
        RightMovment,
        Drift,
        UseItem,
        Pause
    };
    public inputTypes[] inputType = new inputTypes[7];
    public KeyCode[] keys = new KeyCode[7];        
}
