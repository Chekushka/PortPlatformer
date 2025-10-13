using System;

namespace Input
{
    public static class InputSystemEvents
    {
        public static event Action OnBindingsChanged;
    
        public static void InvokeBindingsChanged()
        {
            OnBindingsChanged?.Invoke();
        }
    }
}