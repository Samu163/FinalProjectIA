using UnityEngine;

public class ZombieLookAtTargetSmooth : MonoBehaviour
{
    public Transform target; // Arrastra aqu� el objeto que ser� el objetivo
    public float rotationSpeed = 5f; // Velocidad de rotaci�n (ajustable desde el inspector)
   

    void Update()
    {
        if (target != null)
        {
            // Calcular la direcci�n hacia el objetivo
            Vector3 direction = target.position - transform.position;

            // Ignorar la diferencia en el eje Y (evitar que incline hacia arriba o abajo)
            direction.y = 0;

            // Verificar que la direcci�n no sea cero
            if (direction.sqrMagnitude > 0.01f)
            {
                // Calcular la rotaci�n deseada hacia el objetivo
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Interpolar suavemente hacia la rotaci�n deseada
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSpeed
                );
            }
        }
    }
}

