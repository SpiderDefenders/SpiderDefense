using UnityEngine;

public class ExpandableTile : MonoBehaviour
{
    private AdditionalPathManager manager;
    private Direction direction;

    public void Init(AdditionalPathManager manager, Direction direction)
    {
        this.manager = manager;
        this.direction = direction;
    }

    private void OnMouseDown()
    {
        manager.ExtendPath(direction.Opposite());
    }
}