using Unity.Collections;
using Unity.Mathematics;

namespace BlockDrawBlast.Gameplay
{
    public struct MatrixDataArrays
    {
         // Parallel arrays for better cache locality
        public NativeArray<int2> positions;           // Grid positions
        public NativeArray<ColorType> colors;         // Block colors
        public NativeArray<BlockType> types;          // Block types
        public NativeArray<int> healths;              // Block health values
        public NativeArray<int> keyRequirements;     // Required keys for locked blocks
        public NativeArray<float3> worldPositions;   // World coordinates
        public NativeArray<byte> flags;               // Packed flags (active, dirty, etc.)
        
        // Spatial indexing for fast lookups
        public NativeArray<int> gridToIndex;          // Grid position to array index
        public NativeArray<int> indexToGrid;          // Array index to grid position
        
        public int count;
        public int capacity;
        
        public void Initialize(int lenght, Allocator allocator)
        {
            positions = new NativeArray<int2>(lenght, allocator);
            colors = new NativeArray<ColorType>(lenght, allocator);
            types = new NativeArray<BlockType>(lenght, allocator);
            healths = new NativeArray<int>(lenght, allocator);
            keyRequirements = new NativeArray<int>(lenght, allocator);
            worldPositions = new NativeArray<float3>(lenght, allocator);
            flags = new NativeArray<byte>(lenght, allocator);
            
            gridToIndex = new NativeArray<int>(lenght, allocator);
            indexToGrid = new NativeArray<int>(lenght, allocator);
            
            // Initialize with invalid indices
            for (int i = 0; i < lenght; i++)
            {
                gridToIndex[i] = -1;
                indexToGrid[i] = -1;
            }
            
            count = 0;
            capacity = lenght;
        }
        
        public void Dispose()
        {
            if (positions.IsCreated) positions.Dispose();
            if (colors.IsCreated) colors.Dispose();
            if (types.IsCreated) types.Dispose();
            if (healths.IsCreated) healths.Dispose();
            if (keyRequirements.IsCreated) keyRequirements.Dispose();
            if (worldPositions.IsCreated) worldPositions.Dispose();
            if (flags.IsCreated) flags.Dispose();
            if (gridToIndex.IsCreated) gridToIndex.Dispose();
            if (indexToGrid.IsCreated) indexToGrid.Dispose();
        }
    }
}

