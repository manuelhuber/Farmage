using System;
using Grimity.Loops;
using UnityEngine;

namespace Features.Terrain {
public class PathfindingGrid : MonoBehaviour {
    public float[,] _heightMap;
    public float cellSize;

    private void OnDrawGizmos() {
        var hasNoMap = _heightMap == null;
        if (hasNoMap) return;
        var width = _heightMap.GetLength(0);
        var height = _heightMap.GetLength(1);
        new Loop2D(width, height).loopX((x, y) => {
            var position = new Vector3(x * cellSize - cellSize / 2, _heightMap[x, y], y * cellSize - cellSize / 2);
            var size = new Vector3(cellSize, .100f, cellSize);
            Gizmos.DrawCube(position, size);
        });
    }
}
}