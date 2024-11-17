using UnityEngine;

public class LadderClimbing : MonoBehaviour
{
    public float climbSpeed = 3.0f;
    private FirstPersonController fpsController;
    private bool isClimbing = false;
    private bool isNearLadder = false; // In der Nähe der tatsächlichen Leiter
    private bool isWithinClimbZone = false; // Innerhalb der größeren Erkennungszone
    private Collider ladderCollider;

    private void Start()
    {
        fpsController = FindObjectOfType<FirstPersonController>(); // Findet den FPS Controller im Spiel
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && (isNearLadder || isWithinClimbZone) && !isClimbing)
        {
            StartClimbing();
        }
        else if (Input.GetKeyDown(KeyCode.E) && isClimbing)
        {
            StopClimbing();
        }

        if (isClimbing)
        {
            float inputVertical = Input.GetAxis("Vertical");
            Vector3 moveDirection = new Vector3(0, inputVertical * climbSpeed, 0);
            transform.Translate(moveDirection * Time.deltaTime, Space.World);

            // Überprüfung, ob der Spieler das Ende der Leiter erreicht hat
            CheckLadderBounds();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isNearLadder = true;
            ladderCollider = other; // Speichert den Collider der Leiter
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("LayerZone"))
        {
            isWithinClimbZone = true; // Spieler ist innerhalb der Erkennungszone, aber nicht unbedingt direkt an der Leiter
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isNearLadder = false;
            if (isClimbing)
            {
                StopClimbing();
            }
            ladderCollider = null; // Setzt den Collider der Leiter zurück
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("LayerZone"))
        {
            isWithinClimbZone = false; // Spieler verlässt die Erkennungszone
            if (isClimbing)
            {
                StopClimbing();
            }
        }
    }

    private void StartClimbing()
    {
        isClimbing = true;
        fpsController.SetClimbing(true);
        AlignPlayerWithLadder();
    }

    private void StopClimbing()
    {
        isClimbing = false;
        fpsController.SetClimbing(false);
    }

    private void AlignPlayerWithLadder()
    {
        if (ladderCollider != null)
        {
            // Spieler in Bezug auf den Mittelpunkt des Ladder-Colliders zentrieren
            Vector3 ladderCenter = ladderCollider.bounds.center;
            transform.position = new Vector3(ladderCenter.x, transform.position.y, ladderCenter.z);
        }
    }

    private void CheckLadderBounds()
    {
        if (ladderCollider != null)
        {
            float ladderTop = ladderCollider.bounds.max.y;
            float ladderBottom = ladderCollider.bounds.min.y;

            if (transform.position.y > ladderTop || transform.position.y < ladderBottom)
            {
                StopClimbingAtBound();
            }
        }
    }

    private void StopClimbingAtBound()
    {
        isClimbing = false;
        fpsController.SetClimbing(false);
    }
}
