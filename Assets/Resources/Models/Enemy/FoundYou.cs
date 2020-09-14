using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoundYou : MonoBehaviour
{
    Transform lastSee = null;
    public GameObject emptyGameObjectPrefab;

    float searchingTime = 10.0f;

    private void OnTriggerEnter(Collider other)
    {
        // check other's tag
        if (other.gameObject.tag == "Player")
        {

            transform.parent.GetComponent<AI_Controller>().Found = true;
            transform.parent.GetComponent<AI_Controller>().target = other.gameObject;
            
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            transform.parent.GetComponent<AI_Controller>().Found = false;

            createEmpty(other);
        }
    }

    void createEmpty(Collider other)
    {
        GameObject test = Instantiate(emptyGameObjectPrefab);
        test.transform.position = other.gameObject.transform.position;
        transform.parent.GetComponent<AI_Controller>().target = test;

        Destroy(test, searchingTime);
    }
}
