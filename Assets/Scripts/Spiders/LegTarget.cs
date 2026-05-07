using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class LegTarget : MonoBehaviour
{
    [SerializeField] private Transform legTargetPoint;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float maxDistance = 1.5f;
    [SerializeField] private float stepDuration = 0.25f;
    [SerializeField] private float stepHeight = 0.5f;
    [SerializeField] private float raycastDistance = 10f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform antBody;
    [SerializeField] private bool isOdd;
    [SerializeField] private bool simulate;

    private bool isStepping = false;
    private float stepTimer = 0f;
    private Vector3 stepStart;
    private Vector3 stepEnd;
    private Vector3 originalPosition;
    private string ID;
    private MoveAnt moveAnt;
    private float dist;

    private void Start()
    {
        originalPosition = transform.localPosition;
        moveAnt = GetComponentInParent<MoveAnt>();
        if (legTargetPoint != null)
        {
            legTargetPoint.position = transform.position;
        }

        if (moveAnt != null)
        {
            moveAnt.onMovementStarted += (sender, e) => ApplyOffset(e);
            moveAnt.onMovementChanged += (sender, e) => ApplyOffset(e);
            moveAnt.onMovementStopped += (sender, e) => ApplyOffset(e);
            ID = moveAnt.GetID();
        }
        if (simulate) ApplyOffset(new MovementEventArgs(ID, true, null, true));
    }

    private void Update()
    {
        if (legTargetPoint == null) return;
        
        dist = Vector3.Distance(legTargetPoint.position, transform.position);
        Vector3 rayOrigin = new Vector3(transform.position.x, antBody.transform.position.y + 0.5f, transform.position.z);
        Vector3 rayDirection = Vector3.down;
        Vector3 rayEnd = rayOrigin + rayDirection * raycastDistance;
        // Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.red);
        
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, raycastDistance, groundMask))
        {
            float dot = Vector3.Dot(rayDirection, hit.normal);
            if (dot < 0f)
            {
                transform.position = hit.point;
                // Debug.DrawRay(hit.point, hit.normal * 0.5f, Color.green, 0.5f);
            }
        }
        else
        {
            transform.position = rayEnd;
        }
        
        // if (!isStepping && dist > maxDistance && ((isOdd && moveAnt.CanMoveOddLegs()) || (!isOdd && !moveAnt.CanMoveOddLegs())))
        if (!isStepping && dist > maxDistance)
        {
            GoToTarget();
        }
        
        if (isStepping)
        {
            stepTimer += Time.deltaTime;
            float t = Mathf.Clamp01(stepTimer / stepDuration);
            Vector3 horizontal = Vector3.Lerp(stepStart, transform.position, t);
            float verticalOffset = Mathf.Sin(t * Mathf.PI) * stepHeight;
            legTargetPoint.position = horizontal + Vector3.up * verticalOffset;
        
            if (t >= 1f)
            {
                isStepping = false;
                legTargetPoint.position = transform.position;
            }
        }
    }

    public void ApplyOffset(MovementEventArgs e)
    {
        if (e.ID != ID) return;
        //Debug.Log("Moving legs");
        transform.localPosition = originalPosition;
        if (!e.IsMoving)
        {
            GoToTarget();
            return;
        }

        Vector3 newOffset = Vector3.zero;
        if (e.IsMovingX.HasValue)
        {
            newOffset.x = (e.IsMovingX.Value ? 1 : -1) * offset.x;
        }
        
        if (e.IsMovingZ.HasValue)
        {
            newOffset.z = (e.IsMovingZ.Value ? 1 : -1) * offset.z;
        }
        
        transform.localPosition += newOffset;
    }

    public void GoToTarget()
    {
        isStepping = true;
        stepTimer = 0f;
        stepStart = legTargetPoint.position;
    }

    public bool IsStepping() {  return isStepping; }
}
