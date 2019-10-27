using UnityEngine;
using System.Collections.Generic;
using MLAgents;

namespace Arena {
    [System.Serializable]
    public class RewardFunction {
        public float Coefficient = 1f;

        public RewardFunction(
            float Coefficient_
        )
        {
            Coefficient = Coefficient_;
        }
    }

    [System.Serializable]
    public class RewardFunctionDistance : RewardFunction {
        public List<GameObject> GameObjects = new List<GameObject>();

        private float DistanceLastStep;

        /// <summary>
        /// Constructor.
        /// Generate reward base on the distance from ObjectA to ObjectB
        /// </summary>
        public RewardFunctionDistance(
            GameObject ObjectA,
            GameObject ObjectB,
            float      Coefficient_
        ) : base(Coefficient_)
        {
            GameObjects.Add(ObjectA);
            GameObjects.Add(ObjectB);
        }

        public void
        Reset()
        {
            // Debug.Log(GameObjects[1].transform.position);
            DistanceLastStep = Vector3.Distance(
                GameObjects[0].transform.position, GameObjects[1].transform.position
            );
        }

        public float
        StepGetReward()
        {
            float DistanceThisStep = Vector3.Distance(
                GameObjects[0].transform.position, GameObjects[1].transform.position);
            float DeltaDistance = DistanceThisStep - DistanceLastStep;

            DistanceLastStep = DistanceThisStep;

            return DeltaDistance * Coefficient;
        }
    }

    public class RewardFunctionGeneratorTimePenalty {
        /// <summary>
        /// Constructor.
        /// 1,  penalty for time-contrained tasks.
        /// CumulativeReward: xx
        /// </summary>
        public RewardFunctionGeneratorTimePenalty()
        { }

        public void
        Reset()
        { }

        public float
        StepGetReward()
        {
            return -1f;
        }
    }

    public class RewardFunctionGeneratorKeepTowards {
        private GameObject gameObject;
        private Vector3 Direction;

        private GameObject gameObjectBase;
        private Vector3 PositionBase;

        /// <summary>
        /// Constructor.
        /// 1, encourage to keep the position of the gameObject towards a direction.
        /// If gameObjectBase is not set, the compute will be based on the gameObject's position at reset.
        /// If gameObjectBase is set, the compute will be based on the gameObjectBase's position at step.
        /// </summary>
        public RewardFunctionGeneratorKeepTowards(GameObject gameObject_, Vector3 Direction_)
        {
            gameObject = gameObject_;
            Direction  = Direction_;
        }

        public RewardFunctionGeneratorKeepTowards(GameObject gameObject_, Vector3 Direction_,
          GameObject gameObjectBase_) : this(gameObject_, Direction_)
        {
            gameObjectBase = gameObjectBase_;
        }

        public void
        Reset()
        {
            if (gameObjectBase == null) {
                PositionBase = gameObject.transform.position;
            }
        }

        public float
        StepGetReward()
        {
            Vector3 delta;

            if (gameObjectBase == null) {
                delta = gameObject.transform.position - PositionBase;
            } else {
                delta = gameObject.transform.position - gameObjectBase.transform.position;
            }
            return Vector3.Dot(
                delta,
                Direction.normalized
            );
        }
    }

    public class RewardFunctionGeneratorVelocityToTarget {
        private GameObject BaseObject;
        private GameObject TargetObject;

        /// <summary>
        /// Constructor.
        /// Generate reward base on the velocity along the direction from BaseObject to TargetObject
        /// 1, Reward if velocity is towards Target
        /// 2, Penalize if velocity is away from Target
        /// </summary>
        /// <param name="BaseObject">Reference to the object, with the respect of which you want measure the direction to the target.</param>
        /// <param name="TargetObject">The target object.</param>
        public RewardFunctionGeneratorVelocityToTarget(
            GameObject BaseObject_,
            GameObject TargetObject_)
        {
            BaseObject   = BaseObject_;
            TargetObject = TargetObject_;

            if (BaseObject.GetComponent<Rigidbody>() == null) {
                Debug.LogError(
                    "In other to use RewardFunctionGeneratorVelocityToTarget, BaseObject has to have a Rigidbody attached to it.");
            }
        }

