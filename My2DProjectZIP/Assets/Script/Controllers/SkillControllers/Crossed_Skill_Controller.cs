using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossed_Skill_Controller : MonoBehaviour
{
    private Vector2 startPos;
    private Vector2 lastTargetPos;

    private Vector2 midPos;

    private float percent;
    private float percentSpeed = 0;
    private float speed = 15;

    private player player;
    private Transform closestEnemy;

    public void Init(player _player, Transform _closestEnemy)
    {
        player = _player;
        closestEnemy = _closestEnemy;

        if (closestEnemy != null)
        {
            lastTargetPos = closestEnemy.position;
        }
        else
        {
            lastTargetPos = Utils.GetMousePosition();
        }

        startPos = _player.transform.position;

        midPos = GetMiddlePosition(startPos, lastTargetPos);

        percentSpeed = speed / (lastTargetPos - startPos).magnitude;

        transform.position = startPos;

        percent = 0;

    }

    private Vector2 GetMiddlePosition(Vector2 a, Vector2 b)
    {
        Vector2 m = Vector2.Lerp(a, b, .1f);
        Vector2 normal = Vector2.Perpendicular(a - b).normalized;
        float rd = Random.Range(-2f, 2f);
        float curveRatio = .3f;
        return m + (a - b).magnitude * curveRatio * rd * normal;
    }

    private void Update()
    {
        percent += percentSpeed * Time.deltaTime;
        if (percent > 1)
            percent = 1;
        transform.position = Utils.Bezier(percent, startPos, midPos, lastTargetPos);

        if (Vector2.Distance(transform.position , lastTargetPos) < 1f)
        {
            Destroy(gameObject);
        }
    }
}
