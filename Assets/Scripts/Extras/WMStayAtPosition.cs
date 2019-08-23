using UnityEngine;

[DisallowMultipleComponent]
public class WMStayAtPosition : MonoBehaviour
{
    public Vector2 Position = Vector2.zero;
    private void Awake() => transform.position = Position;
    private void Update() => transform.position = Position;
}