using UnityEngine;

public class Sandbox : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private Color _viewColor;
    
    [Header("Bounds")]
    [SerializeField] private Color _boundsColor;
    [SerializeField] private float _boundsSweepAngle;
    [SerializeField] private float _boundsOffset;
    
    private Arc _view;
    private Arc _bounds;
    
    private void Start()
    {
        _view = new Arc(Vector3.up, Vector3.forward, 20);
    }

    private void Update()
    {
        GetInput(out var horizontal, out var vertical);
        _bounds = new Arc(Vector3.up, Quaternion.Euler(0, _boundsOffset, 0) * Vector3.forward, _boundsSweepAngle);
        _view = _view.Rotate(horizontal).Resize(vertical).Clamp(_bounds);
    }

    private void OnDrawGizmos()
    {
        _bounds.Draw(_boundsColor);
        _view.Draw(_viewColor);
    }

    private static void GetInput(out float horizontal, out float vertical)
    {
        var mul = Time.deltaTime * 90;
        horizontal = Input.GetAxisRaw("Horizontal") * mul;
        vertical = Input.GetAxisRaw("Vertical") * mul;
    }
}