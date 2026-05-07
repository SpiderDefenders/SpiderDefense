using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveAnt : MonoBehaviour
{
    private static Guid ID;
    [Header("Movement Settings")]
    [SerializeField] private float bodyHeight = 0.5f;
    [SerializeField] private float mouseSensitivity = 3f;

    [Header("Attack Settings")]
    [SerializeField] private float attackDistance = 0.5f;
    [SerializeField] private float attackDuration = 0.5f;
    
    [Header("References")]
    [SerializeField] private List<LegTarget> targets;
    [SerializeField] private List<Transform> lastJoints;

    public EventHandler<MovementEventArgs> onMovementStarted;
    public EventHandler<MovementEventArgs> onMovementChanged;
    public EventHandler<MovementEventArgs> onMovementStopped;

    private Rigidbody rb;
    private bool isMoving = false;
    private bool? isMovingZ = null;
    private bool? isMovingX = null;
    private bool canMoveOddLegs = true;
    private bool isAttacking = false;

    private Vector3 inputDir;
    private GameObject enemy;
    private bool isMovementEnabled = false;

    private void OnEnable()
    {
        onMovementStarted = null;
        onMovementChanged = null;
        onMovementStopped = null;
        ID = Guid.NewGuid();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void EnableMovement(object sender, EventArgs e)
    {
        StartCoroutine(EnableMovementAfterDalay(10));
    }
    
    IEnumerator EnableMovementAfterDalay(int numberOfFrames)
    {
        for (int i = 0; i < numberOfFrames; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        isMovementEnabled = true;
    }

    private void Update()
    {
        if (Time.timeScale == 0 || !isMovementEnabled) return;
        float mouseX = Input.GetAxis("Mouse X");
        float horizontalRotation = mouseX * mouseSensitivity;
        if (Mathf.Abs(horizontalRotation) > 0.01f)
        {
            Quaternion rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
            transform.rotation *= rotation;
        }
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        inputDir = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
        HandleMovementEvents(inputDir);
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private void FixedUpdate()
    {
        if (!isMovementEnabled) return;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        Vector3 move = 2f * Time.fixedDeltaTime * (transform.right * inputDir.x + transform.forward * inputDir.z);
        Vector3 targetPos = rb.position + move;
        targetPos.y = CalculateBodyHeight();
        rb.MovePosition(targetPos);
        CalculateBodyRotation();
        CalculateCanMoveLegs();
    }

    private void HandleMovementEvents(Vector3 inputDir)
    {
        if (inputDir.magnitude > 0f)
        {
            if (!isMoving)
            {
                isMoving = true;
                isMovingZ = inputDir.z != 0f ? inputDir.z > 0f : null;
                isMovingX = inputDir.x != 0f ? inputDir.x > 0f : null;
                onMovementStarted?.Invoke(this, new MovementEventArgs(ID.ToString(), isMoving, isMovingX, isMovingZ));
            }
            else
            {
                bool? newIsMovingX = inputDir.x != 0f ? inputDir.x > 0f : null;
                bool? newIsMovingZ = inputDir.z != 0f ? inputDir.z > 0f : null;
                if (newIsMovingZ != isMovingZ || newIsMovingX != isMovingX)
                {
                    isMovingX = newIsMovingX;
                    isMovingZ = newIsMovingZ;
                    onMovementChanged?.Invoke(this, new MovementEventArgs(ID.ToString(), isMoving, isMovingX, isMovingZ));
                }
            }
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;
                isMovingX = null;
                isMovingZ = null;
                onMovementStopped?.Invoke(this, new MovementEventArgs(ID.ToString(), isMoving, isMovingX, isMovingZ));
            }
        }
    }

    private void CalculateCanMoveLegs()
    {
        // Debug.Log(canMoveOddLegs);
        canMoveOddLegs = !targets[0].IsStepping() && !targets[1].IsStepping() && targets[2].IsStepping();
    }

    public void CalculateBodyRotation()
    {
        Quaternion currentYRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        Quaternion targetTilt = Quaternion.Euler(XAxisRotation(), 0f, -ZAxisRotation());
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, currentYRotation * targetTilt, Time.fixedDeltaTime * 10f);
        rb.MoveRotation(targetRotation);
    }

    private float ZAxisRotation()
    {
        if (targets.Count < 6) return 0f;
        float leftAvg = (targets[0].transform.position.y + targets[1].transform.position.y + targets[2].transform.position.y) / 3f;
        float rightAvg = (targets[3].transform.position.y + targets[4].transform.position.y + targets[5].transform.position.y) / 3f;
        return Mathf.Clamp((rightAvg - leftAvg) * 120f, -30f, 30f);
    }

    private float XAxisRotation()
    {
        if (targets.Count < 6) return 0f;
        float frontAvg = (targets[0].transform.position.y + targets[5].transform.position.y) / 2f;
        float backAvg = (targets[2].transform.position.y + targets[3].transform.position.y) / 2f;
        return Mathf.Clamp((backAvg - frontAvg) * 120f, -30f, 30f);
    }

    private float CalculateBodyHeight()
    {
        if (targets.Count == 0) return rb.position.y;
        float result = 0;
        foreach (LegTarget t in targets)
            result += t.transform.position.y;
        return (result / targets.Count) + bodyHeight;
    }

    public bool CanMoveOddLegs() => canMoveOddLegs;
    public string GetID() => ID.ToString();
    
    private IEnumerator Attack()
    {
        Vector3 startPos = rb.position;
        Vector3 forwardPos = startPos + transform.forward * attackDistance;
        float halfDuration = attackDuration / 2f;
        float timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            rb.MovePosition(Vector3.Lerp(startPos, forwardPos, timer / halfDuration));
            yield return null;
        }
        timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            rb.MovePosition(Vector3.Lerp(forwardPos, startPos, timer / halfDuration));
            yield return null;
        }
    }
}
