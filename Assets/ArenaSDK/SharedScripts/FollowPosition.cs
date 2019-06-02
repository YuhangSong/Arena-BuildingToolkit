using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arena
{
    /// <summary>
    /// Attach this script to an GameObject so that this GameObject will always follow,
    /// the position of another GameObject specified by ToFollow
    /// </summary>
    public class FollowPosition : ArenaBase
    {
        /// <summary>
        /// The GameObject to follow.
        /// </summary>
        public GameObject ToFollow;

        /// <summary>
        /// The relative position of this GameObject to the GameObject to follow.
        /// </summary>
        private Vector3 RelativePosition;

        /// <summary>
        /// Called before everything starts.
        /// Record the RelativePosition.
        /// </summary>
        void
        Awake()
        {
            RelativePosition = transform.position - ToFollow.transform.position;
        }

        /// <summary>
        /// Fixed update for the GameObject to follow.
        /// </summary>
        void
        FixedUpdate()
        {
            transform.position = ToFollow.transform.position + RelativePosition;
        }
    }
}
