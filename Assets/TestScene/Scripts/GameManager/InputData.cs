using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inputs", menuName = "InputData")]
public class InputData : ScriptableObject
{
    public enum inputTypes
    {
        Keyboard,
        Controller,
    };
    public inputTypes inputType;
    public string VerticalMovment;
    public string HorizontalMovment;
    public string Drift;
    public string UseItem;
    public string Pause;
}
