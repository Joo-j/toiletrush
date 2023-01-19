using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace toilet
{
    public class DrawMananger : MonoBehaviour
    {
        [SerializeField] private float _minDistance = 0.01f;
        [SerializeField] private float _selectDistance = 0.5f;
        [SerializeField] private Line _line;

        private LineRenderer _lineRenderer;
        private EdgeCollider2D _edgeCollider;
        private List<Vector2> _path = new List<Vector2>();

        private List<Character> _characters = null;
        private List<Transform> _startPos = null;
        private List<Transform> _destPos = null;
        private Character _movingCharacter = null;
        private List<List<Vector2>> _movingPath = new List<List<Vector2>>();
        private List<Line> _lines = new List<Line>();
        private Dictionary<Character, Transform> _info = new Dictionary<Character, Transform>();

        public void SetTarget(List<Character> characters, List<Transform> startPos, List<Transform> destPos)
        {
            _characters = characters;
            _startPos = startPos;
            _destPos = destPos;
            RefreshVariables();
        }

        public void DrawUpdate()
        {
            if (null == _characters)
                return;

            for (var i = 0; i < _characters.Count; i++)
            {
                if (_characters[i].Moving)
                    return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _movingCharacter = null;
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                for (var i = 0; i < _characters.Count; i++)
                {
                    if (_characters[i].Ready)
                        continue;

                    if (_info.ContainsKey(_characters[i]))
                        continue;

                    var dist = Vector2.Distance(pos, _characters[i].transform.position);
                    if (dist < _selectDistance)
                    {
                        _movingCharacter = _characters[i];
                        _info.Add(_movingCharacter, _destPos[i]);
                        break;
                    }
                }

                if (null == _movingCharacter || _info.Count == 0)
                    return;

                var line = Instantiate(_line);
                _lines.Add(line);
                _lineRenderer = line.LineRenderer;
                _edgeCollider = line.EdgeCollider;
                _lineRenderer.colorGradient = _movingCharacter.Color;

                _path.Clear();
                _path.Add(pos);
                _lineRenderer.positionCount = 1;
                _lineRenderer.SetPosition(0, _path[0]);
            }
            else if (Input.GetMouseButton(0))
            {
                if (null == _movingCharacter)
                    return;

                if (_movingCharacter.Ready)
                    return;

                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (Vector2.Distance(pos, _path[_path.Count - 1]) < _minDistance)
                    return;

                if (_lineRenderer.positionCount <= 0)
                    return;

                _path.Add(pos);
                _lineRenderer.SetPosition(++_lineRenderer.positionCount - 1, pos);
                _edgeCollider.points = _path.ToArray();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (null == _movingCharacter)
                    return;

                var destination = _info[_movingCharacter];

                if (Vector2.Distance(_path[_path.Count - 1], destination.position) > _selectDistance * 2)
                {
                    _info.Remove(_movingCharacter);
                    _lines[_lines.Count - 1].gameObject.SetActive(false);
                    _lines.RemoveAt(_lines.Count - 1);

                    return;
                }

                _movingPath.Add(_path.GetRange(0, _path.Count - 1));
                _movingCharacter.SetReady(true);

                if (_info.Count != _characters.Count)
                    return;

                var count = 0;
                foreach (var i in _info)
                {
                    i.Key.Move(_movingPath[count++], i.Value.position, ClearLine, RefreshVariables);
                }
            }
        }

        public void ClearLine()
        {
            for (var i = 0; i < _lines.Count; i++)
            {
                _lines[i].gameObject.SetActive(false);
            }
        }
        
        public void RefreshVariables()
        {
            ClearLine();
            _info.Clear();
            _movingPath.Clear();
            _lines.Clear();
        }
    }
}
