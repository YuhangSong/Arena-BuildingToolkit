using UnityEngine;

namespace Arena {
    public class RewardFunctionGeneratorDistanceToTarget {
        private GameObject BaseObject;
        private GameObject TargetObject;

        private float DistanceToTargetLastTime;

        /// <summary>
        /// Constructor.
        /// Generate reward base on the distance from BaseObject to TargetObject
        /// 1, Reward moving towards Target
        /// 2, Penalize moving away from Target.
        /// CumulativeReward: 7
        /// </summary>
        /// <param name="BaseObject">Reference to the object, with the respect of which you want measure the distance to the target.</param>
        /// <param name="TargetObject">The target object.</param>
        public RewardFunctionGeneratorDistanceToTarget(
            GameObject BaseObject_,
            GameObject TargetObject_)
        {
            BaseObject   = BaseObject_;
            TargetObject = TargetObject_;
        }

        public void
        Reset()
        {
            DistanceToTargetLastTime = Vector3.Distance(
                BaseObject.transform.position, TargetObject.transform.position);
        }

        public float
        StepGetReward()
        {
            float DistanceToTargetThisTime = Vector3.Distance(
                BaseObject.transform.position, TargetObject.transform.position);
            float DistanceMoveClosing = DistanceToTargetLastTime - DistanceToTargetThisTime;

            DistanceToTargetLastTime = DistanceToTargetThisTime;

            return DistanceMoveClosing;
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
