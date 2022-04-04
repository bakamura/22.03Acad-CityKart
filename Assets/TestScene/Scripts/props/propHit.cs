using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class propHit : MonoBehaviour
{
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private GameObject ps;
    [SerializeField] private float TimeInScene;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //List<float> values = new List<float>();
        //foreach (ParticleSystem particle in ps.GetComponentsInChildren<ParticleSystem>()) values.Add(particle.duration);
        //duration = Mathf.Max(values.ToArray());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") collision.gameObject.GetComponent<Rigidbody>().velocity = -collision.GetContact(0).normal * Mathf.Abs(rb.velocity.z);
    }
    private void Start()
    {
        StartCoroutine(this.DestroyObject());
    }
    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(TimeInScene);
        Instantiate(ps, transform.position, Quaternion.identity, null);
        Destroy(this.gameObject);
    }
}
