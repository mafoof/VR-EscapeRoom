using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickUpItem : ClickableObject
{
    [Tooltip("What is the offset between the object's pivot and its center?")]
    [SerializeField] private Vector3 holdOffset;

    public Vector3 HoldOffset {  get { return holdOffset; } }
    protected override void OnClick()
    {
        PlayerPickUp.Instance.PickUp(this);
    }
}
