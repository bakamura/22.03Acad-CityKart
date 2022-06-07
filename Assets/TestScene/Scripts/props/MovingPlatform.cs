using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform[] movePoints;
    [SerializeField] private float displacement;
    [SerializeField] private float movmentSpeed;
    int i = 0;
    private void Awake()
    {
        InvokeRepeating(nameof(MovePlatform), 0f, movmentSpeed);
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && other.GetComponent<ObjectDetectionData>() != null) other.transform.SetParent(transform, true);
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && other.GetComponent<ObjectDetectionData>() != null) other.transform.SetParent(null, true);
    }
    private void MovePlatform()
    {
        Vector3 direction = (movePoints[i].localPosition - transform.localPosition).normalized;
        transform.position += new Vector3(displacement * direction.x, displacement * direction.y, displacement * direction.z);
        if (Mathf.Abs(transform.localPosition.x) >= Mathf.Abs(movePoints[i].localPosition.x) &&
        Mathf.Abs(transform.localPosition.y) >= Mathf.Abs(movePoints[i].localPosition.y) &&
        Mathf.Abs(transform.localPosition.z) >= Mathf.Abs(movePoints[i].localPosition.z))
        {
            i++;
            if (i >= movePoints.Length) i = 0;
        }
    }
}
