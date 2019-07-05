using UnityEngine;
using MLAgents;

namespace Arena {
    /// <summary>
    /// A based implementation of an agent with actions of moving, turning and jumping.
    /// </summary>
    public class BasicAgent : ArenaAgent
    {
        [Header("Reward Functions (Isolated)")][Space(10)]

        public bool IsRewardDistanceToTarget = false; // Agent should move towards Target
        public bool IsRewardFacingTarget     = false; // Agent should face the Target
        public GameObject Target;

        public bool IsRewardTimePenalty = false; // Hurry up

        [Header("Reward Function Properties")][Space(10)]

        public float RewardDistanceCoefficient  = 1.0f;
        public float RewardDirectionCoefficient = 0.01f;
        public float RewardTimeCoefficient      = 0.001f;

        // reward functions
        private RewardFunctionGeneratorDistanceToTarget RewardFunctionDistanceToTarget;
        private RewardFunctionGeneratorFacingTarget RewardFunctionFacingTarget;
        private RewardFunctionGeneratorTimePenalty RewardFunctionTimePenalty;

        [Header("Player Settings")][Space(10)]

        /// <summary>
        /// Reference to the Player.
        /// </summary>
        public GameObject Player;

        /// <summary>
        /// If apply TeamMaterial to the player.
        /// </summary>
        public bool isApplyTeamMaterialToPlayer = true;

        [Header("Action Settings (Move)")][Space(10)]

        /// <summary>
        /// If enable moving actions (Left, Right, Forward, Backward).
        /// </summary>
        public bool AllowMove = true;

        /// <summary>
        /// MoveTypes.
        /// </summary>
        public enum MoveTypes {
            Force,
            Velocity
        }

        /// <summary>
        /// MoveTypes.
        ///   Force: move by force.
        ///   Velocity: move by velocity.
        /// </summary>
        public MoveTypes MoveType = MoveTypes.Force;

        /// <summary>
        /// Accumulate speed of MoveForce.
        /// </summary>
        public float MoveForceAcc = 0.0f;

        /// <summary>
        /// Max speed of MoveForce.
        /// </summary>
        public float MoveForceMax = 40.0f;

        /// <summary>
        /// Base of MoveVelocity.
        /// </summary>
        public float MoveVelocityBase = 5.0f;

        /// <summary>
        /// Accumulate speed of MoveVelocity.
        /// </summary>
        public float MoveVelocityAcc = 0.0f;

        /// <summary>
        /// Max speed of MoveVelocity.
        /// </summary>
        public float MoveVelocityMax = 10.0f;

        [Header("Action Settings (NoAction)")][Space(10)]

        /// <summary>
        /// If freeze the position of the player when taking NoAction.
        /// </summary>
        public bool FreezePositionWhenNoAction = false;

        /// <summary>
        /// If freeze the rotation of the player when taking NoAction.
        /// </summary>
        public bool FreezeRotationWhenNoAction = false;

        [Header("Action Settings (Jump)")][Space(10)]

        /// <summary>
        /// If enable jump actions (Jump).
        /// </summary>
        public bool AllowJump = true;

        /// <summary>
        /// Force of jump.
        /// </summary>
        public float JumpForceMax = 400f;

        /// <summary>
        /// Base of MoveForce.
        /// </summary>
        public float MoveForceBase = 20.0f;

        [Header("Action Settings (Turn)")][Space(10)]

        /// <summary>
        /// If enable turn actions (TurnLeft, TurnRight).
        /// </summary>
        public bool AllowTurn = true;

        /// <summary>
        /// Base of TurnVelocity.
        /// </summary>
        public float TurnVelocityBase = 0.3f;

        /// <summary>
        /// Accumulate speed of TurnVelocity.
        /// </summary>
        public float TurnVelocityAcc = 0.2f;

        /// <summary>
        /// Accumulate speed of TurnVelocity.
        /// </summary>
        public float TurnVelocityMax = 3f;

        [Header("Player Settings (Initialize)")][Space(10)]

        /// <summary>
        /// Random position range (Min).
        /// </summary>
        public Vector3 PlayerRandomPositionMin = new Vector3(0f, 0f, 0f);

        /// <summary>
        /// Random position range (Max).
        /// </summary>
        public Vector3 PlayerRandomPositionMax = new Vector3(0f, 0f, 0f);

