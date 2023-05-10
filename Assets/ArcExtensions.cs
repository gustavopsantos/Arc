using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public static class ArcExtensions
{
    public static Arc Rotate(this Arc arc, float angle)
    {
        return new Arc(Quaternion.AngleAxis(angle, arc.Normal) * arc.Start, arc.Normal, arc.SweepAngle);
    }

    public static Arc Resize(this Arc arc, float degrees)
    {
        var from = Quaternion.AngleAxis(-degrees / 2f, arc.Normal) * arc.Start;
        var sweepAngle = arc.SweepAngle + degrees;
        return new Arc(from, arc.Normal, sweepAngle);
    }

    public static void Draw(this Arc arc, Color color)
    {
        Handles.color = color;
        Handles.DrawSolidArc(Vector3.zero, arc.Normal, arc.Start, arc.SweepAngle, 1);
    }
    
    public static Arc Clamp(this Arc inner, Arc outer)
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
}