using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{

	private static T s_Instance;
	private static bool s_IsDestroyed;

	protected virtual void Awake()
	{

	}

	public static bool IsNull()
	{
		return s_Instance == null;
	}

	public static T Instance
	{
		get
		{

			if (s_Instance == null)
			{
				s_Instance = FindObjectOfType(typeof(T)) as T;

				if (s_Instance == null)
				{
					var gameObject = new GameObject(typeof(T).Name);
					DontDestroyOnLoad(gameObject);

					s_Instance = gameObject.AddComponent(typeof(T)) as T;
				}
			}

			return s_Instance;
		}
	}

	protected virtual void OnDestroy()
	{
		// if (s_Instance)
		// 	Destroy(s_Instance);

		// s_Instance = null;
		// s_IsDestroyed = true;

		//Debug.LogError("OnDestroy  ,name:" + typeof(T).Name);
	}
}