        /// <summary>
        /// Random eular angle range (Min).
        /// </summary>
        public Vector3 PlayerRandomEulerAnglesMin = new Vector3(0f, 0f, 0f);

        /// <summary>
        /// Random eular angle range (Max).
        /// </summary>
        public Vector3 PlayerRandomEulerAnglesMax = new Vector3(0f, 0f, 0f);

        /// <summary>
        /// Random force range (Min).
        /// </summary>
        public Vector3 PlayerRandomForceMin = new Vector3(0f, 0f, 0f);

        /// <summary>
        /// Random force range (Max).
        /// </summary>
        public Vector3 PlayerRandomForceMax = new Vector3(0f, 0f, 0f);

        /// <summary>
        /// TransformReinitializor for the player.
        /// </summary>
        private TransformReinitializor PlayerReinitializor;

        /// <summary>
        /// Accumulator for the Move.
        /// </summary>
        protected Accumulator MoveAccumulator;

        /// <summary>
        /// Accumulator for the TurnAccumulator.
        /// </summary>
        protected Accumulator TurnAccumulator;

        /// <summary>
        /// Start and initialize.
        /// </summary>
        public override void
        InitializeAgent()
        {
            base.InitializeAgent();
            CheckDesign();

            if (isApplyTeamMaterialToPlayer) {
                base.ApplyTeamMaterial(Player);
            }

            PlayerReinitializor = new TransformReinitializor(
                Player,
                PlayerRandomPositionMin, PlayerRandomPositionMax,
                PlayerRandomEulerAnglesMin, PlayerRandomEulerAnglesMax,
                PlayerRandomForceMin, PlayerRandomForceMax);
            PlayerReinitializor.Reinitialize();

            if (GetActionSpaceType() == SpaceType.discrete) {
                if (MoveType == MoveTypes.Force) {
                    MoveAccumulator = new Accumulator(MoveForceBase, MoveForceAcc, MoveForceMax);
                } else if (MoveType == MoveTypes.Velocity) {
                    MoveAccumulator = new Accumulator(MoveVelocityBase, MoveVelocityAcc, MoveVelocityMax);
                }
                TurnAccumulator = new Accumulator(TurnVelocityBase, TurnVelocityAcc, TurnVelocityMax);
            }
        } // InitializeAgent

        protected override void
        InitializeRewardFunction()
        {
            base.InitializeRewardFunction();
            // create reward functions
            if (IsRewardDistanceToTarget) {
                RewardFunctionDistanceToTarget = new RewardFunctionGeneratorDistanceToTarget(
                    Player,
                    Target
                );
            }
            if (IsRewardFacingTarget) {
                RewardFunctionFacingTarget = new RewardFunctionGeneratorFacingTarget(
                    Player,
                    Target,
                    RewardFunctionGeneratorFacingTarget.Types.Dot
                );
            }
            if (IsRewardTimePenalty) {
                RewardFunctionTimePenalty = new RewardFunctionGeneratorTimePenalty(
                );
            }
        }

        /// <summary>
        /// Reset for agent.
        /// </summary>
        public override void
        AgentReset()
        {
            base.AgentReset();
            PlayerReinitializor.Reinitialize();
            if (GetActionSpaceType() == SpaceType.discrete) {
                MoveAccumulator.Reset();
                TurnAccumulator.Reset();
            }
            ResetRewardFunction();
        }

        protected override void
        ResetRewardFunction()
        {
            // reset reward functions
            if (IsRewardDistanceToTarget) {
                RewardFunctionDistanceToTarget.Reset();
            }
            if (IsRewardFacingTarget) {
                RewardFunctionFacingTarget.Reset();
            }
            if (IsRewardTimePenalty) {
                RewardFunctionTimePenalty.Reset();
            }
        }

