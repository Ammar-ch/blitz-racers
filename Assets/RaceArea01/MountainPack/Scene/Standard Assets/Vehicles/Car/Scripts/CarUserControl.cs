using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof(CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // المتحكم في السيارة
        private bool isRespawning = false; // علم ليتأكد من عملية respawn
        private Vector3 lastPosition; // لتتبع آخر موقع للسيارة
        private float stuckCheckTime = 2.0f; // مدة التحقق إذا كانت السيارة عالقة
        private float stuckThreshold = 0.5f; // الحد الأدنى للحركة لاعتبار السيارة "تتحرك"
        private float fallThreshold = -10.0f; // مستوى الارتفاع الذي يعتبر سقوطاً عن الطريق

        // مصفوفة لحفظ نقاط RoadCenter
        public Transform[] roadCenters;
        private Transform lastRoadCenter; // لتخزين آخر نقطة RoadCenter تم تجاوزها

        private void Awake()
        {
            // الحصول على المتحكم في السيارة
            m_Car = GetComponent<CarController>();
            lastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            // تمرير الإدخال للسيارة
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif

            // تحديث نقطة RoadCenter الأخيرة إذا اقتربت منها السيارة
            UpdateLastRoadCenter();

            // التحقق إذا كانت السيارة عالقة أو خرجت عن الطريق
            if ((Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0) && !isRespawning)
            {
                if (Vector3.Distance(transform.position, lastPosition) < stuckThreshold)
                {
                    // إذا لم تتحرك السيارة، البدء في إعادة وضعها
                    StartCoroutine(RespawnCar());
                }
                else
                {
                    // تحديث آخر موقع للسيارة إذا كانت تتحرك
                    lastPosition = transform.position;
                }
            }

            // التحقق إذا كانت السيارة خرجت عن الطريق (سقطت تحت مستوى معين)
            if (transform.position.y < fallThreshold && !isRespawning)
            {
                StartCoroutine(RespawnCar());
            }
        }

        private void UpdateLastRoadCenter()
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

            // إذا كانت السيارة قريبة من مركز الطريق
            if (closestCenter != null && minDistance < 5f) // يمكنك تعديل هذه القيمة للتحكم في مدى القرب
            {
                lastRoadCenter = closestCenter;
            }
        }

        private IEnumerator RespawnCar()
        {
            isRespawning = true;
            yield return new WaitForSeconds(stuckCheckTime);

            // إذا كانت السيارة عالقة أو خرجت عن الطريق
            if (Vector3.Distance(transform.position, lastPosition) < stuckThreshold || transform.position.y < fallThreshold)
            {
                if (lastRoadCenter != null)
                {
                    // إعادة السيارة إلى آخر نقطة RoadCenter تم تجاوزها
                    transform.SetPositionAndRotation(lastRoadCenter.position, Quaternion.Euler(0, lastRoadCenter.eulerAngles.y, 0));
                }
            }

            isRespawning = false;
            lastPosition = transform.position;
        }
    }
}
