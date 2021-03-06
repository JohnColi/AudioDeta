using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T s_Instance;
	private static bool s_IsDestroyed;

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
}