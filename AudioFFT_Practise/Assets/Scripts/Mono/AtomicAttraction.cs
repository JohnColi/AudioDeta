using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtomicAttraction : MonoBehaviour
{
    public GameObject _atom, _attractor;
    public Gradient _gradient;
    public int[] _attractPoints;
    public Vector3 _spacingDirection;
    [Range(0, 20)]
    public float _spacingBetweenAtractPoint;
    [Range(0, 20)]
    public float _scaleAtractPoint;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < _attractPoints.Length; i++)
        {
            float evaluateStep = 1.0f / _attractPoints.Length;
            Color color = _gradient.Evaluate(evaluateStep * i);
            Gizmos.color = color;

            var pos = transform.position + _spacingBetweenAtractPoint * _spacingDirection * i;
            Gizmos.DrawSphere(pos,_scaleAtractPoint);
        }
    }
}
