using UnityEngine;

public class controller : MonoBehaviour
{
    private Transform[] planets;
    private float[] orbitSpeeds;
    private LineRenderer[] orbitLines;
    private Transform cameraTransform;
    
    private float zoomSpeed = 50f;
    private float rotationSpeed = 100f;
    
    void Start()
    {
        cameraTransform = Camera.main.transform;
        
        // Initialize planets by finding them via tags
        string[] planetNames = { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune", "Pluto" };
        planets = new Transform[planetNames.Length];
        orbitLines = new LineRenderer[planetNames.Length];
        
        orbitSpeeds = new float[] { 47.87f, 35.02f, 29.78f, 24.07f, 13.07f, 9.69f, 6.81f, 5.43f, 4.67f };
        
        for (int i = 0; i < planetNames.Length; i++)
        {
            planets[i] = GameObject.FindGameObjectWithTag(planetNames[i]).transform;
            CreateOrbit(planets[i], i);
        }
    }

    void Update()
    {
        MovePlanets();
        HandleCamera();
    }

    void MovePlanets()
    {
        for (int i = 0; i < planets.Length; i++)
        {
            float speed = orbitSpeeds[i] * Time.deltaTime / 500; // Adjusted for real-time motion
            planets[i].RotateAround(Vector3.zero, Vector3.up, speed);
        }
    }

    void HandleCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cameraTransform.position += cameraTransform.forward * scroll * zoomSpeed * Time.deltaTime;
        
        if (Input.GetMouseButton(1)) // Right mouse button to rotate
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            
            cameraTransform.RotateAround(Vector3.zero, Vector3.up, mouseX);
            cameraTransform.RotateAround(Vector3.zero, cameraTransform.right, -mouseY);
        }
    }

    void CreateOrbit(Transform planet, int index)
    {
        GameObject orbitObject = new GameObject("Orbit_" + planet.name);
        LineRenderer line = orbitObject.AddComponent<LineRenderer>();

        int segments = 100;
        float radius = Vector3.Distance(planet.position, Vector3.zero);
        line.positionCount = segments;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.white;
        line.endColor = Color.white;

        for (int i = 0; i < segments; i++)
        {
            float angle = (2 * Mathf.PI / segments) * i;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            line.SetPosition(i, new Vector3(x, 0, z));
        }

        orbitLines[index] = line;
    }
}
