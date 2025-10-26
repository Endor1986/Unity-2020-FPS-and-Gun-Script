<p align="center">
  <img alt="Unity" src="https://img.shields.io/badge/Unity-2021%2B-000000?logo=unity&logoColor=white">
  <img alt="C#" src="https://img.shields.io/badge/C%23-10%2B-239120?logo=csharp&logoColor=white">
  <img alt="License" src="https://img.shields.io/badge/License-MIT-lightgrey.svg">
</p>

# Gun Script for Unity

Beispielskript für eine **einfache Schussmechanik** in Unity.  
Demonstriert **Schießen**, **Nachladen**, **Zielen/Zoom**, **Rückstoß** sowie **Mündungsblitz/Patronenhülsen** inkl. grundlegender Physik.

> ⚠️ **Hinweis:** Das Script ist **zu Demonstrationszwecken** gedacht (Portfolio/Showcase) und kann veraltete Praktiken enthalten. Für Produktionscode bitte an eigene Standards/Architektur (Input System, Animation Rigging, Object Pooling etc.) anpassen.

---

## ✨ Features

- **Schussmechanik** mit Mündungsblitz, Projektil-Spawn & Audio
- **Nachladen** (Magazinlogik, einfache Timings)
- **Zielen/Zoom** (Kamera-FOV anpassen)
- **Rückstoß** (Impuls + langsame Erholung)
- **Patronenhülsen** mit einfacher Physik

---

## 🚀 Quickstart

1) **Unity-Projekt öffnen** (Unity 2021+ empfohlen)  
2) Ordner/Dateien in dein Projekt kopieren:
   ```
   Assets/
   ├─ Scripts/
   │  └─ Gun.cs
   ├─ Prefabs/
   │  ├─ Bullet.prefab
   │  ├─ MuzzleFlash.prefab
   │  └─ Casing.prefab
   └─ Audio/
      ├─ shoot.wav
      └─ reload.wav
   ```
3) In der **Scene**:
   - Lege ein **Gun**-GameObject an (z. B. unter der Kamera/Hand).
   - Hänge `Gun.cs` an das GameObject.
   - Weisen im **Inspector** die Felder zu: `Bullet`, `MuzzleFlash`, `Casing`, `ShootSfx`, `ReloadSfx`, `FirePoint` (Transform), `CasingEjectPoint` (Transform).
4) **Eingaben** (altes Input Manager-System):
   - `Fire1` (Linksklick / Maus 0)
   - `Fire2` (Rechtsklick / Maus 1)
   - `R` zum Nachladen
5) **Play** drücken – schießen, zielen, nachladen testen.

> Wenn du das neue **Input System** verwendest, ersetze die Input-Abfragen entsprechend (Actions/Bindings) oder nutze `PlayerInput`.

---

## 🔧 Inspector-Parameter (Beispiel)

| Feld                 | Typ        | Beschreibung                          |
|:---------------------|:----------:|---------------------------------------|
| Bullet               | `GameObject` | Projektil-Prefab (mit Rigidbody)     |
| MuzzleFlash          | `GameObject` | Mündungsblitz (Partikeleffekt/Prefab)|
| Casing               | `GameObject` | Patronenhülse (mit Rigidbody)        |
| FirePoint            | `Transform`  | Spawn-Punkt für Projektile           |
| CasingEjectPoint     | `Transform`  | Auswurfpunkt der Hülse               |
| Damage               | `float`      | Schaden pro Treffer                   |
| MuzzleVelocity       | `float`      | Anfangsgeschwindigkeit der Kugel      |
| MagazineSize         | `int`        | Patronen pro Magazin                  |
| ReloadTime           | `float`      | Sek. für Nachladevorgang              |
| RecoilKick           | `Vector2`    | Recoil (Pitch/Yaw) beim Schuss        |
| RecoilRecovery       | `float`      | Erholungsgeschwindigkeit              |
| AimFov               | `float`      | Kamera-FOV beim Zielen                |
| AimSpeed             | `float`      | Geschwindigkeit FOV-Übergang          |

---

## 📄 Beispielskript (`Gun.cs` – vereinfachtes Demo)

