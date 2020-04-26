namespace Features.Terrain {
/// <summary>
/// 
/// </summary>
public struct GridNode {
    public int Index;

    public int X;
    public int Z;

    public bool IsWalkable;

    /// <summary>
    /// A cost that will be added when traversing this node
    /// This should be 0 unless you explicitly 
    /// </summary>
    public int Penalty;
}
}