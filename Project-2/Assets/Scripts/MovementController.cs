using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private Transform stackParentTransform;
    [SerializeField] private Transform backGroundParentTransform;

    private void Update()
    {
        stackParentTransform.Translate(Vector3.back * (Time.deltaTime * 3f));
        backGroundParentTransform.Translate(Vector3.back * (Time.deltaTime * 3f));
    }
}