```csharp
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("References")]
    public Transform FirePoint;
    public Transform CasingEjectPoint;
    public GameObject Bullet;
    public GameObject MuzzleFlash;
    public GameObject Casing;
    public AudioSource Audio;
    public AudioClip ShootSfx;
    public AudioClip ReloadSfx;
    public Camera PlayerCam;

    [Header("Ballistics")]
    public float MuzzleVelocity = 60f;
    public float Damage = 10f;

    [Header("Magazine")]
    public int MagazineSize = 12;
    public float ReloadTime = 1.2f;

    [Header("Recoil")]
    public Vector2 RecoilKick = new Vector2(2.0f, 0.8f);
    public float RecoilRecovery = 6f;

    [Header("Aim")]
    public float AimFov = 45f;
    public float AimSpeed = 12f;

    private int _ammo;
    private bool _reloading;
    private float _baseFov;
    private Vector2 _recoilOffset; // x=pitch, y=yaw

    void Start()
    {
        _ammo = MagazineSize;
        if (PlayerCam != null) _baseFov = PlayerCam.fieldOfView;
    }

    void Update()
    {
        HandleAim();
        HandleInput();
        RecoverRecoil();
    }

    void HandleInput()
    {
        if (_reloading) return;

        if (Input.GetButtonDown("Fire1"))
            TryShoot();

        if (Input.GetKeyDown(KeyCode.R))
            StartReload();
    }

    void HandleAim()
    {
        if (PlayerCam == null) return;
        float target = Input.GetButton("Fire2") ? AimFov : _baseFov;
        PlayerCam.fieldOfView = Mathf.Lerp(PlayerCam.fieldOfView, target, Time.deltaTime * AimSpeed);
    }

    void RecoverRecoil()
    {
        // sanfte Rückkehr Richtung 0
        _recoilOffset = Vector2.Lerp(_recoilOffset, Vector2.zero, Time.deltaTime * RecoilRecovery);
        transform.localRotation = Quaternion.Euler(_recoilOffset.x, _recoilOffset.y, 0f);
    }

    void TryShoot()
    {
        if (_ammo <= 0) { StartReload(); return; }

        // Projektil
        if (Bullet && FirePoint)
        {
            var go = Instantiate(Bullet, FirePoint.position, FirePoint.rotation);
            var rb = go.GetComponent<Rigidbody>();
            if (rb) rb.velocity = FirePoint.forward * MuzzleVelocity;
        }

        // Mündungsblitz (Auto-Destroy)
        if (MuzzleFlash && FirePoint)
            Destroy(Instantiate(MuzzleFlash, FirePoint.position, FirePoint.rotation), 1f);

        // Hülse
        if (Casing && CasingEjectPoint)
        {
            var c = Instantiate(Casing, CasingEjectPoint.position, CasingEjectPoint.rotation);
            var rb = c.GetComponent<Rigidbody>();
            if (rb) rb.AddForce(CasingEjectPoint.right * Random.Range(1.5f, 2.5f), ForceMode.Impulse);
            Destroy(c, 5f);
        }

        // Audio
        if (Audio && ShootSfx) Audio.PlayOneShot(ShootSfx);

        // Recoil (einfach)
        _recoilOffset.x -= RecoilKick.x;
        _recoilOffset.y += Random.Range(-RecoilKick.y, RecoilKick.y);

        _ammo--;
    }

    void StartReload()
    {
        if (_reloading || _ammo == MagazineSize) return;
        StartCoroutine(ReloadRoutine());
    }

    System.Collections.IEnumerator ReloadRoutine()
    {
        _reloading = true;
        if (Audio && ReloadSfx) Audio.PlayOneShot(ReloadSfx);
        yield return new WaitForSeconds(ReloadTime);
        _ammo = MagazineSize;
        _reloading = false;
    }
}
```

> Für Produktionscode empfiehlt sich **Object Pooling** (statt `Instantiate/Destroy`), **scriptable Weapon Configs**, **Raycast-Hitscan** für Hits, und das **neue Input System**.

---

## 🧰 Troubleshooting

- **Nichts passiert bei Klick:** Prüfe, ob `Gun.cs` am richtigen Objekt hängt und `Fire1/Fire2` im Input Manager belegt sind.  
- **Kamera zoomt nicht:** `PlayerCam` im Inspector zuweisen, `AimFov` < Standard-FOV wählen.  
- **Kugeln fallen nach unten:** `Bullet` benötigt `Rigidbody` und wird mit `FirePoint.forward` beschleunigt.  
- **Leistung schlecht:** Für häufiges Schießen **Pooling** verwenden; Effekte kürzer leben lassen.

---

## ✅ Status

- Funktionsfähige Demo einer Schusswaffe (Single Fire)  
- Leicht **erweiterbar** (Feuerraten, Automatik, Spread, Trefferfeedback/Decals, Ammo-Typen)  
- Geeignet als **Showcase/Portfolio** für Gameplay-Scripting

---

## 📜 Lizenz

MIT – siehe `LICENSE`.
