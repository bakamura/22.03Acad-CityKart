using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] movePoints;
    [SerializeField] private float velocity;
    private Rigidbody rb;
    int i = 0;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        rb.velocity = (movePoints[i].localPosition - transform.localPosition).normalized * velocity;
        if (Mathf.Abs(transform.localPosition.x) >= Mathf.Abs(movePoints[i].localPosition.x) &&
        Mathf.Abs(transform.localPosition.y) >= Mathf.Abs(movePoints[i].localPosition.y) &&
        Mathf.Abs(transform.localPosition.z) >= Mathf.Abs(movePoints[i].localPosition.z))
        {
            i++;
            if (i >= movePoints.Length) i = 0;            
        }
    }
}
