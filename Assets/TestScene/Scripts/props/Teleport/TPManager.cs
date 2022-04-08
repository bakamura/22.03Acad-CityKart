using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TPManager : MonoBehaviour
{
    public static TPManager Instance { get; private set; }
    [NonSerialized] public Transform curentPoint;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && curentPoint != null)
        {
            other.transform.position = curentPoint.position;
        }
    }
}
