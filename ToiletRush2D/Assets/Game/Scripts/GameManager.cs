using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace toilet
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        [SerializeField] private DrawMananger _drawManager;
        [SerializeField] private List<LevelManager> _levels;

        private int _levelNum = 0;

        public LevelManager CurrentLevel => _levels[_levelNum];


        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            for (var i = 0; i < _levels.Count; i++)
            {
                _levels[i].gameObject.SetActive(false);
            }

            CurrentLevel.Init();
            _drawManager.SetTarget(CurrentLevel.Characters, CurrentLevel.StartPos, CurrentLevel.DestPos);
        }
        private void Update()
        {
            _drawManager.DrawUpdate();
            _levels[_levelNum].LevelUpdate();
        }

        public void NextLevel()
        {
            CurrentLevel.Release();
            
            if (_levelNum + 1 >= _levels.Count)
                _levelNum = 0;
            else
                ++_levelNum;

            CurrentLevel.Init();
            _drawManager.SetTarget(CurrentLevel.Characters, CurrentLevel.StartPos, CurrentLevel.DestPos);
        }

    }
}
