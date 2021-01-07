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

    #region Unity Events

    private void Awake()
    {
        MakeNode();

        RBody = GetComponent<Rigidbody>();
        if (RBody == null)
            RBody = gameObject.AddComponent<Rigidbody>();
        Anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        TEMP_WasdMove();
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

    #endregion // ==========================================================

    #region Public Methods



    #endregion // ==========================================================

}
