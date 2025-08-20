using System;
using UnityEngine;

public class ObjectDeleter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hi");
        if (!other.CompareTag("Ground"))
        {
            other.gameObject.SetActive(false);
        }
    }
}
