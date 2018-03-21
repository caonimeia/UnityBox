using System;
using System.Reflection;

public class Singleton<T> where T : class {
    private static T _instance;

    //public static T GetInstance() {
    //    if(_instance == null) {
    //        _instance = Activator.CreateInstance(typeof(T), true) as T;
    //    }

    //    return _instance;
    //}

    public static T instance {
        get {
            if(_instance == null) {
                _instance = Activator.CreateInstance(typeof(T), true) as T;
            }

            return _instance;
        }

        private set {}
    }
}

