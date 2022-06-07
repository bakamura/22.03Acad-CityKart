using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InputCar : MonoBehaviour
{
    public InputData inputData;
    [NonSerialized] public int invertControls = 1;

    public float HorzMov()
    {
        return Input.GetAxis(inputData.HorizontalMovment) * invertControls;
    }
    public float VertMov()
    {
        return Input.GetAxis(inputData.VerticalMovment) * invertControls;
    }
    public bool Drift()
    {
        switch (inputData.inputType)
        {
            case InputData.inputTypes.Keyboard:
                return Input.GetButton(inputData.Drift);
            case InputData.inputTypes.Controller:
               return Input.GetAxis(inputData.Drift) > 0;
            default:
                return false;
        }
    }
    public bool UseItem()
    {
        switch (inputData.inputType)
        {
            case InputData.inputTypes.Keyboard:
                return Input.GetButton(inputData.UseItem);
            case InputData.inputTypes.Controller:
                return Input.GetAxis(inputData.UseItem) > 0;
            default:
                return false;
        }
    }
    public bool Pause()
    {
        return Input.GetButton(inputData.Pause);
    }
}
