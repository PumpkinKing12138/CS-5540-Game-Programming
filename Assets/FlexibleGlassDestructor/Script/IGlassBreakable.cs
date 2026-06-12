using UnityEngine;

namespace FlexibleGlassDestructor
{
    public interface IGlassBreakable
    {
        void TakeDamage(Vector3 hitPoint, Vector3 direction, float force);

        void Fracture();
    }
}
