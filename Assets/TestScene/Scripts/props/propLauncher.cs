using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class propLauncher : MonoBehaviour
{
    [SerializeField] int MaxObjts;
    [Tooltip("The objects that will be thrown")]
    [SerializeField] private GameObject[] objsList;
    [Tooltip("How many will spawn each time.")]
    [SerializeField] private int quantityPerDelay;
    [Tooltip("the time between spawns.")]
    [SerializeField] private float delay;
    [Tooltip("How fast the objects will travel")]
    [SerializeField] private float objectVelocity;
    [SerializeField] private bool canThrowObjects = true;
    [SerializeField] private SpriteRenderer spawnArea;
    List<GameObject> spawnObjects = null;
    Coroutine spawnCoroutine = null;
    private void Awake()
    {
        spawnArea = GetComponentInChildren<SpriteRenderer>();
        spawnArea.enabled = false;
        if (spawnObjects == null)
        {
            spawnObjects = new List<GameObject>();
            for (int i = 0; i < MaxObjts; i++) spawnObjects.Add(Instantiate(objsList[Random.Range(0, objsList.Length)], transform.position, Quaternion.identity));
        }
        //StartCoroutine(CreateObjects());
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(this.CreateObjects());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }
    IEnumerator CreateObjects()
    {
        while (canThrowObjects)
        {
            for (int i = 0; i < quantityPerDelay; i++)
            {
                int objIndex = GetRandomObject();
                Vector3 launchPoint = spawnArea.gameObject.transform.position + new Vector3(Random.Range(-spawnArea.size.x, spawnArea.size.x) * spawnArea.gameObject.transform.localScale.x / 2, Random.Range(-spawnArea.size.y, spawnArea.size.y) * spawnArea.gameObject.transform.localScale.y / 2, 0);
                spawnObjects[objIndex].transform.position = launchPoint;
                spawnObjects[objIndex].GetComponent<propHit>().RevertDestruction();
                spawnObjects[objIndex].GetComponent<Rigidbody>().velocity = transform.forward * objectVelocity;
            }
            yield return new WaitForSeconds(delay);
        }
    }
    int GetRandomObject()
    {
        int index = 0;
        for (int i = 0; i < spawnObjects.Count; i++)
        {
            if (spawnObjects[i].GetComponent<propHit>().canBeLaunched)
            {
                index = i;
                break;
            }
        }
        return index;
    }
}
