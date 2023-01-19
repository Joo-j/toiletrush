using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace toilet{
public class Line : MonoBehaviour
    {
 [SerializeField] private LineRenderer _lineRenderer;
 [SerializeField] private EdgeCollider2D _edgeCollider;

    public LineRenderer LineRenderer => _lineRenderer;
    public EdgeCollider2D EdgeCollider => _edgeCollider;

    }
}


