using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {
    [SerializeField] private Transform MaxHeight;
    [SerializeField] private float addPositionAmount = .5f;
    [SerializeField] private float addPositionFrequency = .02f;

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Player")) {
            //other.GetComponent<Rigidbody>().AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
            StartCoroutine(MoveObject(other.gameObject));
        }
    }
    IEnumerator MoveObject(GameObject obj)
    {
        Rigidbody playerRb = obj.GetComponent<Rigidbody>();
        float drag = playerRb.drag;
        playerRb.useGravity = false;
        playerRb.drag = 0;
        while (obj.transform.position.y < MaxHeight.position.y)
        {
            float distance = MaxHeight.position.y - obj.transform.position.y;
            obj.transform.position += new Vector3(0, distance*.05f + addPositionAmount, 0);
            yield return new WaitForSeconds(addPositionFrequency);
        }
        playerRb.useGravity = true;
        playerRb.drag = drag;
    }
}
