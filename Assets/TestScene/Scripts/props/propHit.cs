using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class propHit : MonoBehaviour
{
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private ParticleSystem ps;
    private Rigidbody rb;
    [HideInInspector] public bool canBeLaunched = true;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) collision.gameObject.GetComponent<PlayerData>().rb.velocity = -collision.GetContact(0).normal * Mathf.Abs(rb.velocity.z);
        rb.velocity = Vector3.zero;
        canBeLaunched = true;
        mesh.enabled = false;
        ps.Play();
    }
    public void RevertDestruction()
    {
        mesh.enabled = true;
        canBeLaunched = false;
    }
}
