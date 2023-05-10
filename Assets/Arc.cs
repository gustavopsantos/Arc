using UnityEngine;
using UnityEngine.Assertions;

public readonly struct Arc
{
    public Vector3 Normal { get; }
    public float SweepAngle { get; }
    public Vector3 Start { get; }
    public Vector3 Middle { get; }
    public Vector3 End { get; }

    public Arc(Vector3 start, Vector3 normal, float sweepAngle)
    {
        Assert.IsTrue(sweepAngle >= 0, "Sweep angle must be positive");
        
        Start = start;
        Normal = normal;
        SweepAngle = sweepAngle;

        Middle = Quaternion.AngleAxis(sweepAngle / 2, normal) * start;
        End = Quaternion.AngleAxis(sweepAngle, normal) * start;
    }
}