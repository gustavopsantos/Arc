using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public readonly struct Arc
{
    private readonly Vector3 _normal;
    private readonly Vector3 _from;
    private readonly float _sweepAngle;
    
    public Vector3 Start { get; }
    public Vector3 Middle { get; }
    public Vector3 End { get; }

    public Arc(Vector3 normal, Vector3 from, float sweepAngle)
    {
        _normal = normal;
        _from = from;
        _sweepAngle = sweepAngle;
        
        Start = _from;
        Middle = Quaternion.AngleAxis(sweepAngle / 2, normal) * _from;
        End = Quaternion.AngleAxis(sweepAngle, normal) * _from;
    }
    
    public Arc Rotate(float angle)
    {
        return new Arc(_normal, Quaternion.AngleAxis(angle, _normal) * _from, _sweepAngle);
    }
    
    public Arc Resize(float degrees)
    {
        var from = Quaternion.AngleAxis(-degrees / 2f, _normal) * _from;
        var sweepAngle = _sweepAngle + degrees;
        return new Arc(_normal, from, sweepAngle);
    }

    public Arc Clamp(Arc outer)
    {
        return Clamp(this, outer);
    }

    public static Arc Clamp(Arc inner, Arc outer)
    {
        Assert.IsTrue(inner._normal == outer._normal, "Arcs must have the same normal");
        
        if (inner._sweepAngle > outer._sweepAngle)
        {
            return outer;
        }

        if (TryGetOvershoot(inner, outer, inner._normal, out var overshoot))
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
        Handles.DrawSolidArc(Vector3.zero, _normal, _from, _sweepAngle, 1);
    }
}