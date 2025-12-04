using UnityEngine;

public class Tower : MonoBehaviour
{
    public Transform target;

   
    public float range = 15f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;


    public string enemyTag = "Enemy";

    public GameObject bulletPrefab;
    public Transform firePoint;

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating ("UpdateTarget", 0f, 0.5f);
        
    }

    // Update is called once per frame
    void Update ()
    {
        if (target == null)
        {
            return;
        }
        if (fireCountdown <= 0f) 
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    void Shoot ()
    {
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(target);
        }
    }
    void OnDrawGizmosSelected ()
	{
		Gizmos.DrawWireSphere(transform.position, range);
	}
    void UpdateTarget ()
	{
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }
        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        } else
		{
			target = null;
		}
    }
}
