using UnityEngine;

public class BulletGlow : MonoBehaviour
{
    public float glowDelay = 2f; // Verzögerung in Sekunden, bevor das Leuchten beginnt, öffentlich zugänglich
    public Color glowColor = Color.yellow; // Farbe des Leuchtens, öffentlich zugänglich
    public float glowIntensity = 2f; // Intensität des Leuchtens, öffentlich zugänglich

    private Material bulletMaterial;
    private bool isGlowing = false;

    void Start()
    {
        // Stelle sicher, dass das Projektil-Material die Emissionseigenschaft unterstützt
        bulletMaterial = GetComponent<Renderer>().material;
        bulletMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (!isGlowing && glowDelay > 0f)
        {
            // Startet das Leuchten nach der Verzögerung
            glowDelay -= Time.deltaTime;
        }
        else if (!isGlowing)
        {
            StartGlowing();
            isGlowing = true;
        }
    }

    public void StartGlowing()
    {
        // Setzt die Emissionseigenschaft des Materials, um das Leuchten zu starten
        bulletMaterial.SetColor("_EmissionColor", glowColor * glowIntensity);
        // Sorgt dafür, dass das Leuchten auch ohne direkte Beleuchtung sichtbar ist
        DynamicGI.SetEmissive(GetComponent<Renderer>(), glowColor * glowIntensity);
    }
}
