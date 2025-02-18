using UnityEngine;
using System;
public class Point
{
    public float X, Y;
    public object data;
    public bool colliding;

    public Point(float x, float y, object data)
    {
        X = x;
        Y = y;
        this.data = data;
    }

    public float Radius() 
    {
        GameObject dataGo = data as GameObject;
        return dataGo!.transform.localScale.x * 1.25f;
    }

    public override bool Equals(object obj)
    {
        if (obj is Point other)
        {
            if (data != null || other.data != null)
                return data == other.data;
            float epsilon = 0.0001f;
            return Mathf.Abs(X - other.X) < epsilon && Mathf.Abs(Y - other.Y) < epsilon;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}