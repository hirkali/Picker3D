using System;
using Commands.Level;
using Data.UnityObjects;
using Data.ValueObjects;
using Signals;
using UnityEngine;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables
        
        [SerializeField] private Transform levelHolder;
        [SerializeField] private byte totalLevelCount;
        
        #endregion

        #region Private Variables

        private OnLevelLoaderCommand _onLevelLoaderCommand;
        private OnLevelDestroyerCommand _onLevelDestroyerCommand;
        private short _currentLevel;
        private LevelData _levelData;

        #endregion

        #endregion

        private void Awake()
        {
            Init();
            _levelData = GetLevelData();
            _currentLevel = GetActiveLevel();
        }

        private void Init()
        {
            _onLevelLoaderCommand = new OnLevelLoaderCommand(levelHolder);
            _onLevelDestroyerCommand = new OnLevelDestroyerCommand(levelHolder);
        }
        
        private LevelData GetLevelData()
        {
            return Resources.Load<CD_Level>("Data/CD_Level").Levels[_currentLevel];
        }
        
        private byte GetActiveLevel()
        {
            return (byte)_currentLevel;
        }

        public void LevelLoader(byte levelIndex)
        {
            _onLevelLoaderCommand.Execute(levelIndex);
        }
        public void LevelDestroyer( )
        {
            _onLevelDestroyerCommand.Execute();
        }
        private void OnEnable()
        {
            SubscribeEvents();
        }
        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelInitialize += _onLevelLoaderCommand.Execute;
            CoreGameSignals.Instance.onClearActiveLevel += _onLevelDestroyerCommand.Execute;
            CoreGameSignals.Instance.onGetLevelValue += OnGetLevelValue;
            CoreGameSignals.Instance.onNextLevel += OnNextLevel;
            CoreGameSignals.Instance.onRestartLevel += OnRestartLevel;
        }
        private void OnNextLevel()
        {
            _currentLevel++;
            CoreGameSignals.Instance.onClearActiveLevel?.Invoke();
            CoreGameSignals.Instance.onReset?.Invoke();
            CoreGameSignals.Instance.onLevelInitialize?.Invoke((byte) (_currentLevel % totalLevelCount));
        }
        private void OnRestartLevel()
        {
            CoreGameSignals.Instance.onClearActiveLevel?.Invoke();
            CoreGameSignals.Instance.onReset?.Invoke();
            CoreGameSignals.Instance.onLevelInitialize?.Invoke((byte) (_currentLevel % totalLevelCount));
        }
        private byte OnGetLevelValue()
        {
            return (byte)_currentLevel; 
        }
        
        private void UnSubscribeEvents()
        {
            CoreGameSignals.Instance.onLevelInitialize -= _onLevelLoaderCommand.Execute;
            CoreGameSignals.Instance.onClearActiveLevel -= _onLevelDestroyerCommand.Execute;
            CoreGameSignals.Instance.onGetLevelValue -= OnGetLevelValue;
            CoreGameSignals.Instance.onNextLevel -= OnNextLevel;
            CoreGameSignals.Instance.onRestartLevel -= OnRestartLevel;
        }
        private void OnDisable()
        {
            UnSubscribeEvents();
        }
        private void Start()
        {
            CoreGameSignals.Instance.onLevelInitialize?.Invoke((byte) (_currentLevel % totalLevelCount));
            //UISignals
        }
   
    }
}