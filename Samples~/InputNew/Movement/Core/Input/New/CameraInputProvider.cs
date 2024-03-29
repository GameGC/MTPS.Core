#if ENABLE_INPUT_SYSTEM

using Cinemachine;
using UnityEngine;

namespace MTPS.Movement.Core.Input.New
{
    /// <summary>
    /// This is an add-on to override the legacy input system and read input using the
    /// UnityEngine.Input package API.  Add this behaviour to any CinemachineVirtualCamera 
    /// or FreeLook that requires user input, and drag in the the desired actions.
    /// If the Input System Package is not installed, then this behaviour does nothing.
    /// </summary>
    public class CameraInputProvider : MonoBehaviour, AxisState.IInputAxisProvider
    {
        public IMoveInput InputReader;

        /// <summary>
        /// Implementation of AxisState.IInputAxisProvider.GetAxisValue().
        /// Axis index ranges from 0...2 for X, Y, and Z.
        /// Reads the action associated with the axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns>The current axis value</returns>
        public virtual float GetAxisValue(int axis)
        {
            if (InputReader == null) return 0;
            if (axis > 2) return 0;
            return InputReader.lookInput[axis];
        }
    }
}
#endif
