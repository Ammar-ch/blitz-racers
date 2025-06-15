using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarAIControl : MonoBehaviour
    {
        public enum BrakeCondition
        {
            NeverBrake,
            TargetDirectionDifference,
            TargetDistance,
        }

        [SerializeField] [Range(0, 1)] private float m_CautiousSpeedFactor = 0.05f;
        [SerializeField] [Range(0, 180)] private float m_CautiousMaxAngle = 50f;
        [SerializeField] private float m_CautiousMaxDistance = 100f;
        [SerializeField] private float m_CautiousAngularVelocityFactor = 30f;
        [SerializeField] private float m_SteerSensitivity = 0.05f;
        [SerializeField] private float m_AccelSensitivity = 0.04f;
        [SerializeField] private float m_BrakeSensitivity = 1f;
        [SerializeField] private float m_LateralWanderDistance = 3f;
        [SerializeField] private float m_LateralWanderSpeed = 0.1f;
        [SerializeField] [Range(0, 1)] private float m_AccelWanderAmount = 0.1f;
        [SerializeField] private float m_AccelWanderSpeed = 0.1f;
        [SerializeField] private BrakeCondition m_BrakeCondition = BrakeCondition.TargetDistance;
        [SerializeField] private bool m_Driving;
        [SerializeField] private Transform m_Target;
        [SerializeField] private bool m_StopWhenTargetReached;
        [SerializeField] private float m_ReachTargetThreshold = 2;

        private float m_RandomPerlin;
        private CarController m_CarController;
        private float m_AvoidOtherCarTime;
        private float m_AvoidOtherCarSlowdown;
        private float m_AvoidPathOffset;
        private Rigidbody m_Rigidbody;

        // Respawn variables
        public Transform[] roadCenters;
        private Vector3 lastPosition;
        private float stuckCheckTime = 2.0f;
        private float stuckThreshold = 0.5f;
        private float minSpeedForStuckCheck = 1.0f;
        private float respawnCooldownTime = 3.0f;
        private float nextRespawnAllowedTime = 0f;
        private bool isRespawning = false;
        private float terrainBoundaryY = -10f; // Level unterhalb dessen das Auto als "außerhalb" betrachtet wird

        private void Awake()
        {
            m_CarController = GetComponent<CarController>();
            m_RandomPerlin = Random.value * 100;
            m_Rigidbody = GetComponent<Rigidbody>();
            lastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            if (m_Target == null || !m_Driving)
            {
                m_CarController.Move(0, 0, -1f, 1f);
            }
            else
            {
                // Check if the car is off the terrain or stuck
                if (IsOffTerrain() || (Time.time >= nextRespawnAllowedTime && !isRespawning && m_Rigidbody.velocity.magnitude < minSpeedForStuckCheck && Vector3.Distance(transform.position, lastPosition) < stuckThreshold))
                {
                    Debug.Log("Respawn wird ausgeführt: Auto ist außerhalb des Terrains oder steckt fest.");
                    StartCoroutine(RespawnCar());
                }
                else
                {
                    lastPosition = transform.position; // Update last position if the car is moving
                    DriveTowardsTarget(); // Main AI driving logic
                }
            }
        }

        private bool IsOffTerrain()
        {
            // Prüfen, ob das Auto unterhalb eines bestimmten Y-Levels ist
            bool isOff = transform.position.y < terrainBoundaryY;
            if (isOff)
            {
                Debug.Log("Auto ist außerhalb des Terrains.");
            }
            return isOff;
        }

        private void DriveTowardsTarget()
        {
            Vector3 fwd = transform.forward;
            if (m_Rigidbody.velocity.magnitude > m_CarController.MaxSpeed * 0.1f)
            {
                fwd = m_Rigidbody.velocity;
            }

            float desiredSpeed = m_CarController.MaxSpeed;

            switch (m_BrakeCondition)
            {
                case BrakeCondition.TargetDirectionDifference:
                    float approachingCornerAngle = Vector3.Angle(m_Target.forward, fwd);
                    float spinningAngle = m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor;
                    float cautiousnessRequired = Mathf.InverseLerp(0, m_CautiousMaxAngle, Mathf.Max(spinningAngle, approachingCornerAngle));
                    desiredSpeed = Mathf.Lerp(m_CarController.MaxSpeed, m_CarController.MaxSpeed * m_CautiousSpeedFactor, cautiousnessRequired);
                    break;

                case BrakeCondition.TargetDistance:
                    Vector3 delta = m_Target.position - transform.position;
                    float distanceCautiousFactor = Mathf.InverseLerp(m_CautiousMaxDistance, 0, delta.magnitude);
                    float cautiousnessRequired2 = Mathf.Max(Mathf.InverseLerp(0, m_CautiousMaxAngle, m_Rigidbody.angularVelocity.magnitude * m_CautiousAngularVelocityFactor), distanceCautiousFactor);
                    desiredSpeed = Mathf.Lerp(m_CarController.MaxSpeed, m_CarController.MaxSpeed * m_CautiousSpeedFactor, cautiousnessRequired2);
                    break;
            }

            Vector3 offsetTargetPos = m_Target.position;
            if (Time.time < m_AvoidOtherCarTime)
            {
                desiredSpeed *= m_AvoidOtherCarSlowdown;
                offsetTargetPos += m_Target.right * m_AvoidPathOffset;
            }
            else
            {
                offsetTargetPos += m_Target.right * (Mathf.PerlinNoise(Time.time * m_LateralWanderSpeed, m_RandomPerlin) * 2 - 1) * m_LateralWanderDistance;
            }

            float accelBrakeSensitivity = (desiredSpeed < m_CarController.CurrentSpeed) ? m_BrakeSensitivity : m_AccelSensitivity;
            float accel = Mathf.Clamp((desiredSpeed - m_CarController.CurrentSpeed) * accelBrakeSensitivity, -1, 1);
            accel *= (1 - m_AccelWanderAmount) + (Mathf.PerlinNoise(Time.time * m_AccelWanderSpeed, m_RandomPerlin) * m_AccelWanderAmount);

            Vector3 localTarget = transform.InverseTransformPoint(offsetTargetPos);
            float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
            float steer = Mathf.Clamp(targetAngle * m_SteerSensitivity, -1, 1) * Mathf.Sign(m_CarController.CurrentSpeed);

            m_CarController.Move(steer, accel, accel, 0f);

            if (m_StopWhenTargetReached && localTarget.magnitude < m_ReachTargetThreshold)
            {
                m_Driving = false;
            }
        }

        private IEnumerator RespawnCar()
        {
            isRespawning = true;
            yield return new WaitForSeconds(stuckCheckTime);

            if ((IsOffTerrain() || Vector3.Distance(transform.position, lastPosition) < stuckThreshold) && m_Rigidbody.velocity.magnitude < minSpeedForStuckCheck)
            {
                if (roadCenters.Length > 0)
                {
                    Transform closestCenter = GetClosestRoadCenter();
                    if (closestCenter != null)
                    {
                        transform.position = closestCenter.position;
                        transform.rotation = closestCenter.rotation;
                        Debug.Log("Auto wurde zum nächsten RoadCenter zurückgesetzt.");
                    }
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
                float distance = Vector3.Distance(transform.position, center.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCenter = center;
                }
            }
            return closestCenter;
        }

        public void SetTarget(Transform target)
        {
            m_Target = target;
            m_Driving = true;
        }
    }
}
