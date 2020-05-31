using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier : MonoBehaviour {
    public GameObject rod;
    public GameObject bobber;
    private float waterHeight;
    private LineRenderer renderer;
    private Vector3 point1;
    private Vector3 point0;
    private Vector3 point2;
    private float t;
    // Use this for initialization
    void Start () {
        renderer = GetComponent<LineRenderer>();
        waterHeight = GameObject.FindGameObjectWithTag("Water").transform.position.y;
    }
	
	// Update is called once per frame
	void Update () {
        point1 = new Vector3((rod.transform.position.x + bobber.transform.position.x)/2, bobber.transform.position.y, (rod.transform.position.z + bobber.transform.position.z)/2);
        point0 = rod.transform.position;
        point2 = bobber.transform.position;
        Vector3[] pos;
        int state = bobber.GetComponent<Bobber>().GetFlag();
        if (state == 0 || state >= 5)
        {
            pos = calculateLinePoints(100);
        }
        else
            pos = lineEquation(100);
        draw(pos);
    }
    private void draw(Vector3[] positions) {
        renderer.positionCount = positions.Length;
        renderer.SetPositions(positions);
    }

    private Vector3[] lineEquation(int numOfSteps) {
        Vector3[] linePoints = new Vector3[numOfSteps];
        linePoints[0] = point0;
        for(int i = 1; i < numOfSteps; i++)
        {

            float t = (i + 1) / (float)numOfSteps;
            linePoints[i] = calculateBezierPoint(t, point0, point1, point2);
            if (linePoints[i].y < waterHeight)
                linePoints[i].y = waterHeight;
            else
            {
                linePoints[i].x = (float)System.Math.Round(linePoints[i].x, 2);
                linePoints[i].y = (float)System.Math.Round(linePoints[i].y, 2);
                linePoints[i].z = (float)System.Math.Round(linePoints[i].z, 2);
            }
        }

        return linePoints;
    }

    private Vector3 calculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

    private Vector3[] calculateLinePoints(int amount)
    {

        float step_x = Mathf.Abs(point0.x - point2.x)/amount;
        float step_y = Mathf.Abs(point0.y - point2.y) / amount;
        float step_z = Mathf.Abs(point0.z - point2.z) / amount;
        Vector3[] positions = new Vector3[amount];
        positions[0] = point0;
        if (point0.x > point2.x)
            step_x = -step_x;
        if (point0.y > point2.y)
            step_y = -step_y;
        if (point0.z > point2.z)
            step_z = -step_z;
        for (int i = 1; i < amount; ++i)
        {
            positions[i] = positions[i - 1] + new Vector3(step_x, step_y, step_z);
        }
        return positions;
    }
}
