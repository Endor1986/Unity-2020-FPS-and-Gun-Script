using UnityEngine;
using System.Collections;

public class GunScript : MonoBehaviour
{
    public Transform firePoint;
    public GameObject muzzleFlashEffect;
    public GameObject bulletPrefab; // Prefab f�r das visuelle Geschoss
    public GameObject bulletShellPrefab; // Prefab f�r Patronenh�lsen
    public Transform shellEjectionPort; // Ort, an dem die Patronenh�lsen ausgeworfen werden
    public AudioSource gunAudioSource;
    public Animator _SingleLoad;
    public Camera playerCamera;
    public float shootRate = 0.2f;
    public float reloadTime = 2f;
    public int maxAmmo = 30;
    public float recoilAmount = 5f; // Grad, um den die Waffe beim Schie�en zur�ckst��t
    public float recoilRecoverySpeed = 1f; // Geschwindigkeit, mit der sich die Waffe vom R�cksto� erholt
    public float zoomFOV = 30f;
    public float normalFOV = 60f;
    public float zoomSpeed = 10f;
    public float bulletSpeed = 1000f; // Geschwindigkeit des Geschosses

    private float shootTimer = 0;
    private int currentAmmo;
    private bool isReloading;
    private Quaternion originalRotation;

    void Start()
    {
        currentAmmo = maxAmmo; // Initialisiere die Munition auf Maximum beim Start
        playerCamera.fieldOfView = normalFOV;
        originalRotation = firePoint.localRotation; // Speichere die urspr�ngliche Rotation des firePoint
    }

    void Update()
    {
        shootTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && currentAmmo > 0 && shootTimer >= shootRate && !isReloading)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadMagazine());
        }

        // Steuerung f�r Zoom und Aim-Animation
        if (Input.GetMouseButton(1)) // Rechte Maustaste wird gehalten
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, zoomFOV, Time.deltaTime * zoomSpeed);
            _SingleLoad.SetBool("IsAiming", true);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, Time.deltaTime * zoomSpeed);
            _SingleLoad.SetBool("IsAiming", false);
        }

        // R�cksto� langsam zur�cksetzen
        if (firePoint.localRotation != originalRotation)
        {
            firePoint.localRotation = Quaternion.Lerp(firePoint.localRotation, originalRotation, recoilRecoverySpeed * Time.deltaTime);
        }
    }

    void Shoot()
    {
        shootTimer = 0;
        currentAmmo--;

        gunAudioSource.Play(); // Spielt den Schusssound ab
        _SingleLoad.SetTrigger("Fire"); // Aktiviert die Schussanimation
        InstantiateBullet(); // Instanziiert das Geschoss
        ApplyRecoil(); // F�ge R�cksto� hinzu
        EjectShell(); // Patronenh�lse auswerfen
    }

    void InstantiateBullet()
    {
        GameObject effectInstance = Instantiate(muzzleFlashEffect, firePoint.position, Quaternion.LookRotation(firePoint.forward));
        Destroy(effectInstance, 0.1f); // Zerst�rt den M�ndungsblitz nach kurzer Anzeige

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); // Instanziiert das Geschoss
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.velocity = firePoint.forward * bulletSpeed; // Setzt die Geschwindigkeit des Geschosses
        Destroy(bullet, 5f); // Zerst�rt das Geschoss nach 5 Sekunden, falls es nichts trifft
    }

    void EjectShell()
    {
        GameObject shell = Instantiate(bulletShellPrefab, shellEjectionPort.position, shellEjectionPort.rotation);
        Rigidbody shellRb = shell.GetComponent<Rigidbody>();
        shellRb.AddForce(shellEjectionPort.right * 2f, ForceMode.Impulse); // Einfache Kraft nach rechts
        shellRb.AddTorque(Random.insideUnitSphere * 0.5f, ForceMode.Impulse); // Zuf�lliger Drehimpuls
    }

    void ApplyRecoil()
    {
        firePoint.localRotation *= Quaternion.Euler(-recoilAmount, 0, 0);
    }

    IEnumerator ReloadMagazine()
    {
        isReloading = true;
        _SingleLoad.SetTrigger("Reload"); // Startet die Nachladeanimation
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        _SingleLoad.SetTrigger("EndReload"); // Beendet die Nachladeanimation
    }
}