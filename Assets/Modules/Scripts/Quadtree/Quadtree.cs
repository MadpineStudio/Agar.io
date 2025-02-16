using System;
using System.Collections.Generic;
using UnityEngine;

public class Quadtree
{
    public Rectangle boundary;
    public int capacity;
    public List<Point> points;
    public bool divided;
    public Quadtree northeast, northwest, southeast, southwest;

    public Quadtree(Rectangle boundary, int capacity)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException("capacity must be greater than 0");
        }

        this.boundary = boundary;
        this.capacity = capacity;
        points = new List<Point>();
        divided = false;
    }

    private void Subdivide()
    {
        float x = boundary.X;
        float y = boundary.Y;
        float w = boundary.W / 2;
        float h = boundary.H / 2;

        Rectangle ne = new Rectangle(x + w, y - h, w, h);
        northeast = new Quadtree(ne, capacity);
        Rectangle nw = new Rectangle(x - w, y - h, w, h);
        northwest = new Quadtree(nw, capacity);
        Rectangle se = new Rectangle(x + w, y + h, w, h);
        southeast = new Quadtree(se, capacity);
        Rectangle sw = new Rectangle(x - w, y + h, w, h);
        southwest = new Quadtree(sw, capacity);

        divided = true;
    }
    public bool Insert(Point point)
    {
        if (!boundary.Contains(point))
        {
            return false;
        }

        if (points.Count < capacity && !divided)
        {
            points.Add(point);
            return true;
        }

        if (!divided)
        {
            Subdivide();

            // Redistribui os pontos já inseridos para os filhos
            List<Point> tempPoints = new List<Point>(points);
            points.Clear();
            foreach (Point p in tempPoints)
            {
                northeast.Insert(p);
                northwest.Insert(p);
                southeast.Insert(p);
                southwest.Insert(p);
            }
        }

        return northeast.Insert(point) || northwest.Insert(point) ||
               southeast.Insert(point) || southwest.Insert(point);
    }
    // UPDATE TREE POINTS - SEARCH
    public bool Remove(Point point)
    {
        if (!boundary.Contains(point))
            return false;

        bool removed = points.Remove(point);

        if (divided)
        {
            removed = northeast.Remove(point) || northwest.Remove(point) ||
                      southeast.Remove(point) || southwest.Remove(point) || removed;
        }

        if (removed && divided)
            TryUnsubdivide();

        return removed;
    }

    public bool UpdatePosition(Point oldPoint, Point newPoint)
    {
        if (!Remove(oldPoint))
            return false;
        return Insert(newPoint);
    }

    private void TryUnsubdivide()
    {
        if (!divided)
            return;
        // Atualiza o estado dos nós filhos antes de testar se estão vazios
        int subvidedPoints = CountPoints();
        northeast?.TryUnsubdivide();
        northwest?.TryUnsubdivide();
        southeast?.TryUnsubdivide();
        southwest?.TryUnsubdivide();
        if (northeast.IsEmpty() && northwest.IsEmpty() &&
            southeast.IsEmpty() && southwest.IsEmpty() || subvidedPoints <= capacity)
        {
            points =  GetPoints();
            northeast = null;
            northwest = null;
            southeast = null;
            southwest = null;
            divided = false;
        
        }
    }

    private bool IsEmpty()
    {
        if (points.Count > 0)
            return false;
        if (!divided)
            return true;
        return (northeast == null || northeast.IsEmpty()) &&
               (northwest == null || northwest.IsEmpty()) &&
               (southeast == null || southeast.IsEmpty()) &&
               (southwest == null || southwest.IsEmpty());
    }
    // Em Quadtree.cs
    public List<Point> Query(Rectangle range)
    {
        List<Point> found = new List<Point>();
        if (!boundary.Intersects(range))
            return found;

        foreach (Point p in points)
            if (range.Contains(p))
                found.Add(p);

        if (divided)
        {
            found.AddRange(northeast.Query(range));
            found.AddRange(northwest.Query(range));
            found.AddRange(southeast.Query(range));
            found.AddRange(southwest.Query(range));
        }
        return found;
    }
    public List<Point> QueryCircle(Vector2 center, float radius)
    {
        List<Point> found = new List<Point>();
        if (!CircleIntersectsRectangle(center, radius, boundary))
            return found;
        foreach (Point p in points)
            if (Vector2.Distance(new Vector2(p.X, p.Y), center) <= radius)
                found.Add(p);
        if (divided)
        {
            found.AddRange(northeast.QueryCircle(center, radius));
            found.AddRange(northwest.QueryCircle(center, radius));
            found.AddRange(southeast.QueryCircle(center, radius));
            found.AddRange(southwest.QueryCircle(center, radius));
        }
        return found;
    }

    private bool CircleIntersectsRectangle(Vector2 center, float radius, Rectangle rect)
    {
        float dx = Mathf.Max(Mathf.Abs(center.x - rect.X) - rect.W, 0);
        float dy = Mathf.Max(Mathf.Abs(center.y - rect.Y) - rect.H, 0);
        return (dx * dx + dy * dy) <= radius * radius;
    }
    public int CountPoints()
    {
        int total = points.Count;
        if (divided)
        {
            total += northeast.CountPoints();
            total += northwest.CountPoints();
            total += southeast.CountPoints();
            total += southwest.CountPoints();
        }
        return total;
    }
     public List<Point> GetPoints()
    {
        List<Point> total = points;
        if (divided)
        {
            total.AddRange(northeast.points);
            total.AddRange(northwest.points);
            total.AddRange(southeast.points);
            total.AddRange(southwest.points);
        }
        return total;
    }
}