        /// <summary>
        /// CheckHistoryAction.
        /// </summary>
        override protected int
        CheckHistoryAction(int LastAction_, int Action_)
        {
            Action_ = base.CheckHistoryAction(LastAction_, Action_);

            if (((LastAction_ == Left) && (Action_ == Left)) || ((LastAction_ == Right) && (Action_ == Right)) ||
              ((LastAction_ == Forward) && (Action_ == Forward)) ||
              ((LastAction_ == Backward) && (Action_ == Backward)))
            {
                MoveAccumulator.Accumulate();
            } else {
                MoveAccumulator.Reset();
            }

            if (((LastAction_ == TurnLeft) && (Action_ == TurnLeft)) ||
              ((LastAction_ == TurnRight) && (Action_ == TurnRight)))
            {
                TurnAccumulator.Accumulate();
            } else {
                TurnAccumulator.Reset();
            }

            return Action_;
        }

        /// <summary>
        /// Check design problem.
        /// </summary>
        protected virtual void
        CheckDesign()
        {
            if (Player.GetComponentsInChildren<Rigidbody>().Length != 1) {
                Debug.LogWarning(
                    "Player should have and only one Rigidbody, but it has "
                    + Player.GetComponentsInChildren<Rigidbody>().Length);
            }
            if (AllowTurn) {
                if ((Player.GetComponentInChildren<Rigidbody>().constraints !=
                  (RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ)) &&
                  (Player.GetComponentInChildren<Rigidbody>().constraints !=
                  (RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ
                  | RigidbodyConstraints.FreezePositionY)))
                {
                    LogWarningFreezeRotationXZ();
                }
            } else {
                if ((Player.GetComponentInChildren<Rigidbody>().constraints !=
                  (RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ
                  | RigidbodyConstraints.FreezeRotationY)) && (Player.GetComponentInChildren<Rigidbody>().constraints !=
                  (RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ
                  | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionY)))
                {
                    LogWarningFreezeRotationXZ();
                }
            }
        }

        /// <summary>
        /// Log warning on FreezeRotationXZ.
        /// </summary>
        private void
        LogWarningFreezeRotationXZ()
        {
            Debug.LogWarning(
                "BasicAgent should freeze rotatation on X and Z, as it does not support rotation on these axises. See example Fallflat if you want the agent to rotated on these two axises.");
        }

        /// <summary>
        /// Step.
        /// </summary>
        override protected void
        DiscreteStep(int Action_)
        {
            base.DiscreteStep(Action_);

            if (AllowMove) {
                if (FreezePositionWhenNoAction) {
                    switch (Action_) {
                        case NoAction:
                            Player.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;
                            break;
                        default:
                            break;
                    }
                }
                if (MoveType == MoveTypes.Force) {
                    switch (Action_) {
                        case Left:
                            Player.GetComponentInChildren<Rigidbody>().AddForce(Player.transform.TransformVector(Vector3
                              .
                              left
                              * MoveAccumulator.getCurrent()));
                            break;
                        case Right:
                            Player.GetComponentInChildren<Rigidbody>().AddForce(Player.transform.TransformVector(Vector3
                              .
                              right
                              * MoveAccumulator.getCurrent()));
                            break;
                        case Forward:
                            Player.GetComponentInChildren<Rigidbody>().AddForce(Player.transform.TransformVector(Vector3
                              .
                              forward
                              * MoveAccumulator.getCurrent()));
                            break;
                        case Backward:
                            Player.GetComponentInChildren<Rigidbody>().AddForce(Player.transform.TransformVector(Vector3
                              .
                              back
                              * MoveAccumulator.getCurrent()));
                            break;
                        default:
                            break;
                    }
                } else if (MoveType == MoveTypes.Velocity) {
                    switch (Action_) {
                        case Left:
                            Player.GetComponentInChildren<Rigidbody>().velocity =
                              (Player.transform.TransformVector(Vector3.
                              left
                              * MoveAccumulator.getCurrent()));
                            break;
                        case Right:
                            Player.GetComponentInChildren<Rigidbody>().velocity =
                              (Player.transform.TransformVector(Vector3.
                              right
                              * MoveAccumulator.getCurrent()));
                            break;
                        case Forward:
                            Player.GetComponentInChildren<Rigidbody>().velocity =
                              (Player.transform.TransformVector(Vector3.
                              forward
                              * MoveAccumulator.getCurrent()));
                            break;
                        case Backward:
                            Player.GetComponentInChildren<Rigidbody>().velocity =
                              (Player.transform.TransformVector(Vector3.
                              back
                              * MoveAccumulator.getCurrent()));
                            break;
                        default:
                            break;
                    }
                }
            }

            if (AllowTurn) {
                if (FreezeRotationWhenNoAction) {
                    switch (Action_) {
                        case NoAction:
                            Player.GetComponentInChildren<Rigidbody>().angularVelocity = Vector3.zero;
                            break;
                        default:
                            break;
                    }
                }
                switch (Action_) {
                    case TurnLeft:
                        Player.GetComponentInChildren<Rigidbody>().angularVelocity = (
                            new Vector3(0.0f, -TurnAccumulator.getCurrent(), 0.0f)
                        );
                        break;
                    case TurnRight:
                        Player.GetComponentInChildren<Rigidbody>().angularVelocity = (
                            new Vector3(0.0f, TurnAccumulator.getCurrent(), 0.0f)
                        );
                        break;
                    default:
                        // Turn is stopped when not taking actions of TurnLeft and TurnRight.
                        // This is better for user experience.
                        Player.GetComponentInChildren<Rigidbody>().angularVelocity = Vector3.zero;
                        break;
                }
            }

            if (AllowJump) {
                switch (Action_) {
                    case Jump:
                        // Only works if the agent is on the ground.
                        // However, this judgement of if agent is on the groud could be buggy.
                        // But no better way avaible for now.
                        if (Player.GetComponentInChildren<Rigidbody>().velocity.y == 0f) {
                            Player.GetComponentInChildren<Rigidbody>().AddForce(
                                Player.transform.TransformVector(
                                    Vector3.up * JumpForceMax
                                )
                            );
                        }
                        break;
                    default:
                        break;
                }
            }
        } // DiscreteStep

