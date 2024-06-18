using UnityEngine;

// Этот класс позволяет другим объектам ссылаться на единственный 
// общий объект. Его используют классы GameManager и InputManager.
// Чтобы воспользоваться этим классом, унаследуйте его:
// public class MyManager : Singleton<MyManager> { }
// После этого вы сможете обращаться к единственному общему 
// экземпляру класса так:
// MyManager.instance.DoSomething();
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Единственный экземпляр этого класса.
    private static T _instance;
    // Метод доступа. При первом вызове настраивает _instance.
    // Если требуемый объект не найден,
    // выводит сообщение об ошибке в журнал.
    public static T Instance
    {
        get
        {
            // Если свойство _instance еще не настроено..
            if (_instance == null)
            {
                // ...попытаться найти объект.
                _instance = FindObjectOfType<T>();
                // Записать сообщение об ошибке в случае неудачи.
                if (_instance == null)
                {
                    Debug.LogError("Can't find " + typeof(T) + "!");
                }
            }
            // Вернуть экземпляр для использования!
            return _instance;
        }
    }
}
