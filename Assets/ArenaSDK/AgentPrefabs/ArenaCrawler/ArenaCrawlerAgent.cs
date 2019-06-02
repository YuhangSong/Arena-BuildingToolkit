using UnityEngine;
using MLAgents;

namespace Arena {
    [RequireComponent(typeof(JointDriveController))] // Required to set joint forces
    public class ArenaCrawlerAgent : ArenaRobot
    {
        // Keep as in the prefab is you are not creating new robot
        [Header("Body Parts")][Space(10)]
        public Transform body;
        public Transform leg0Upper;
        public Transform leg0Lower;
        public Transform leg1Upper;
        public Transform leg1Lower;
        public Transform leg2Upper;
        public Transform leg2Lower;
        public Transform leg3Upper;
        public Transform leg3Lower;

        // Keep as in the prefab is you are not creating new robot
        [Header("Foot Grounded Visualization")][Space(10)]
        public bool useFootGroundedVisualization;
        public MeshRenderer foot0;
        public MeshRenderer foot1;
        public MeshRenderer foot2;
        public MeshRenderer foot3;
        public Material groundedMaterial;
        public Material unGroundedMaterial;

        protected override void
        InitializeBody()
        {
            base.InitializeBody();
            // Setup each body part
            jdController.SetupBodyPart(body);
            jdController.SetupBodyPart(leg0Upper);
            jdController.SetupBodyPart(leg0Lower);
            jdController.SetupBodyPart(leg1Upper);
            jdController.SetupBodyPart(leg1Lower);
            jdController.SetupBodyPart(leg2Upper);
            jdController.SetupBodyPart(leg2Lower);
            jdController.SetupBodyPart(leg3Upper);
            jdController.SetupBodyPart(leg3Lower);
        } // InitializeBody

        protected override void
        ContinuousStep(float[] Action_)
        {
            base.ContinuousStep(Action_);

            // If enabled the feet will light up green when the foot is grounded.
            // This is just a visualization and isn't necessary for function
            if (useFootGroundedVisualization) {
                foot0.material = jdController.bodyPartsDict[leg0Lower].groundContact.touchingGround ?
                  groundedMaterial :
                  unGroundedMaterial;
                foot1.material = jdController.bodyPartsDict[leg1Lower].groundContact.touchingGround ?
                  groundedMaterial :
                  unGroundedMaterial;
                foot2.material = jdController.bodyPartsDict[leg2Lower].groundContact.touchingGround ?
                  groundedMaterial :
                  unGroundedMaterial;
                foot3.material = jdController.bodyPartsDict[leg3Lower].groundContact.touchingGround ?
                  groundedMaterial :
                  unGroundedMaterial;
            }
        } // ContinuousStep

        protected override void
        JointUpdate(float[] Action_)
        {
            // The dictionary with all the body parts in it are in the jdController
            var bpDict = jdController.bodyPartsDict;

            int i = -1;

            // Pick a new Target joint rotation
            bpDict[leg0Upper].SetJointTargetRotation(Action_[++i], Action_[++i], 0);
            bpDict[leg1Upper].SetJointTargetRotation(Action_[++i], Action_[++i], 0);
            bpDict[leg2Upper].SetJointTargetRotation(Action_[++i], Action_[++i], 0);
            bpDict[leg3Upper].SetJointTargetRotation(Action_[++i], Action_[++i], 0);
            bpDict[leg0Lower].SetJointTargetRotation(Action_[++i], 0, 0);
            bpDict[leg1Lower].SetJointTargetRotation(Action_[++i], 0, 0);
            bpDict[leg2Lower].SetJointTargetRotation(Action_[++i], 0, 0);
            bpDict[leg3Lower].SetJointTargetRotation(Action_[++i], 0, 0);

            // Update joint strength
            bpDict[leg0Upper].SetJointStrength(Action_[++i]);
            bpDict[leg1Upper].SetJointStrength(Action_[++i]);
            bpDict[leg2Upper].SetJointStrength(Action_[++i]);
            bpDict[leg3Upper].SetJointStrength(Action_[++i]);
            bpDict[leg0Lower].SetJointStrength(Action_[++i]);
            bpDict[leg1Lower].SetJointStrength(Action_[++i]);
            bpDict[leg2Lower].SetJointStrength(Action_[++i]);
            bpDict[leg3Lower].SetJointStrength(Action_[++i]);
        }
    }
}
