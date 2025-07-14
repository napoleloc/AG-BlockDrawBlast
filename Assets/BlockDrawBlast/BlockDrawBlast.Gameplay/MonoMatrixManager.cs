using System.Runtime.CompilerServices;
using EncosyTower.Collections;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace BlockDrawBlast.Gameplay
{
    public class MonoMatrixManager : MonoBehaviour
    {
        [SerializeField] private int _rowCount = 10;
        [SerializeField] private int _columnCount = 10;
        [SerializeField] private float _tileSize = 1.0f;
        [SerializeField] private Vector3 _gridOrigin = Vector3.zero;
        
        private readonly ArrayMap<Vector2Int, TileData> _coordToTileData = new ArrayMap<Vector2Int, TileData>();
        private readonly ArrayMap<Vector2Int, BlockData> _coordToBlockData = new ArrayMap<Vector2Int, BlockData>();

        private MatrixDataArrays _matrixDataArrays;
        private NativeArray<int> _keyInventory;
        private bool _isInitialized;

        // Job handles
        private JobHandle _currentJobHandle;
        
        public int RowCount => _rowCount;
        public int ColumnCount => _columnCount;

        private void Initialize(int rows, int columns)
        {
            if(_isInitialized) return;
            
            _rowCount = rows;
            _columnCount = columns;
            
            var lenght = rows * columns;
            _matrixDataArrays.Initialize(lenght, Allocator.Persistent);
        }
        
        private void InitializeTiles()
        {
            for (int row = 0; row < _rowCount; row++)
            {
                for (int col = 0; col < _columnCount; col++)
                {
                    var coord = new Vector2Int(col, row);
                    var worldPos = GridToWorldPosition(coord);
                    
                    var tileData = new TileData
                    {
                        row = row,
                        column = col,
                        tileState = TileState.Empty,
                        worldPosition = worldPos
                    };
                    
                    _coordToTileData[coord] = tileData;
                }
            }
        }

        public bool TryPlaceBlock(Vector2Int coord, ColorType colorType, out BlockData blockData)
        {
            blockData = default;

            if (_coordToTileData.TryGetValue(coord, out var tileData) == false
                || tileData.tileState != TileState.Empty)
            {
                return false;
            }
            
            var worldPos = GridToWorldPosition(coord);
            blockData = BlockData.CreateNormal(coord.y, coord.x, colorType, worldPos);
            
            _coordToBlockData[coord] = blockData;

            tileData.tileState = TileState.Occupied;
            
            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetGridIndex(int2 position)
        {
            return position.y * _columnCount + position.x;
        }
        
        private float3 GridToWorldPosition(Vector2Int coord)
        {
            return new float3(
                _gridOrigin.x + coord.x * _tileSize,
                _gridOrigin.y,
                _gridOrigin.z + coord.y * _tileSize
            );
        }
        
        private NativeArray<BlockData> ConvertToBlockDataArray()
        {
            var blockArray = new NativeArray<BlockData>(_matrixDataArrays.count, Allocator.TempJob);
            
            for (int i = 0; i < _matrixDataArrays.count; i++)
            {
                blockArray[i] = new BlockData
                {
                    Row = _matrixDataArrays.positions[i].y,
                    Column = _matrixDataArrays.positions[i].x,
                    ColorType = _matrixDataArrays.colors[i],
                    BlockType = _matrixDataArrays.types[i],
                    RequiredKeyCount = _matrixDataArrays.keyRequirements[i],
                    Health = _matrixDataArrays.healths[i],
                    WorldPosition = _matrixDataArrays.worldPositions[i]
                };
            }
            
            return blockArray;
        }
        
        public NativeArray<BlockData> GetAllBlocks()
        {
            return ConvertToBlockDataArray();
        }
        
        public bool TryGetBlockAt(Vector2Int coord, out BlockData blockData)
        {
            return _coordToBlockData.TryGetValue(coord, out blockData);
        }

    }
}