        public void
        Reset()
        { }

        public float
        StepGetReward()
        {
            Vector3 TargetDirection = TargetObject.transform.position - BaseObject.transform.position;

            return Vector3.Dot(TargetDirection.normalized, BaseObject.GetComponent<Rigidbody>().velocity);
        }
    }

    public class RewardFunctionGeneratorFacingTarget : RewardFunctionGeneratorFacing {
        private GameObject BaseObject;
        private GameObject TargetObject;

        /// <summary>
        /// Constructor.
        /// Generate reward base on the direction from BaseObject to TargetObject
        /// 1, Reward facing Target
        /// 2, Penalize facing away from Target
        /// </summary>
        /// <param name="BaseObject">Reference to the object, with the respect of which you want measure the direction to the target.</param>
        /// <param name="TargetObject">The target object.</param>
        /// <param name="Type">
        ///   "dot"
        /// </param>
        public RewardFunctionGeneratorFacingTarget(
            GameObject BaseObject_,
            GameObject TargetObject_,
            Types      Type_) : base(Vector3.zero, Type_, 0f, 0f)
        {
            BaseObject   = BaseObject_;
            TargetObject = TargetObject_;
        }

        public override float
        GetDirectionDot(Vector3 Facingto)
        {
            Vector3 TargetDirection = TargetObject.transform.position - BaseObject.transform.position;

            return Vector3.Dot(TargetDirection.normalized, Facingto);
        }
    }

    public class RewardFunctionGeneratorFacing {
        /// <summary>
        /// Dot
        /// Binary_NP: -1 or +1
        /// Binary_NZ: -1 or 0
        /// Binary_ZP: 0 or 1
        /// </summary>
        public enum Types {
            Dot,
            Binary_NP,
            Binary_NZ,
            Binary_ZP
        }

        private Vector3 TargetDirection;

        protected Types Type;
        protected float Epsilon;
        protected float Offset;

        /// <summary>
        /// Constructor.
        /// Generate reward base on the direction from BaseObject to TargetObject
        /// 1, Reward facing Target
        /// 2, Penalize facing away from Target
        /// </summary>
        /// <param name="Type">
        /// Dot
        /// Binary_NP: -1 or +1
        /// Binary_NZ: -1 or 0
        /// Binary_ZP: 0 or 1
        /// </param>
        public RewardFunctionGeneratorFacing(
            Vector3 TargetDirection_,
            Types   Type_,
            float   Epsilon_,
            float   Offset_)
        {
            TargetDirection = TargetDirection_;
            Type    = Type_;
            Epsilon = Epsilon_;
            Offset  = Offset_;
        }

        public virtual void
        Reset(){ }

        public virtual float
        GetDirectionDot(Vector3 Facingto)
        {
            return Vector3.Dot(TargetDirection.normalized, Facingto);
        }

        public float
        StepGetReward(Vector3 Facingto)
        {
            float DirectionDot = GetDirectionDot(Facingto);

            if (Type == Types.Dot) {
                return DirectionDot;
            } else if (Type == Types.Binary_NP) {
                if (DirectionDot < -Epsilon) {
                    return -1f;
                } else if (DirectionDot > Epsilon) {
                    return 1f;
                } else {
                    return 0f;
                }
            } else if (Type == Types.Binary_NZ) {
                if (DirectionDot < -Epsilon) {
                    return -1f;
                } else if (DirectionDot > Epsilon) {
                    return 0f;
                } else {
                    return 0f;
                }
            } else if (Type == Types.Binary_ZP) {
                if (DirectionDot < -Epsilon) {
                    return 0f;
                } else if (DirectionDot > Epsilon) {
                    return 1f;
                } else {
                    return 0f;
                }
            } else {
                Debug.LogError("Invalid Type");
                return 0f;
            }
        } // StepGetReward
    }
}
