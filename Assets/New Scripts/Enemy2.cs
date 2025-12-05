using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public float speed = 10f;

    public int health = 100;

    public int moneyGained = 50;

    public GameObject deathEffect;

    private Transform target;
	private int wavepointIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = Waypoints.points[0];
        
    }


    public void TakeDamage (int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die ()
    {
        PlayerManager.Money += moneyGained;
        GameObject effect = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 5f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dir = target.position - transform.position;
		transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, target.position) <= 0.2f)
		{
			GetNextWaypoint();
		}



	
        
    }
    void GetNextWaypoint()
	{
		if (wavepointIndex >= Waypoints.points.Length - 1)
		{
			EndPath();
            return;

		}

		wavepointIndex++;
		target = Waypoints.points[wavepointIndex];
    }

    void EndPath ()
    {
        PlayerManager.Lives--;
        Destroy(gameObject);
    }
}