using System.Threading;
using BlockDrawBlast.Database;
using BlockDrawBlast.Shared;
using Cysharp.Threading.Tasks;
using EncosyTower.Ids;
using EncosyTower.Logging;
using EncosyTower.Vaults;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoGameplayManager : MonoBehaviour
    {
        [Title("Debugging", titleAlignment: TitleAlignments.Centered)] 
        [SerializeField, ReadOnly] private GameplayStatus _gameplayStatus;
        
        private MonoWorldCamera _worldCamera;
        private MonoInputReceiver _inputReceiver;
        
        private MonoDrawingManaged _monoDrawingManaged;
        private MonoMatrixManaged _monoMatrixManaged;
        private MonoStackManaged _monoStackManaged;
        
        // Database
        private LevelDataTableAsset _levelDataTable;
        
        private int _currentLevel;

        private async void Awake()
        {
            await InitializeAsync();
        }

        private async UniTask InitializeAsync()
        {
            using var initCts = new CancellationTokenSource();

            _worldCamera = await GetAsync(MonoWorldCamera.TypeId, initCts.Token);
            _inputReceiver = await GetAsync(MonoInputReceiver.TypeId, initCts.Token);
            
            _monoDrawingManaged = await GetAsync(MonoDrawingManaged.TypeId, initCts.Token);
            _monoMatrixManaged = await GetAsync(MonoMatrixManaged.TypeId, initCts.Token);
            _monoStackManaged = await GetAsync(MonoStackManaged.TypeId, initCts.Token);
        }

        private async UniTask<T> GetAsync<T>(Id<T> typeId, CancellationToken token)
        {
            var valueTOpt = await GlobalObjectVault.TryGetAsync(typeId, this, token);
            if (valueTOpt.TryValue(out var valueT) != false) return valueT;
            
            DevLoggerAPI.LogError("Failed to get object");
            return default;

        } 
        
        private void Start()
        {
            _inputReceiver.OnTouchStart += HandleTouchStart;
            _inputReceiver.OnTouchMove += HandleTouchMove;
            _inputReceiver.OnTouchEnd += HandleTouchEnd;
        }
        
        private void OnDestroy()
        {
            _inputReceiver.OnTouchStart -= HandleTouchStart;
            _inputReceiver.OnTouchMove -= HandleTouchMove;
            _inputReceiver.OnTouchEnd -= HandleTouchEnd;
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