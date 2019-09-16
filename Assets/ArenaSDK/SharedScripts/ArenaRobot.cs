using UnityEngine;
using MLAgents;

namespace Arena {
    public class ArenaRobot : ArenaAgent
    {
        [Header("Reward Functions (Isolated)")][Space(10)]

        public bool IsRewardDistanceToTarget = false;
        private RewardFunctionGeneratorDistanceToTarget RewardFunctionDistanceToTarget;

        public bool IsRewardVelocityToTarget = false;
        private RewardFunctionGeneratorVelocityToTarget RewardFunctionVelocityToTarget;

        public bool IsRewardFacingTarget = false;
        private RewardFunctionGeneratorFacingTarget RewardFunctionFacingTarget;

        public GameObject Target;
        public GameObject BodyCore;

        public bool IsRewardCoreUp = false;
        private RewardFunctionGeneratorKeepTowards RewardFunctionCoreUp;

        public bool IsRewardHeadUp = false;
        public RewardFunctionGeneratorFacing.Types RewardHeadUpType = RewardFunctionGeneratorFacing.Types.Binary_NZ;
        private RewardFunctionGeneratorFacing RewardFunctionHeadUp;

        public bool IsRewardTimePenalty = false;
        private RewardFunctionGeneratorTimePenalty RewardFunctionTimePenalty;

        [Header("Joint Settings")][Space(10)]

        protected JointDriveController jdController;

        // Keep as in the prefab is you are not creating new robot
        protected bool isNewDecisionStep;
        protected int currentDecisionStep;

        public override void
        InitializeAgent()
        {
            base.InitializeAgent();

            ReConfigSystemSettingsForRobot();

            jdController        = GetComponent<JointDriveController>();
            currentDecisionStep = 1;

            InitializeBody();
        } // InitializeAgent

        private void
        ReConfigSystemSettingsForRobot()
        {
            // We increase the Physics solver iterations in order to
            // make walker joint calculations more accurate.
            Monitor.verticalOffset = 1f;
            Physics.defaultSolverIterations         = 12;
            Physics.defaultSolverVelocityIterations = 12;
            Time.fixedDeltaTime   = 0.01333f; // (75fps). default is .2 (60fps)
            Time.maximumDeltaTime = .15f;     // Default is .33
        }

        protected virtual void
        InitializeBody(){ }

        protected override void
        InitializeRewardFunction()
        {
            base.InitializeRewardFunction();

            // create reward functions
            if (IsRewardDistanceToTarget) {
                RewardFunctionDistanceToTarget = new RewardFunctionGeneratorDistanceToTarget(
                    BodyCore,
                    Target
                );
            }
            if (IsRewardVelocityToTarget) {
                RewardFunctionVelocityToTarget = new RewardFunctionGeneratorVelocityToTarget(
                    BodyCore,
                    Target
                );
            }
            if (IsRewardFacingTarget) {
                RewardFunctionFacingTarget = new RewardFunctionGeneratorFacingTarget(
                    BodyCore,
                    Target,
                    RewardFunctionGeneratorFacingTarget.Types.Dot
                );
            }
            if (IsRewardCoreUp) {
                RewardFunctionCoreUp = new RewardFunctionGeneratorKeepTowards(
                    BodyCore,
                    Vector3.up
                );
            }
            if (IsRewardHeadUp) {
                RewardFunctionHeadUp = new RewardFunctionGeneratorFacing(
                    Vector3.up,
                    RewardHeadUpType,
                    0f,
                    0f
                );
            }
            if (IsRewardTimePenalty) {
                RewardFunctionTimePenalty = new RewardFunctionGeneratorTimePenalty(
                );
            }
        } // InitializeRewardFunction

        /// <summary>
        /// We only need to change the joint settings based on decision freq.
        /// </summary>
        public void
        IncrementDecisionTimer()
        {
            if (currentDecisionStep == agentParameters.numberOfActionsBetweenDecisions ||
              agentParameters.numberOfActionsBetweenDecisions == 1)
            {
                currentDecisionStep = 1;
                isNewDecisionStep   = true;
            } else {
                currentDecisionStep++;
                isNewDecisionStep = false;
            }
        }

        protected override void
        DiscreteContinuousStep()
        {
            base.DiscreteContinuousStep();
            if (IsRewardDistanceToTarget) {
                AddReward(
                    RewardFunctionDistanceToTarget.StepGetReward() * globalManager.RewardDistanceCoefficient
                    * RewardSchemeScale);
            }
            if (IsRewardVelocityToTarget) {
                AddReward(
                    RewardFunctionVelocityToTarget.StepGetReward() * globalManager.RewardVelocityCoefficient
                    * RewardSchemeScale);
            }
            if (IsRewardFacingTarget) {
                AddReward(
                    RewardFunctionFacingTarget.StepGetReward(
                        BodyCore.transform.forward) * globalManager.RewardDirectionCoefficient * RewardSchemeScale);
            }
            if (IsRewardCoreUp) {
                AddReward(
                    RewardFunctionCoreUp.StepGetReward() * globalManager.RewardDistanceCoefficient * RewardSchemeScale);
            }
            if (IsRewardHeadUp) {
                AddReward(
                    RewardFunctionHeadUp.StepGetReward(
                        BodyCore.transform.up) * globalManager.RewardDirectionCoefficient * RewardSchemeScale);
            }
            if (IsRewardTimePenalty) {
                AddReward(
                    RewardFunctionTimePenalty.StepGetReward() * globalManager.RewardTimeCoefficient
                    * RewardSchemeScale);
            }
        } // DiscreteContinuousStep

        private void
        ResetRobot()
        {
            foreach (var bodyPart in jdController.bodyPartsDict.Values) {
                bodyPart.Reset(bodyPart);
            }
            isNewDecisionStep   = true;
            currentDecisionStep = 1;
        }

        /// <summary>
        /// Loop over body parts and reset them to initial conditions.
        /// </summary>
        public override void
        AgentReset()
        {
            base.AgentReset();
            ResetRobot();
            ResetRewardFunction();
        } // AgentReset

        protected override void
        ResetRewardFunction()
        {
            // Set reward for this step according to mixture of the following elements.
            if (IsRewardDistanceToTarget) {
                RewardFunctionDistanceToTarget.Reset();
            }
            if (IsRewardVelocityToTarget) {
                RewardFunctionVelocityToTarget.Reset();
            }
            if (IsRewardFacingTarget) {
                RewardFunctionFacingTarget.Reset();
            }
            if (IsRewardCoreUp) {
                RewardFunctionCoreUp.Reset();
            }
            if (IsRewardHeadUp) {
                RewardFunctionHeadUp.Reset();
            }
            if (IsRewardTimePenalty) {
                RewardFunctionTimePenalty.Reset();
            }
        }

        protected override void
        ContinuousStep(float[] Action_)
        {
            base.ContinuousStep(Action_);

            // Joint update logic only needs to happen when a new decision is made
            if (isNewDecisionStep) {
                JointUpdate(Action_);
            }

            IncrementDecisionTimer();
        } // ContinuousStep

        protected virtual void
        JointUpdate(float[] Action_){ }
    }
}
