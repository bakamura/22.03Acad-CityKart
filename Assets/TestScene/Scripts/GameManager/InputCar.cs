using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InputCar : MonoBehaviour
{
    public InputData inputData;
    public float HorzMov()
    {
        return Input.GetAxis(inputData.HorizontalMovment);
    }
    public float VertMov()
    {
        return Input.GetAxis(inputData.VerticalMovment);
    }
    public bool Drift()
    {
        switch (inputData.inputType)
        {
            case InputData.inputTypes.Keyboard:
                return Input.GetButton(inputData.Drift);
            case InputData.inputTypes.Controller:
               return Input.GetAxis(inputData.Drift) > 0 ? true: false;
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
                return Input.GetAxis(inputData.UseItem) > 0 ? true : false;
            default:
                return false;
        }
    }
    public bool Pause()
    {
        return Input.GetButton(inputData.Pause);
    }
}
