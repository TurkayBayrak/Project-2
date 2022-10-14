using System;
using UnityEngine;

public class StackDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError("girdi");
        if (other.transform.CompareTag("Stack"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
