using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InputCar : MonoBehaviour
{
    [NonSerialized] public InputData inputData;
    public float HorzMov()
    {
        float result = 0;
        if (Input.GetKeyDown(inputData.keys[(int)InputData.inputTypes.FowardMovment]))
        {
            result += 1;
        }
        if (Input.GetKeyDown(inputData.keys[(int)InputData.inputTypes.BackwardMovment]))
        {
            result += -1;
        }
        return result;
    }
    public float VertMov()
    {
        float result = 0;
        if (Input.GetKeyDown(inputData.keys[(int)InputData.inputTypes.RightMovment]))
        {
            result += 1;
        }
        if (Input.GetKeyDown(inputData.keys[(int)InputData.inputTypes.LeftMovment]))
        {
            result += -1;
        }
        return result;
    }
    public bool Drift()
    {
        if (Input.GetKeyDown(inputData.keys[(int)InputData.inputTypes.Drift])) return true;
        else return false;
    }
    public bool UseItem()
    {
        if (Input.GetKeyDown(inputData.keys[(int)InputData.inputTypes.UseItem])) return true;
        else return false;
    }
}
