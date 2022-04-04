using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class propLauncher : MonoBehaviour
{
    [Tooltip("The objects that will be thrown")]
    [SerializeField] private GameObject[] objsList;
    [Tooltip("How many will spawn each time.")]
    [SerializeField] private int quantityPerDelay;
    [Tooltip("the time between spawns.")]
    [SerializeField] private float delay;
    [Tooltip("How fast the objects will travel")]
    [SerializeField] private float objectVelocity;
    [SerializeField] private bool canThrowObjects = true;
    private BoxCollider spawnArea;
    private void Start()
    {
        spawnArea = GetComponent<BoxCollider>();
        StartCoroutine(CreateObjects());
    }
    IEnumerator CreateObjects()
    {
        while (canThrowObjects)
        {
            int r = Random.Range(0, objsList.Length);
            for (int i = 0; i < quantityPerDelay; i++)
            {
                Vector3 spawnPoint = transform.position + new Vector3(Random.Range(-spawnArea.size.x, spawnArea.size.x)/2, Random.Range(-spawnArea.size.y, spawnArea.size.y)/2, 0);
                GameObject gobj = Instantiate(objsList[r], spawnPoint, Quaternion.identity);
                gobj.GetComponent<Rigidbody>().velocity = transform.forward * objectVelocity;
            }
            yield return new WaitForSeconds(delay);
        }
    }
}
