using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    public float m_Speed = 5f;                 // Velocidad de movimiento del bot
    public float m_RotationSpeed = 2f;         // Velocidad de rotación del bot
    public float m_AttackRange = 10f;          // Rango de ataque del bot
    public float m_AttackCooldown = 2f;        // Tiempo de enfriamiento entre ataques
    public Transform m_FireTransform;          // Posición de lanzamiento de proyectiles
    public Rigidbody m_ProjectilePrefab;       // Prefab del proyectil a lanzar
    public float m_ProjectileSpeed = 10f;      // Velocidad del proyectil lanzado

    private Transform m_Player;                // Referencia al transform del jugador
    private bool m_CanAttack = true;           // Indicador de si el bot puede atacar

    private void Start()
    {
        // Encontrar el transform del jugador
        m_Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Rotar hacia el jugador
        Vector3 directionToPlayer = m_Player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_RotationSpeed * Time.deltaTime);

        // Mover hacia adelante
        transform.Translate(Vector3.forward * m_Speed * Time.deltaTime);

        // Si el jugador está dentro del rango de ataque y el bot puede atacar, disparar
        if (Vector3.Distance(transform.position, m_Player.position) <= m_AttackRange && m_CanAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        // Crear un proyectil y lanzarlo en la dirección del jugador
        Rigidbody projectileInstance = Instantiate(m_ProjectilePrefab, m_FireTransform.position, m_FireTransform.rotation);
        projectileInstance.velocity = m_ProjectileSpeed * m_FireTransform.forward;

        // Iniciar el cooldown de ataque
        m_CanAttack = false;
        Invoke("ResetAttackCooldown", m_AttackCooldown);
    }

    private void ResetAttackCooldown()
    {
        // Restablecer la capacidad de ataque del bot después del tiempo de enfriamiento
        m_CanAttack = true;
    }
    // Start is called before the first frame update
   
}
