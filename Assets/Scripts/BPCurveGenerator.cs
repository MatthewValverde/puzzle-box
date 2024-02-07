using System.Collections.Generic;
using UnityEngine;

public static class BPCurveGenerator
{
    public static List<Vector3> GenerateSmoothCurve(Vector3[] points, int resolution)
    {
        List<Vector3> curvePoints = new List<Vector3>();

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 p0 = i > 0 ? points[i - 1] : points[i];
            Vector3 p1 = points[i];
            Vector3 p2 = i < points.Length - 1 ? points[i + 1] : p1;
            Vector3 p3 = i < points.Length - 2 ? points[i + 2] : p2;

            for (int j = 0; j < resolution; j++)
            {
                float t = j / (float)resolution;
                Vector3 point = CalculateCatmullRomPosition(t, p0, p1, p2, p3);
                curvePoints.Add(point);
            }
        }

        return curvePoints;
    }

    private static Vector3 CalculateCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float t0 = ((-t + 2) * t - 1) * t * 0.5f;
        float t1 = (((3 * t - 5) * t) * t + 2) * 0.5f;
        float t2 = ((-3 * t + 4) * t + 1) * t * 0.5f;
        float t3 = ((t - 1) * t * t) * 0.5f;

        return p0 * t0 + p1 * t1 + p2 * t2 + p3 * t3;
    }
}

