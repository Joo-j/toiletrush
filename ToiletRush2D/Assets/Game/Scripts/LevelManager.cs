using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace toilet
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<Character> _characters;
        [SerializeField] private List<Transform> _startPos;
        [SerializeField] private List<Transform> _destPos;

        private bool _initiated = false;
        private bool _cleared = false;
        private int _clearCount = 0;

        public List<Character> Characters => _characters;
        public List<Transform> StartPos => _startPos;
        public List<Transform> DestPos => _destPos;
        public bool Initiated => _initiated;
        public bool Clear => _cleared;

        public void Init()
        {
            _cleared = false;
            _initiated = true;
            gameObject.SetActive(true);
        }

        public void Release()
        {
            gameObject.SetActive(false);
        }

        public void LevelUpdate()
        {
            if (_cleared)
                return;

            for (var i = 0; i < _characters.Count; i++)
            {
                if (_characters[i].Arrived)
                    ++_clearCount;
            }

            if (_clearCount == _characters.Count)
            {
                _cleared = true;

                GameManager.Instance.NextLevel();
            }
            else
            {
                _clearCount = 0;
            }
        }

    }
}