        protected override void
        ContinuousStep(float[] Action_)
        {
            base.ContinuousStep(Action_);

            Action_ = Utils.ClampNumberList(Action_, -1f, 1f);

            if (AllowMove) {
                if (MoveType == MoveTypes.Force) {
                    Player.GetComponentInChildren<Rigidbody>().AddForce(
                        Player.transform.TransformVector(
                            (Vector3.right * Action_[AxisLeftRight] + Vector3.forward * Action_[AxisForwardBackward]))
                        * MoveForceMax);
                } else if (MoveType == MoveTypes.Velocity) {
                    // Player.GetComponentInChildren<Rigidbody>().velocity = (
                    //     Player.transform.TransformVector(
                    //         Vector3.right * Action_[AxisLeftRight] * MoveVelocityMax));
                    // Player.GetComponentInChildren<Rigidbody>().velocity = (
                    //     Player.transform.TransformVector(
                    //         Vector3.forward * Action_[AxisForwardBackward] * MoveVelocityMax));
                    Debug.LogError(
                        "Continuous control on velocity is not valid, since it will override the physic culculation");
                }
            }

            if (AllowTurn) {
                Player.GetComponentInChildren<Rigidbody>().angularVelocity = (
                    new Vector3(0.0f, Action_[AxisTurnLeftRight] * TurnVelocityMax, 0.0f)
                );
            }

            if (AllowJump) {
                // Only works if the agent is on the ground.
                // However, this judgement of if agent is on the groud could be buggy.
                // But no better way avaible for now.
                if (Player.GetComponentInChildren<Rigidbody>().velocity.y == 0f) {
                    Player.GetComponentInChildren<Rigidbody>().AddForce(
                        Player.transform.TransformVector(
                            Vector3.up * Utils.ClampNumber(Action_[AxisJump], 0f, 1f) * JumpForceMax
                        )
                    );
                }
            }
        } // ContinuousStep

        protected override void
        DiscreteContinuousStep()
        {
            base.DiscreteContinuousStep();
            // step reward functions
            if (IsRewardDistanceToTarget) {
                AddReward(
                    RewardFunctionDistanceToTarget.StepGetReward() * RewardDistanceCoefficient
                    * RewardSchemeScale);
            }
            if (IsRewardFacingTarget) {
                AddReward(
                    RewardFunctionFacingTarget.StepGetReward(
                        Player.transform.forward) * RewardDirectionCoefficient * RewardSchemeScale);
            }
            if (IsRewardTimePenalty) {
                AddReward(
                    RewardFunctionTimePenalty.StepGetReward() * RewardTimeCoefficient * RewardSchemeScale);
            }
        } // DiscreteContinuousStep
    }
}
