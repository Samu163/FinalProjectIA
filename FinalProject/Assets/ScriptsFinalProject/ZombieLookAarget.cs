using UnityEngine;

public class ZombieLookAtTargetSmooth : MonoBehaviour
{
    public Transform target; // Arrastra aquí el objeto que será el objetivo
    public float rotationSpeed = 5f; // Velocidad de rotación (ajustable desde el inspector)
   

    void Update()
    {
        if (target != null)
        {
            // Calcular la dirección hacia el objetivo
            Vector3 direction = target.position - transform.position;

            // Ignorar la diferencia en el eje Y (evitar que incline hacia arriba o abajo)
            direction.y = 0;

            // Verificar que la dirección no sea cero
            if (direction.sqrMagnitude > 0.01f)
            {
                // Calcular la rotación deseada hacia el objetivo
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Interpolar suavemente hacia la rotación deseada
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSpeed
                );
            }
        }
    }
}

