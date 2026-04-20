using UnityEngine;

public static class GeometryUtils
{
    public static float GetAngleToPlane(Vector3 direction, Vector3 planeNormal)
    {
        // Находим угол между вектором и нормалью в радианах
        float dot = Vector3.Dot(direction.normalized, planeNormal.normalized);

        // Угол между вектором и нормалью (в градусах)
        float angleToNormal = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f)) * Mathf.Rad2Deg;

        // Угол к самой плоскости — это 90 минус угол к нормали
        return Mathf.Abs(90f - angleToNormal);
    }
}
