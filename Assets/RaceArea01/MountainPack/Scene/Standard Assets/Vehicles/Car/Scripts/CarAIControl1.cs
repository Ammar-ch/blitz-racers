using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarAIControl1 : MonoBehaviour
    {
        public enum BrakeCondition
        {
            NeverBrake,
            TargetDistance,
        }

        [SerializeField] [Range(0, 1)] private float m_CautiousSpeedFactor = 0.0f;
        [SerializeField] private float m_CautiousMaxDistance = 50f;
        [SerializeField] private float m_SteerSensitivity = 0.2f;
        [SerializeField] private float m_AccelSensitivity = 0.5f;
        [SerializeField] private float m_BrakeSensitivity = 1f;
        [SerializeField] private bool m_Driving = true;
        [SerializeField] private Transform m_Target;
        [SerializeField] private bool m_StopWhenTargetReached;
        [SerializeField] private float m_ReachTargetThreshold = 8f;

        private float m_RandomPerlin;
        private CarController m_CarController;
        private Rigidbody m_Rigidbody;

        public Transform[] trackingMarkers;
        private int currentMarkerIndex = 0;

        public Transform[] roadCenters;

        private Vector3 lastPosition;
        private float respawnCooldownTime = 3.0f;
        private float nextRespawnAllowedTime = 0f;
        private bool isRespawning = false;
        private float startTime; // Zeit, bei der das Spiel gestartet wurde

        private void Awake()
        {
            m_CarController = GetComponent<CarController>();
            m_RandomPerlin = Random.value * 100;
            m_Rigidbody = GetComponent<Rigidbody>();
            lastPosition = transform.position;

            startTime = Time.time; // Initialisiere Startzeit

            if (trackingMarkers.Length > 0)
            {
                SetTarget(trackingMarkers[0]);
                Debug.Log("Erster Tracking Marker gesetzt: " + trackingMarkers[0].name);
            }
            else
            {
                Debug.LogError("trackingMarkers array is empty! Please assign tracking markers.");
            }
        }

        private void FixedUpdate()
        {
            if (m_Target == null || !m_Driving)
            {
                m_CarController.Move(0, 0, -1f, 1f);
            }
            else
            {
                if (Time.time >= nextRespawnAllowedTime && ShouldRespawn())
                {
                    StartCoroutine(RespawnCar());
                }
                else
                {
                    DriveTowardsTarget();
                    CheckIfReachedTarget();
                }
            }

            // Aktualisiere die letzte Position, wenn das Fahrzeug sich bewegt
            if (m_Rigidbody.velocity.magnitude > 0.5f)
            {
                lastPosition = transform.position;
            }
        }

        private void DriveTowardsTarget()
        {
            Vector3 fwd = transform.forward;

            if (m_Rigidbody.velocity.magnitude > m_CarController.MaxSpeed * 0.1f)
            {
                fwd = m_Rigidbody.velocity;
            }

            float desiredSpeed = m_CarController.MaxSpeed;

            float accelBrakeSensitivity = (desiredSpeed < m_CarController.CurrentSpeed) ? m_BrakeSensitivity : m_AccelSensitivity;
            float accel = Mathf.Clamp((desiredSpeed - m_CarController.CurrentSpeed) * accelBrakeSensitivity, -1, 1);

            Vector3 localTarget = transform.InverseTransformPoint(m_Target.position);
            float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            float steer = Mathf.Clamp(targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign(m_CarController.CurrentSpeed);

            m_CarController.Move(steer, accel, accel, 0f);
        }

        private void CheckIfReachedTarget()
        {
            if (Vector3.Distance(transform.position, m_Target.position) < m_ReachTargetThreshold)
            {
                Debug.Log($"Ziel erreicht: Marker {currentMarkerIndex} - {m_Target.name}. Wechsel zum nächsten Ziel.");

                int previousMarkerIndex = currentMarkerIndex;
                currentMarkerIndex = (currentMarkerIndex + 1) % trackingMarkers.Length;

                Debug.Log($"Wechsel von Marker {previousMarkerIndex} zu Marker {currentMarkerIndex}");

                DebugDirectionToNextMarker(); // Debugge die Richtung zur nächsten Markierung

                SetTarget(trackingMarkers[currentMarkerIndex]);
            }
        }

        private bool ShouldRespawn()
        {
            // Verhindere Respawning in den ersten 5 Sekunden nach Spielstart
            if (Time.time - startTime < 5f)
            {
                return false;
            }

            bool isStuck = m_Rigidbody.velocity.magnitude < 0.5f && Vector3.Distance(transform.position, lastPosition) < 1f;
            bool isOffTerrain = transform.position.y < -10f;

            if (isStuck)
            {
                Debug.Log("Respawn benötigt: Fahrzeug ist festgefahren.");
            }
            if (isOffTerrain)
            {
                Debug.Log("Respawn benötigt: Fahrzeug ist außerhalb des Terrains.");
            }

            return isStuck || isOffTerrain;
        }

        private IEnumerator RespawnCar()
        {
            isRespawning = true;

            Debug.Log("Respawn gestartet...");
            yield return new WaitForSeconds(2.0f);

            if (roadCenters.Length > 0)
            {
                Transform closestCenter = GetClosestRoadCenter();
                if (closestCenter != null)
                {
                    transform.position = closestCenter.position;
                    transform.rotation = closestCenter.rotation;
                    Debug.Log("Respawn durchgeführt bei Road Center: " + closestCenter.name);
                }
            }

            nextRespawnAllowedTime = Time.time + respawnCooldownTime;
            isRespawning = false;
            lastPosition = transform.position;
        }

        private Transform GetClosestRoadCenter()
        {
            float minDistance = Mathf.Infinity;
            Transform closestCenter = null;

            foreach (Transform center in roadCenters)
            {
                float distance = Vector3.Distance(lastPosition, center.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCenter = center;
                }
            }

            return closestCenter;
        }

        private void DebugDirectionToNextMarker()
        {
            if (currentMarkerIndex < trackingMarkers.Length - 1)
            {
                Vector3 directionToNext = (trackingMarkers[currentMarkerIndex + 1].position - trackingMarkers[currentMarkerIndex].position).normalized;
                Debug.Log($"Richtung von Marker {currentMarkerIndex} zu Marker {currentMarkerIndex + 1}: {directionToNext}");
            }
        }

        public void SetTarget(Transform target)
        {
            m_Target = target;
            m_Driving = true;
            Debug.Log("Neues Ziel gesetzt: " + target.name);
        }
    }
}
