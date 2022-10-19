using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour
{
    [SerializeField] float walkSpeed = 1;
    [SerializeField] float gravity = 9.8f;
    [Header("Crouching")]
    [SerializeField] float crouchHeight = 0.8f;
    [SerializeField] float crouchSpeed = 0.6f;
    [Tooltip("How long does the movement from standing to crouching take?")]
    [SerializeField] float crouchTime = 0.5f;

    bool                isCrouching = false;
    float               standingHeight;
    float               fallSpeed = 0;
    float               currentSpeed;

    CharacterController characterController;
    Camera              main;

    public bool IsCrouching { get { return isCrouching; } }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        main = Camera.main;
        standingHeight = characterController.height;
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        Crouch();
        Move();
    }

    /// <summary>
    /// Checks for horizontal and vertical input and moves the player in that direction.
    /// If the player is not grounded, it applies gravity to them.
    /// </summary>
    private void Move()
    {
        // player movement - forward, backward, left, right
        float horizontal = Input.GetAxis("Horizontal") * currentSpeed;
        float vertical = Input.GetAxis("Vertical") * currentSpeed;
        characterController.Move((main.transform.right * horizontal + main.transform.forward * vertical) * Time.deltaTime);

        // Gravity
        if (characterController.isGrounded)
        {
            fallSpeed = 0;
        }
        else
        {
            fallSpeed -= gravity * Time.deltaTime;
            characterController.Move(new Vector3(0, fallSpeed, 0));
        }
    }
    /// <summary>
    /// Checks for crouching input and if so changes the player controller height and movement speed
    /// </summary>
    private void Crouch()
    {
        bool crouchButtonHeld = Input.GetAxis("Crouch") > 0;
        if (crouchButtonHeld && !isCrouching)
        {
            currentSpeed = crouchSpeed;
            isCrouching = true;
            StartCoroutine("StartCrouch", new float[] { standingHeight, crouchHeight });

        }
        else if (!crouchButtonHeld && isCrouching)
        {
            currentSpeed = walkSpeed;
            isCrouching = false;
            StartCoroutine("StartCrouch", new float[] { crouchHeight, standingHeight });
        }
    }
    IEnumerator StartCrouch(float[] heights)
    {
        float startHeight = heights[0];
        float newHeight = heights[1];
        float time = 0;
        float totalTime = crouchTime;
        while (time < totalTime)
        {
            float h = Mathf.Lerp(startHeight, newHeight, time / totalTime);
            SetNewHeight(h);
            yield return new WaitForFixedUpdate();
            time += Time.deltaTime;
        }

        void SetNewHeight(float height)
        {
            float yOffset = height + 1;
            characterController.Move(new Vector3(0, yOffset, 0));
            //setting scale
            characterController.height = height;
        }
    }
}