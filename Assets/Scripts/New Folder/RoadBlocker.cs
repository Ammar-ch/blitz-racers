using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBumperPlacer : MonoBehaviour
{
    public GameObject bumperPrefab; // Prefab الخاص بالصادم
    public Transform roadPath; // الطريق الذي سيتم توزيع الصادمات على طوله
    public int bumperCount = 20; // عدد الصادمات المراد توزيعها
    public float spacing = 10.0f; // المسافة بين كل صادم وآخر

    void Start()
    {
        PlaceBumpersAlongRoad();
    }

    void PlaceBumpersAlongRoad()
    {
        // افتراض أنك تعرف طول الطريق (أو استخدم طول الطريق الفعلي إذا كان موجودًا)
        float pathLength = roadPath.GetComponent<Renderer>().bounds.size.z;

        for (int i = 0; i < bumperCount; i++)
        {
            // حساب الموضع على طول المسار باستخدام موقع `roadPath`
            Vector3 bumperPosition = roadPath.position + (roadPath.forward * (i * spacing));
            bumperPosition.y = roadPath.position.y; // التأكد من أن الصادمات في نفس مستوى الطريق

            // إنشاء الصادم ووضعه في الموضع المحسوب
            GameObject bumperInstance = Instantiate(bumperPrefab, bumperPosition, roadPath.rotation);

            // تعيين الصادم كابن للطريق
            bumperInstance.transform.parent = roadPath;
        }
    }
}
