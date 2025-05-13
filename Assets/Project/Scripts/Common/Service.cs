using System;
using System.Runtime.CompilerServices;

namespace IdleTycoon
{
    // Service - Service locator wrapper.
    public static class Service<T> where T : class
    {
        static T _instance;

        // Gets global instance of T type.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get(bool createIfNotExists = false)
        {
            if (_instance != null)
            {
                return _instance;
            }
            if (createIfNotExists)
            {
                _instance = (T)Activator.CreateInstance(typeof(T), true);
            }
            return _instance;
        }

        // Sets global instance of T type.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Set(T instance)
        {
            _instance = instance;
            return _instance;
        }
    }
}