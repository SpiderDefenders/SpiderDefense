using System;

public class MovementEventArgs : EventArgs
{
    public string ID { get; }
    public bool IsMoving { get; }
    public bool? IsMovingX { get; }
    public bool? IsMovingZ { get; }

    public MovementEventArgs(string ID, bool isMoving, bool? isMovingX, bool? isMovingZ)
    {
        this.ID = ID;
        IsMoving = isMoving;
        IsMovingX = isMovingX;
        IsMovingZ = isMovingZ;
    }
}
