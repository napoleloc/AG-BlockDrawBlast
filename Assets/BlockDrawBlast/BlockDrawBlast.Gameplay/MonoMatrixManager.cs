using EncosyTower.Collections;
using UnityEngine;

namespace BlockDrawBlast.Gameplay
{
    public class MonoMatrixManager : MonoBehaviour
    {
        private readonly ArrayMap<Vector2Int, TileData> _coordToTileData = new ArrayMap<Vector2Int, TileData>();
        private readonly ArrayMap<Vector2Int, BlockData> _coordToBlockData = new ArrayMap<Vector2Int, BlockData>();
        
        
    }
}

