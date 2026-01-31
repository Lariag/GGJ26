using UnityEngine;

public static class Util
{
	public static T MakeSingleton<T>(T instance) where T : MonoBehaviour
	{
		var instances = GameObject.FindObjectsByType<T>(FindObjectsSortMode.None);
		if (instances.Length > 1)
		{
			Object.Destroy(instance.gameObject);
			return instances[0] == instance ? instances[1] : instance;
		}
		else
		{
			Object.DontDestroyOnLoad(instance.gameObject);
			return instance;
		}
	}
}
