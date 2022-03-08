using UnityEngine;
 
 
 
public class FollowTargetAI : MonoBehaviour
{
    public float MinDistance = 3;
    public float MaxDistance = 1;
    public float Speed = 3;
    public Transform Player;
 
 
    void Update()
    {
        transform.LookAt(Player);
        if (Vector3.Distance(transform.position, Player.position) >= MinDistance)
        {
            Vector3 follow = Player.position;
           
            follow.y = this.transform.position.y;
            
            this.transform.position = Vector3.MoveTowards(this.transform.position, follow, Speed * Time.deltaTime);
        }
    }
}