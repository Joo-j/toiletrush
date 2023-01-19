using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace toilet
{
    public class Character : MonoBehaviour
    {
        public enum CharacterType
        {
            Unknown = -1,
            Man = 0,
            Girl,
        }

        [SerializeField] private CharacterType _type;
        [SerializeField] private Gradient _color;
        [SerializeField] private Animator _animator;

        private readonly string ANI_RUN = "isRun";
        private readonly string ANI_IDLE = "idle";
        private readonly string ANI_DIE = "die";

        private readonly float _moveTime = 7.5f;
        private readonly float _moveDistance = 0.01f;
        private readonly float _arriveDistance = 0.5f;

        private float _originScale = 0.4f;
        private bool _arrived = false;
        private bool _moving = false;
        private bool _crushed = false;
        private bool _ready = false;

        public bool Ready => _ready;
        public bool Arrived => _arrived;
        public bool Moving => _moving;
        public CharacterType Type => _type;
        public Gradient Color => _color;

        private void Start()
        {
            _originScale = transform.localScale.x;
        }

        public void SetReady(bool value)
        {
            _ready = value;
        }

        public void Move(List<Vector2> path, Vector3 destPos, System.Action clearLine, System.Action onDraw)
        {
            StartCoroutine(Co_Move(path, destPos, clearLine, onDraw));
        }

        private IEnumerator Co_Move(List<Vector2> path, Vector3 destPos, System.Action clearLine, System.Action onDraw)
        {
            _moving = true;
            _animator.SetBool(ANI_RUN, true);

            var speed = path.Count / _moveTime;
            for (var i = 0; i < path.Count; i++)
            {
                while (Vector2.Distance(transform.position, path[i]) > _moveDistance)
                {
                    yield return null;

                    if (_crushed)
                    {
                        clearLine.Invoke();
                        StartCoroutine(Co_Return(path, i, onDraw));
                        yield break;
                    }

                    transform.position = Vector2.MoveTowards(transform.position, path[i], speed * Time.deltaTime);
                    var dir = (new Vector2(transform.position.x, transform.position.y) - path[i]).normalized;
                    if (dir.x < 0.5)
                        transform.localScale = new Vector3(-_originScale, _originScale, _originScale);
                    else
                        transform.localScale = new Vector3(_originScale, _originScale, _originScale);
                }
            }

            if (Vector2.Distance(transform.position, destPos) < _arriveDistance)
            {
                _arrived = true;
                _animator.SetBool(ANI_RUN, false);
            }

            yield return new WaitForSeconds(1f);

            StartCoroutine(Co_Return(path, path.Count - 1, onDraw));
        }

        IEnumerator Co_Return(List<Vector2> path, int index, System.Action onDraw)
        {
            if (_crushed)
            {
                _animator.SetTrigger(ANI_DIE);
                yield return new WaitForSeconds(1.5f);
                _animator.SetTrigger(ANI_IDLE);
            }

            _animator.SetBool(ANI_RUN, true);

            var speed = path.Count / _moveTime;
            for (var i = index; i >= 0; i--)
            {
                while (Vector2.Distance(transform.position, path[i]) > _moveDistance)
                {
                    yield return null;
                    transform.position = Vector2.MoveTowards(transform.position, path[i], 3 * speed * Time.deltaTime);
                }
            }

            _moving = false;
            _crushed = false;
            _ready = false;
            _arrived = false;
            _animator.SetBool(ANI_RUN, false);
            onDraw.Invoke();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var character = other.GetComponent<Character>();
            if (null != character)
            {
                _crushed = true;
                return;
            }

            var obstacle = other.GetComponent<Obstacle>();
            if (null != obstacle)
            {
                _crushed = true;
                return;
            }

        }
    }
}


