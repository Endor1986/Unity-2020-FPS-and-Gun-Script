using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed = 1000f; // Geschwindigkeit des Geschosses
    public float lifetime = 3f; // Lebensdauer des Geschosses in Sekunden

    void Start()
    {
        // Setzt die Bewegung des Geschosses
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }

        // Zerst�rt das Geschoss automatisch nach der angegebenen Lebensdauer
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // F�ge hier die Logik f�r den Aufprall hinzu, z.B. Schaden verursachen
        Debug.Log("Bullet hit: " + other.name);

        // Zerst�rt das Geschoss bei Kollision
        Destroy(gameObject);
    }
}