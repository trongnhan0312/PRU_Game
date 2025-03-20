using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    public float speed = 2f;  // Tốc độ dao động
    public float height = 0.5f;  // Độ cao dao động




    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * height;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
