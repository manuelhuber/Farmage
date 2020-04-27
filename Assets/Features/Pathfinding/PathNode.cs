namespace Features.Pathfinding {
/// <summary>
/// Contains the path-related info for a node
/// This struct will be instantiated per path
/// </summary>
public struct PathNode {
    /// <summary>
    /// The same index as the associated GridNode
    /// </summary>
    public int Index;

    /// <summary>
    /// The cost it took to get from start to this node
    /// </summary>
    public int ArrivalCost;

    /// <summary>
    /// The estimated cost of going from this node to the end node
    /// </summary>
    public int HeuristicCost;

    /// <summary>
    /// ArrivalCost + HeuristicCost
    /// </summary>
    public int TotalCost;

    /// <summary>
    /// The index of the best node from which to get to this node
    /// </summary>
    public int CameFromNodeIndex;

    public void UpdateTotalCost() {
        TotalCost = ArrivalCost + HeuristicCost;
    }
}
}