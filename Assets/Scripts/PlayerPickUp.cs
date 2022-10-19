using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUp : MonoBehaviour
{
    //serialized vars
    [Header("Where to hold, where to drop")]
    [Tooltip("How far in front of you should you drop the item?")]
    [SerializeField] float dropDistance = 1;
    [Tooltip("How far below eye level should the object be when you drop it?")]
    [SerializeField] float dropHeightBelowEyelevel = 3;
    [Tooltip("how far from yourself are you holding the object? (x: right, y: up, z: forward)")]
    [SerializeField] Vector3 holdDistance = new Vector3(1, 0, 2);

    [Header("Object bobs when you're walking")]
    [Tooltip("How much does it go up/down while walking?")]
    [SerializeField] float objectBobbingHeight = 1;
    [Tooltip("How long does it take for it to go up, down and back to where it started?")]
    [SerializeField] float bobTime = 1;

    [Header("Camera layering")]
    [SerializeField] Camera heldItemCamera;
    private const int heldItemLayer = 10;

    //private vars
    private float time = 0f;
    //private Vector3 holdOffset;
    private PickUpItem      heldItem    = null;
    private Vector3         heldItemPosition;
    private LayerMask       itemLayer;

    private MovementScript  movement;
    private Camera          main;
    private CharacterController character;
    //getters and setters
    public PickUpItem HeldItem { get { return heldItem; } }

    private void Start()
    {
        main = Camera.main;
        //holdOffset = new Vector3(holdDistance.x, holdDistance.y, 0);
        movement = GetComponent<MovementScript>();
        character = GetComponentInChildren<CharacterController>();
    }
    private void Update()
    {
        if (Input.GetAxis("DropObject") > 0)
        {
            DropItem();
        }
        if (heldItem) { SetHeldItemPosition(); }
    }

    public void PickUp(PickUpItem item)
    {
        if (movement.IsCrouching) { return; }
        DropItem(); 
        heldItem = item;
        //set item layer
        itemLayer = item.gameObject.layer;
        SetLayer(item.transform, heldItemLayer);
        //turn off physics
        heldItem.GetComponent<Rigidbody>().isKinematic = true;
        heldItem.GetComponentInChildren<Collider>().enabled = false;
        //set parent
        heldItem.transform.SetParent(heldItemCamera.transform);
        //set position
        heldItemPosition = LocalHoldOffset(heldItemCamera.transform, holdDistance);
        heldItemPosition += LocalHoldOffset(heldItemCamera.transform, heldItem.HoldOffset);
        heldItem.transform.localPosition =  heldItemPosition;
    }
    private void SetHeldItemPosition()
    {
        if (character.velocity.x == 0 && character.velocity.y == 0) return;
        //move object to side of view, add bob offset
        Vector3 bobOffset = GetBobOffset();
        heldItem.transform.localPosition = heldItemPosition + bobOffset;

        Vector3 GetBobOffset()
        {
            time += Time.deltaTime;
            if (time >= bobTime) { time = 0; }
            Vector3 bobOffset = objectBobbingHeight * heldItemCamera.transform.up * Mathf.Sin(2 * Mathf.PI * time / bobTime);
            if (movement.IsCrouching) { bobOffset /= 2; }
            return bobOffset;
        }
    }

    private void DropItem()
    {
        if (!heldItem || movement.IsCrouching) { return; }
        //enable physics
        heldItem.GetComponent<Rigidbody>().isKinematic = false;
        heldItem.GetComponentInChildren<Collider>().enabled = true;
        //drop it in a position
        Vector3 itemPosition = main.transform.position + dropDistance * main.transform.forward;
        itemPosition.y = main.transform.position.y - dropHeightBelowEyelevel;
        itemPosition += LocalHoldOffset(heldItem.transform, heldItem.HoldOffset);
        heldItem.transform.position = itemPosition;
        //set back to its original layer
        heldItem.transform.SetParent(null);
        SetLayer(heldItem.transform, itemLayer);
        heldItem = null;
    }
    Vector3 LocalHoldOffset(Transform t, Vector3 offset)
    {
        Vector3 x = offset.x * t.right;    //x direction is to the right
        Vector3 y = offset.y * t.up;       //y direction is up
        Vector3 z = offset.z * t.forward;  //z direction is forward
        return x + y + z;

    }
    private void SetLayer(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        foreach( Transform child in t) { SetLayer(child, layer); }
    }
    //singleton code
    public static PlayerPickUp Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
