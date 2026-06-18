using UnityEngine;

public static class GeometryUtils
{
    public static float GetAngleToPlane(Vector3 direction, Vector3 planeNormal)
    {
        float dot = Vector3.Dot(direction.normalized, planeNormal.normalized);
        float angleToNormal = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f)) * Mathf.Rad2Deg;
        return Mathf.Abs(90f - angleToNormal);
    }
}
