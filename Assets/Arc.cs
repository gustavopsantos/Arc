using UnityEditor;
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
        Start = start;
        Normal = normal;
        SweepAngle = sweepAngle;

        Middle = Quaternion.AngleAxis(sweepAngle / 2, normal) * start;
        End = Quaternion.AngleAxis(sweepAngle, normal) * start;
    }

    public Arc Rotate(float angle)
    {
        return new Arc(Quaternion.AngleAxis(angle, Normal) * Start, Normal, SweepAngle);
    }

    public Arc Resize(float degrees)
    {
        var from = Quaternion.AngleAxis(-degrees / 2f, Normal) * Start;
        var sweepAngle = SweepAngle + degrees;
        return new Arc(from, Normal, sweepAngle);
    }

    public Arc Clamp(Arc outer)
    {
        return Clamp(this, outer);
    }

    public static Arc Clamp(Arc inner, Arc outer)
    {
        Assert.IsTrue(inner.Normal == outer.Normal, "Arcs must have the same normal");

        if (inner.SweepAngle > outer.SweepAngle)
        {
            return outer;
        }

        if (TryGetOvershoot(inner, outer, inner.Normal, out var overshoot))
        {
            return inner.Rotate(-overshoot);
        }

        return inner;
    }

    private static bool TryGetOvershoot(Arc inner, Arc outer, Vector3 normal, out float overshoot)
    {
        var outerTangent = Vector3.Cross(outer.Middle, -normal);
        var innerSideLocalToOuter = Vector3.Dot(inner.Middle, outerTangent);

        if (innerSideLocalToOuter >= 0) // Inner its on outer right quadrant
        {
            overshoot = -Vector3.SignedAngle(inner.End, outer.End, normal);

            if (overshoot > 0)
            {
                return true;
            }
        }
        else // Inner its on outer left quadrant
        {
            overshoot = -Vector3.SignedAngle(inner.Start, outer.Start, normal);

            if (overshoot < 0)
            {
                return true;
            }
        }

        return false;
    }

    public void Draw(Color color)
    {
        Handles.color = color;
        Handles.DrawSolidArc(Vector3.zero, Normal, Start, SweepAngle, 1);
    }
}