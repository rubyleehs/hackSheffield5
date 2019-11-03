using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Transform player;
    public float moveSpeed;
    public float sqrDetectionRange;
    public float fovAngle;
    public LayerMask wallMask;

    public List<Vector2Int> patrolPath;//will be preprocessed by A* then preporccessed again to cut corners
    public bool isPatrolling;
    public Vector2 target;

    public void SmartMove()
    {
        Vector3 delta = transform.position - player.position;
        if (delta.sqrMagnitude < sqrDetectionRange && Mathf.Abs(Vector3.Angle(delta, transform.right)) <= fovAngle && HasDirectLineOFSight(transform.position,player.position,0))
        {
            target = player.position;
            Fire();
        }
        else
        {
            MoveTowards(target);
        }
    }

    public bool HasDirectLineOFSight(Vector2 start, Vector2 end, float radius)
    {
        float _magnitude = Mathf.Sqrt(Vector2.SqrMagnitude(end - start));
        if (radius == 0) return !Physics2D.Raycast(start, end - start, _magnitude, wallMask);
        else return !(Physics2D.Raycast(start + Vector2.up * radius, end - start, _magnitude, wallMask) || Physics2D.Raycast(start - Vector2.up * radius, end - start, _magnitude, wallMask) || Physics2D.Raycast(start + Vector2.left * radius, end - start, _magnitude, wallMask) || Physics2D.Raycast(start + Vector2.right * radius, end - start, _magnitude, wallMask));
    }

    public void MoveTowards(Vector3 target)
    {
        if (!isPatrolling)
        {
            //A* to target
        }
        //Move directly
        else transform.position = (target - transform.position).normalized * moveSpeed * GameManager.deltaTime;
    }

    public void Fire()
    {

    }
}
