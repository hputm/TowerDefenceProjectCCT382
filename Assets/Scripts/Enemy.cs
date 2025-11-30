using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 10f;
    private Transform target;
	private int wavepointIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = Waypoints.points[0];
        
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
			Destroy(gameObject);
            return;

		}

		wavepointIndex++;
		target = Waypoints.points[wavepointIndex];
    }
}
