using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    /// <summary>
    /// Abstract class for Reinitializor.
    /// </summary>
    abstract public class Reinitializor
    {
        /// <summary>
        /// Every Reinitializor should implement this method.
        /// </summary>
        abstract public void
        Reinitialize();
    }
}
