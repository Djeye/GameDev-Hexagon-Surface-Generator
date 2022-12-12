using UnityEngine;

public class RocketTarget : MonoBehaviour, IExplode
{
    [SerializeField] private Rigidbody rb;
    public Rigidbody Rb => rb;
    
    public void Explode() => Destroy(gameObject);
}