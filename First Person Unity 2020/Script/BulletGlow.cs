using UnityEngine;

public class BulletGlow : MonoBehaviour
{
    public float glowDelay = 2f; // Verz�gerung in Sekunden, bevor das Leuchten beginnt, �ffentlich zug�nglich
    public Color glowColor = Color.yellow; // Farbe des Leuchtens, �ffentlich zug�nglich
    public float glowIntensity = 2f; // Intensit�t des Leuchtens, �ffentlich zug�nglich

    private Material bulletMaterial;
    private bool isGlowing = false;

    void Start()
    {
        // Stelle sicher, dass das Projektil-Material die Emissionseigenschaft unterst�tzt
        bulletMaterial = GetComponent<Renderer>().material;
        bulletMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (!isGlowing && glowDelay > 0f)
        {
            // Startet das Leuchten nach der Verz�gerung
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
        // Sorgt daf�r, dass das Leuchten auch ohne direkte Beleuchtung sichtbar ist
        DynamicGI.SetEmissive(GetComponent<Renderer>(), glowColor * glowIntensity);
    }
}
