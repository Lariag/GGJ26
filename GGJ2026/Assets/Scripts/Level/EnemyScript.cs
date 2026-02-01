using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float RemainingTimeToLive;


    void Update()
    {
        if(Managers.Ins.GameScript.CurrentGameState != Enums.GameState.Playing)
            return;
        RemainingTimeToLive -= Time.deltaTime;
        if(RemainingTimeToLive <= 0f)
        {
            Destroy(gameObject);
		}
	}
}
