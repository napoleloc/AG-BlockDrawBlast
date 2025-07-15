using System.Runtime.CompilerServices;
using BlockDrawBlast.Database;
using BlockDrawBlast.Extensions;
using EncosyTower.Logging;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoGameplayManager : MonoBehaviour
    {
        [SerializeField] private LevelDataTableAsset _levelDataTable;
        [SerializeField] private MonoInputReceiver _inputReceiver;
        
        [Title("Managed", titleAlignment: TitleAlignments.Centered)]
        [SerializeField] private MonoDrawingManaged _monoDrawingManaged;
        [SerializeField] private MonoMatrixManaged _monoMatrixManaged;
        [SerializeField] private MonoStackManaged _monoStackManaged;

        [Title("Debugging", titleAlignment: TitleAlignments.Centered)] 
        [SerializeField] private GameplayStatus _gameplayStatus;
        
        private int _currentLevel;

        private void Start()
        {
            _inputReceiver.OnTouchStart += HandleTouchStart;
            _inputReceiver.OnTouchMove += HandleTouchMove;
            _inputReceiver.OnTouchEnd += HandleTouchEnd;
        }

        private void HandleTouchStart(float3 worldPosition)
        {
            // Convert world position to matrix position if needed
            var matrixPosition = _monoMatrixManaged.WorldToMatrixPosition(worldPosition);
            _monoDrawingManaged?.StartDrawing(matrixPosition);
        }

        private void HandleTouchMove(float3 worldPosition)
        {
            // Convert world position to matrix position if needed
            var matrixPosition = _monoMatrixManaged.WorldToMatrixPosition(worldPosition);
            _monoDrawingManaged?.ContinueDrawing(matrixPosition);
            
            DevLoggerAPI.LogInfo($"matrix-position: {matrixPosition}");
        }

        private void HandleTouchEnd(float3 worldPosition)
        {
            _monoDrawingManaged?.EndDrawing();
        }
        
        private void OnStartGame()
        {
            if (_gameplayStatus == GameplayStatus.StartGame)
            {
                return;
            }
            
            var entryOpt = _levelDataTable.GetLevelData(_currentLevel);

            if (entryOpt.TryValue(out var entry) == false)
            {
                DevLoggerAPI.LogError("Failed to get level data");
                return;
            }

            var preparedTileDataArray = ConvertDataHelper.ConvertTileDataBurst(entry.PreparedLevelTiles.Span);
            var preparedBlockDataArray = ConvertDataHelper.ConvertBlockDataBurst(entry.PreparedLevelBlocks.Span);
            var preparedStackBlockDataArray = ConvertDataHelper.ConvertStackDataBurst(entry.PreparedLevelStackBlocks.Span);
            
            try
            {
                _monoMatrixManaged.PreparedWhenStartGame(entry.rows, entry.columns, preparedTileDataArray, preparedBlockDataArray);
                _monoStackManaged.PreparedWhenStartGame(preparedStackBlockDataArray);
            }
            finally
            {
                // Clean up temporary arrays
                if (preparedTileDataArray.IsCreated) preparedTileDataArray.Dispose();
                if (preparedBlockDataArray.IsCreated) preparedBlockDataArray.Dispose();
                if (preparedStackBlockDataArray.IsCreated) preparedStackBlockDataArray.Dispose();
                
                _gameplayStatus = GameplayStatus.StartGame;
            }
        }

        private void OnPauseGame()
        {
            if (_gameplayStatus != GameplayStatus.StartGame)
            {
                return;
            }
            
            _gameplayStatus = GameplayStatus.PauseGame;
        }
        
        private void OnLevelFailed()
        {
            if (_gameplayStatus != GameplayStatus.StartGame)
            {
                return;
            }
            
            _gameplayStatus = GameplayStatus.LevelFailed;
        }
        
        private void OnLevelCompleted()
        {
            if (_gameplayStatus != GameplayStatus.StartGame)
            {
                return;
            }
            
            _gameplayStatus = GameplayStatus.LevelCompleted;
        }
    }
}