﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed;
    public float sqrDetectionRange;
    public float fovAngle;

    public float lookAroundSpeed;
    public float lookAroundAngle;
    public LayerMask wallMask;

    public List<Vector2Int> patrolPath;
    public int patrolPathProgress;

    public bool isPatrolling;
    public bool isAlert;
    public Vector2 target;
    public IEnumerator actionInExecution;

    public void SmartMove()
    {
        Vector3 delta = transform.position - PlayerController.transform.position;
        if (delta.sqrMagnitude < sqrDetectionRange && Mathf.Abs(Vector3.Angle(delta, transform.right)) <= fovAngle && HasDirectLineOFSight(transform.position, PlayerController.transform.position, 0))
        {
            AlertPosition(PlayerController.transform.position);
            Fire();
        }
        else 
        {
            if (actionInExecution != null) return;
            MoveTowards(PathfindPathTowards(target));
            if (Vector2.Distance(transform.position, target) < 0.25f)
            {
                if (!isPatrolling)
                {
                    if (isAlert)
                    {
                        actionInExecution = LookAround(lookAroundSpeed, lookAroundAngle);
                        StartCoroutine(actionInExecution);
                        if (actionInExecution == null)
                        {
                            isAlert = false;
                            target = patrolPath[patrolPathProgress];
                        }
                    }
                    else isPatrolling = true;
                }
                else
                {
                    patrolPathProgress = (patrolPathProgress + 1) % (patrolPath.Count - 1);
                    target = patrolPath[patrolPathProgress];
                }
            }
        }

    }

    public void AlertPosition(Vector2 pos)
    {
        if (actionInExecution != null)
        {
            StopCoroutine(actionInExecution);
            actionInExecution = null;
        }
        isAlert = true;
        isPatrolling = false;
        target = pos;
    }

    public bool HasDirectLineOFSight(Vector2 start, Vector2 end, float radius)
    {
        float _magnitude = Mathf.Sqrt(Vector2.SqrMagnitude(end - start));
        if (radius == 0) return !Physics2D.Raycast(start, end - start, _magnitude, wallMask);
        else return !(Physics2D.Raycast(start + Vector2.up * radius, end - start, _magnitude, wallMask) || Physics2D.Raycast(start - Vector2.up * radius, end - start, _magnitude, wallMask) || Physics2D.Raycast(start + Vector2.left * radius, end - start, _magnitude, wallMask) || Physics2D.Raycast(start + Vector2.right * radius, end - start, _magnitude, wallMask));
    }

    public Vector3 PathfindPathTowards(Vector3 target)
    {
        List<Vector2> path = MapManager.pathFinder.Locate(Vector2Int.RoundToInt(transform.position), Vector2Int.RoundToInt(PlayerController.transform.position));
        if (path.Count > 1) return path[1];
        else if (path.Count == 1) return path[0];
        else return target;
    }

    public void MoveTowards(Vector3 target)
    {
        transform.position = (target - transform.position).normalized * moveSpeed * GameManager.deltaTime;
    }

    public void Fire()
    {

    }

    public IEnumerator LookAround(float rotateSpeed, float angle)
    {
        Vector2 centerAngle = transform.right - transform.position;
        Vector2 targetAngle = Quaternion.Euler(0, 0, angle * Mathf.Deg2Rad) * centerAngle;

        while ((Vector2)(transform.right - transform.position) != targetAngle)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.right, targetAngle, angle * GameManager.deltaTime, 0.0f));
            yield return new WaitForEndOfFrame();
        }
        targetAngle = Quaternion.Euler(0, 0, -angle * Mathf.Deg2Rad) * centerAngle;
        while ((Vector2)(transform.right - transform.position) != targetAngle)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.right, targetAngle, angle * GameManager.deltaTime, 0.0f));
            yield return new WaitForEndOfFrame();
        }
        actionInExecution = null;
    }
}
