using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    [SerializeField] ObjectInteraction[] objectsWithInteractions;
    [SerializeField] bool showDebugMessages = false;

    bool clicking = false;
    Vector3 startPos;
    Animator animator;
    protected void Start()
    {
        startPos = transform.position;
        animator = gameObject.GetComponent<Animator>();

    }
    protected void OnMouseOver()
    {
        if (Input.GetAxis("ObjectInteract") > 0 && !clicking)
        {
            clicking = true;
            PickUpItem heldItem = PlayerPickUp.Instance.HeldItem;
            ObjectInteraction interaction = GetInteraction(heldItem);
            if (interaction != null)
            {
                switch(interaction.reaction)
                {
                    case Reaction.AnimationTrigger:
                        if (!animator) {
                            Debug.LogError("ClickableObject " + name + " reacts to " + interaction.itemName + " with an animation, but has no Animator");
                        }
                        animator.SetTrigger(interaction.animationTrigger);
                        break;
                    case Reaction.Shake:
                        StartCoroutine("Shake");
                        break;
                    case Reaction.Other:
                        OnClickWithObject(interaction.itemName);
                        break;
                }
            }
            else
            {
                OnClick();
            }
        } else if (Input.GetAxis("ObjectInteract") == 0) { clicking = false; }
    }

    protected virtual void OnClick()
    {
        ShowMessage("item was interacted with with no object.");
        StartCoroutine("Shake");
    }
    protected virtual void OnClickWithObject(string itemName)
    {
        switch (itemName)
        {
            //case "GrayTable":
            //    ShowMessage("interacted with gray table");
            //    break;
            case "BrownTable":
                ShowMessage("interacted with brown table");
                break;
            default:
                Debug.LogError("There is an object in objectsWithInteractions that has no interaction defined in OnClickWithObject");
                break;
        }
        //StartCoroutine("Shake");
    }

    protected IEnumerator Shake()
    {
        float shakeTime = 1;
        float numShakes = 5;
        float time = 0;
        float shakeDist = 0.05f;
        while (time < shakeTime)
        {
            float currentDist = shakeDist * Mathf.Sin(time * numShakes * (2 * Mathf.PI));
            transform.position = startPos + new Vector3(currentDist, 0, currentDist);
            yield return new WaitForFixedUpdate();
            time += Time.deltaTime;
        }
        transform.position = startPos;
    }
    private void ShowMessage(string str)
    {
        if (showDebugMessages)
        {
            Debug.Log(str);
        }
    }
    private ObjectInteraction GetInteraction(PickUpItem heldItem)
    {
        if (!heldItem) { return null; }
        foreach (ObjectInteraction o in objectsWithInteractions)
        {
            if (o.pickUpItem == heldItem) { return o; }
        }
        return null;
    }
}
