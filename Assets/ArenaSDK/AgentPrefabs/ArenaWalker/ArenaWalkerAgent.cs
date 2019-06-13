using UnityEngine;
using MLAgents;

namespace Arena {
    [RequireComponent(typeof(JointDriveController))] // Required to set joint forces
    public class ArenaWalkerAgent : ArenaRobot
    {
        // The agent is based on the Walker provided by Unity ML-Agents

        [Header("ArenaWalkerAgent Reward Functions")][Space(10)]
        public bool IsRewardWalkerPosture = true;

        public float RewardPostureCoefficient = 1.0f;

        // Keep as in the prefab is you are not creating new robot
        [Header("ArenaWalkerAgent Body Parts")][Space(10)]
        public Transform hips;
        public Transform chest;
        public Transform spine;
        public Transform head;
        public Transform thighL;
        public Transform shinL;
        public Transform footL;
        public Transform thighR;
        public Transform shinR;
        public Transform footR;
        public Transform armL;
        public Transform forearmL;
        public Transform handL;
        public Transform armR;
        public Transform forearmR;
        public Transform handR;

        protected override void
        InitializeBody()
        {
            jdController.SetupBodyPart(hips);
            jdController.SetupBodyPart(chest);
            jdController.SetupBodyPart(spine);
            jdController.SetupBodyPart(head);
            jdController.SetupBodyPart(thighL);
            jdController.SetupBodyPart(shinL);
            jdController.SetupBodyPart(footL);
            jdController.SetupBodyPart(thighR);
            jdController.SetupBodyPart(shinR);
            jdController.SetupBodyPart(footR);
            jdController.SetupBodyPart(armL);
            jdController.SetupBodyPart(forearmL);
            jdController.SetupBodyPart(handL);
            jdController.SetupBodyPart(armR);
            jdController.SetupBodyPart(forearmR);
            jdController.SetupBodyPart(handR);
        }

        protected override void
        JointUpdate(float[] Action_)
        {
            var bpDict = jdController.bodyPartsDict;
            int i      = -1;

            bpDict[chest].SetJointTargetRotation(Action_[++i], Action_[++i], Action_[++i]);
            bpDict[spine].SetJointTargetRotation(Action_[++i], Action_[++i], Action_[++i]);

            bpDict[thighL].SetJointTargetRotation(Action_[++i], Action_[++i], 0);
            bpDict[thighR].SetJointTargetRotation(Action_[++i], Action_[++i], 0);
            bpDict[shinL].SetJointTargetRotation(Action_[++i], 0, 0);
            bpDict[shinR].SetJointTargetRotation(Action_[++i], 0, 0);
            bpDict[footR].SetJointTargetRotation(Action_[++i], Action_[++i], Action_[++i]);
            bpDict[footL].SetJointTargetRotation(Action_[++i], Action_[++i], Action_[++i]);


            bpDict[armL].SetJointTargetRotation(Action_[++i], Action_[++i], 0);
            bpDict[armR].SetJointTargetRotation(Action_[++i], Action_[++i], 0);
            bpDict[forearmL].SetJointTargetRotation(Action_[++i], 0, 0);
            bpDict[forearmR].SetJointTargetRotation(Action_[++i], 0, 0);
            bpDict[head].SetJointTargetRotation(Action_[++i], Action_[++i], 0);

            // update joint strength settings
            bpDict[chest].SetJointStrength(Action_[++i]);
            bpDict[spine].SetJointStrength(Action_[++i]);
            bpDict[head].SetJointStrength(Action_[++i]);
            bpDict[thighL].SetJointStrength(Action_[++i]);
            bpDict[shinL].SetJointStrength(Action_[++i]);
            bpDict[footL].SetJointStrength(Action_[++i]);
            bpDict[thighR].SetJointStrength(Action_[++i]);
            bpDict[shinR].SetJointStrength(Action_[++i]);
            bpDict[footR].SetJointStrength(Action_[++i]);
            bpDict[armL].SetJointStrength(Action_[++i]);
            bpDict[forearmL].SetJointStrength(Action_[++i]);
            bpDict[armR].SetJointStrength(Action_[++i]);
            bpDict[forearmR].SetJointStrength(Action_[++i]);
        } // JointUpdate

        // // Set reward for this step according to mixture of the following elements.
        // // a. Velocity alignment with goal direction.
        // // b. Rotation alignment with goal direction.
        // // c. Encourage head height.
        // // d. Discourage head movement.
        // AddReward(
        //     +0.03f * Vector3.Dot(dirToTarget.normalized, jdController.bodyPartsDict[hips].rb.velocity)
        //     + 0.01f * Vector3.Dot(dirToTarget.normalized, hips.forward)
        //     + 0.02f * (head.position.y - hips.position.y)
        //     - 0.01f * Vector3.Distance(jdController.bodyPartsDict[head].rb.velocity,
        //     jdController.bodyPartsDict[hips].rb.velocity)
        // );

        private float
        GetWalkerPostureReward()
        {
            // 1, Encourage head height.
            // 2, Discourage head movement.
            return (
                +0.02f * (head.position.y - hips.position.y)
                - 0.01f * Vector3.Distance(jdController.bodyPartsDict[head].rb.velocity,
                jdController.bodyPartsDict[hips].rb.velocity)
            );
        }

        protected override void
        DiscreteContinuousStep()
        {
            base.DiscreteContinuousStep();
            if (IsRewardWalkerPosture) {
                AddReward(
                    GetWalkerPostureReward() * RewardPostureCoefficient);
            }
        } // DiscreteContinuousStep
    }
}
