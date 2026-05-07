using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveAnt : MonoBehaviour
{
    private static Guid ID;

    [Header("Movement Settings")] [SerializeField]
    private float bodyHeight = 0.5f;

    [Header("Attack Settings")] [SerializeField]
    private float attackDistance = 0.5f;

    [SerializeField] private float attackDuration = 0.5f;

    [Header("References")] [SerializeField]
    private List<LegTarget> targets;

    [SerializeField] private List<Transform> lastJoints;

    public EventHandler<MovementEventArgs> onMovementStarted;
    public EventHandler<MovementEventArgs> onMovementChanged;
    public EventHandler<MovementEventArgs> onMovementStopped;

    private Rigidbody rb;
    private bool canMoveOddLegs = true;
    private bool isAttacking = false;

    private Vector3 inputDir;
    private GameObject enemy;

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
    }

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