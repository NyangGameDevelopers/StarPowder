using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2021-01-07 21:56

/// <summary> 플레이어 캐릭터 핵심 컴포넌트 스크립트 </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerCore : MonoBehaviour
{
    // Temp
    [Range(1f, 20f)]
    public float _moveSpeed = 3;
    [Range(1f, 20f)]
    public float _turningSpeed = 1;
    [Range(1f, 3f)]
    public float _runSpeedMultiplier = 1.2f;

    private bool _isRunning = false;

    public Rigidbody RBody { get; private set; }
    public Animator Anim { get; private set; }


    // Temp - CJK ***************************************************************
    // Cam look variables.
    [SerializeField]
    private float rotSpeedX; // Mouse X sensitivity control, set in editor.
    [SerializeField]
    private float rotSpeedY; // Mouse Y sensitivity control, set in editor.
    [SerializeField]
    private float rotDamp; // Damping value for camera rotation.

    private float mX = 0f; // Mouse X
    private float mY = 0f; // Mouse Y

    // Player move variables.
    [SerializeField]
    private float walkSpeed; // Walk (normal movement) speed, set in editor.
    [SerializeField]
    private float runSpeed; // Run speed, set in editor.

    private float currentSpeed; // Stores current movement speed.

    [SerializeField]
    private KeyCode runKey; // Run key, set in editor.

    private CharacterController cc; // Reference to attached CharacterController.

    [SerializeField]
    private GameObject playerCamera; // Player cam, set in editor.
    // ***************************************************************

    #region Unity Events

    private void Awake()
    {
        MakeNode();
        RBody = GetComponent<Rigidbody>();
        if (RBody == null)
            RBody = gameObject.AddComponent<Rigidbody>();
        Anim = GetComponentInChildren<Animator>();

        // Temp - CJK ***************************************************************
        cc = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;
        // ***************************************************************
    }

    private void Update()
    {
        TEMP_CJK_UpdateMove();
        //TEMP_WasdMove();
    }

    #endregion // ==========================================================

    #region Private Methods

    /// <summary> BT 노드 조립 </summary>
    private void MakeNode()
    {

    }

    private void TEMP_WasdMove()
    {
        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDir += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) moveDir += Vector3.back;
        if (Input.GetKey(KeyCode.D)) moveDir += Vector3.right;
        if (Input.GetKey(KeyCode.A)) moveDir += Vector3.left;

        _isRunning = Input.GetKey(KeyCode.LeftShift);

        moveDir.Normalize();
        if (moveDir.magnitude > 0.1f)
        {
            RBody.velocity = moveDir * _moveSpeed * (_isRunning ? _runSpeedMultiplier : 1f);
            TEMP_LookAt(Quaternion.LookRotation(moveDir));
        }

        TEMP_UpdateAnimation(moveDir);
    }

    private void TEMP_LookAt(Quaternion targetRotation)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _turningSpeed * Time.deltaTime * 500f);
    }

    private void TEMP_UpdateAnimation(Vector3 moveVector)
    {
        if (moveVector.magnitude < 0.1) Anim.Play("IDLE");
        else if (_isRunning) Anim.Play("RUN");
        else Anim.Play("WALK");
    }

    // Temp - CJK ***************************************************************
    private void TEMP_CJK_UpdateMove()
    {
        // MOVEMENT - Hor and Ver axis:

        // Get mouse axis
        mX += Input.GetAxis("Mouse X") * rotSpeedX * (Time.deltaTime * rotDamp);
        mY += Input.GetAxis("Mouse Y") * rotSpeedY * (Time.deltaTime * rotDamp) * -1;

        // Clamp Y so player can't 'flip'
        mY = Mathf.Clamp(mY, -80, 80); // mY 값이 범위를 넘지 않도록 (목이 돌아가지 않도록 설정)

        // Adjust rotation of camera and player's body.
        // Rotate the camera on its X axis for up / down camera movement.
        playerCamera.transform.localEulerAngles = new Vector3(mY, 0f, 0f);
        // Rotate the player's body on its Y axis for left/ right camera movement.
        transform.eulerAngles = new Vector3(0f, mX, 0f);

        // Get Hor and Ver imput.
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        // Set speed to walk speed.
        currentSpeed = walkSpeed;
        // If player is pressing run key and moving forward, set speed to run speed.
        if (Input.GetKey(runKey) && Input.GetKey(KeyCode.W))
        {
            currentSpeed = runSpeed;
        }
            

        // Get new move position based off input.
        Vector3 moveDir = (transform.right * hor) + (transform.forward * ver);
        TEMP_UpdateAnimation(moveDir);

        // Move CharController.
        // Move will not apply gravity, use SimpleMove if you want gravity.
        cc.Move(moveDir * currentSpeed * Time.deltaTime);
    }
    // ***************************************************************

    #endregion // ==========================================================

    #region Public Methods



    #endregion // ==========================================================

}
