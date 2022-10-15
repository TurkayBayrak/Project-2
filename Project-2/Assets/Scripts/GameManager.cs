using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform fallingStackTransform;
    [SerializeField] private Material[] stackMaterials;
    [SerializeField] private TextMeshProUGUI matchCountText;

    private MeshRenderer currentStackMeshRenderer;
    private Transform previousStackTransform;
    private Transform currentStackTransform;
    private ObjectPooler ObjectPooler;
    private GameObject player;
    private int perfectSeries;
    private int matchCount;
    private int index;
    private int materialIndex;
    private bool isOnTheRight;
    private bool isGameOver;

    private void Start()
    {
        ObjectPooler = ObjectPooler.instance;
        StartCoroutine(WaitCo());
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SetStackAndSendNextOne();
        }
    }

    private void SetStackAndSendNextOne()
    {
        DOTween.KillAll();
        PlaceStack();
    }

    private void PlaceStack()
    {
        var currentLocalScale = currentStackTransform.localScale;
        var currentPosition = currentStackTransform.position;
        var previousPosX = previousStackTransform.position.x;
        var currentPosX = currentPosition.x;
        var currentRightX = currentPosition.x + currentLocalScale.x / 2;
        var previousLocalScale = previousStackTransform.localScale;
        var previousLeftX = previousPosX - previousLocalScale.x / 2;
        var currentLeftX = currentPosition.x - currentLocalScale.x / 2;
        var previousRightX = previousPosX + previousLocalScale.x / 2;

        if (previousRightX < currentLeftX || previousLeftX > currentRightX) //No match condition
        {
            currentStackTransform.DOMoveY(-30, 5);
            return;
        }

        matchCount++;
        matchCountText.text = matchCount.ToString();
        
        if (Mathf.Abs(currentPosition.x - previousPosX) < 0.2f) //Perfect match condition
        {
            currentStackTransform.position = new Vector3(previousPosX, currentPosition.y, currentPosition.z);
            AudioManager.instance.PlayNote(perfectSeries);
            perfectSeries++;
        }
        else //Matched but not perfect
        {
            perfectSeries = 0;
            AudioManager.instance.PlayCutEffect();
            if (currentPosX < previousPosX) //Current stack is on the left side of the previous stack
            {
                var newScaleX = currentRightX - previousLeftX;
                var newScale = new Vector3(newScaleX, currentLocalScale.y, currentLocalScale.z);
                fallingStackTransform.localScale = new Vector3(currentLocalScale.x - newScaleX, currentLocalScale.y, currentLocalScale.z);
                fallingStackTransform.position = new Vector3(previousLeftX - fallingStackTransform.localScale.x / 2, currentPosition.y, currentPosition.z);
                fallingStackTransform.eulerAngles = new Vector3(0, 0, 0);
                fallingStackTransform.DORotate(new Vector3(0, 0, 30), 1);
                currentStackTransform.localScale = newScale;
                var newPosX = previousLeftX + newScaleX / 2;
                var newPos = new Vector3(newPosX, currentPosition.y, currentPosition.z);
                currentStackTransform.position = newPos;
                player.transform.DOMoveX(newPosX, 1);
            }
            else //Current stack is on the right side of the previous stack
            {
                var newScaleX = previousRightX - currentLeftX;
                var newScale = new Vector3(newScaleX, currentLocalScale.y, currentLocalScale.z);
                fallingStackTransform.localScale = new Vector3(currentLocalScale.x - newScaleX, currentLocalScale.y, currentLocalScale.z);
                fallingStackTransform.position = new Vector3(previousRightX + fallingStackTransform.localScale.x / 2, currentPosition.y, currentPosition.z);
                fallingStackTransform.eulerAngles = new Vector3(0, 0, 0);
                fallingStackTransform.DORotate(new Vector3(0, 0, -30), 1);
                currentStackTransform.localScale = newScale;
                var newPosX = previousRightX - newScaleX / 2;
                var newPos = new Vector3(newPosX, currentPosition.y, currentPosition.z);
                player.transform.DOMoveX(newPosX, 1);
                currentStackTransform.position = newPos;
            }
            fallingStackTransform.DOMoveY(-30, 5);
            fallingStackTransform.GetComponent<MeshRenderer>().material = currentStackMeshRenderer.material;
        }
        GetNextStack();
    }

    private void GetNextStack()
    {
        if (index != ObjectPooler.pooledObjects.Count - 1)
            index++;
        else
            index = 0;
        
        var stack = ObjectPooler.instance.GetPooledObject("Stack", index);

        previousStackTransform = currentStackTransform;

        stack.transform.localScale = previousStackTransform.localScale;
        currentStackTransform = stack.transform;
        SetStackMaterial();

        stack.SetActive(true);
        SendNextStack();
    }

    private void SendNextStack()
    {
        var zAdjustmentPre = previousStackTransform.localScale.z / 2;
        var zAdjustmentCurrent = currentStackTransform.localScale.z / 2;
        var totalAddition = zAdjustmentPre + zAdjustmentCurrent;

        var position = previousStackTransform.position;

        var multiplier = isOnTheRight ? 1 : -1;
        
        var pos = new Vector3(position.x + 20 * multiplier, -0.5f, position.z + totalAddition);
        currentStackTransform.position = pos;

        currentStackTransform.DOMoveX(position.x - 20 * multiplier, 6);
        isOnTheRight = !isOnTheRight;
    }

    private IEnumerator WaitCo()
    {
        yield return new WaitForSeconds(1f);
        var stack = ObjectPooler.GetPooledObject("Stack", 0);
        currentStackTransform = stack.transform;
        stack.SetActive(true);

        previousStackTransform = GameObject.Find("FirstStack").transform;
        var zAdjustmentPre = previousStackTransform.localScale.z / 2;
        var zAdjustmentCurrent = stack.transform.localScale.z / 2;
        var totalAddition = zAdjustmentPre + zAdjustmentCurrent;

        var position = previousStackTransform.position;
        var pos = new Vector3(position.x - 20, -0.5f, position.z + totalAddition);
        stack.transform.position = pos;
        isOnTheRight = !isOnTheRight;

        stack.transform.DOMoveX(120, 6);
        SetStackMaterial();
    }

    private void SetStackMaterial()
    {
        currentStackMeshRenderer = currentStackTransform.GetComponent<MeshRenderer>();
        currentStackMeshRenderer.material = stackMaterials[materialIndex];

        if (materialIndex != stackMaterials.Length - 1)
            materialIndex++;
        else
            materialIndex = 0;
    }
}
