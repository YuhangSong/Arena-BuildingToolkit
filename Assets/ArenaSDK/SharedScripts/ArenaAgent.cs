using UnityEngine;
using UnityEngine.UI;
using MLAgents;
using System.Collections.Generic;

namespace Arena
{
    /// <summary>
    /// Every agent should inherit the ArenaAgent.
    /// </summary>
    public class ArenaAgent : Agent
    {
        [Header("Agent Settings")][Space(10)]

        /// <summary>
        /// Setup display ID: the ID's color is the team color, and the ID's displayed number is the AgentID.
        /// This is usefull for agents to identify their teammates and collaborate with each other.
        /// Use the prefab ID as a child of the agent, or customize a object with 3D text on it.
        /// </summary>
        public GameObject ID;

        [Header("Reward Scheme")][Space(10)]

        /// <summary>
        /// Scale of RewardScheme at this level
        /// </summary>
        public float RewardSchemeScale = 1.0f;

        [Header("Action Settings (Attack)")][Space(10)]

        /// <summary>
        /// If enable attack with gun (GunAttack).
        /// </summary>
        public bool AllowGunAttack = false;

        /// <summary>
        /// Reference to the GunAttack related objects.
        /// </summary>
        public GameObject Gun;

        /// <summary>
        /// Reference to the GunAttack related objects.
        /// </summary>
        public GameObject BulletEmitter;

        /// <summary>
        /// Reference to the GunAttack related objects.
        /// </summary>
        public GameObject Bullet;

        /// <summary>
        /// GunAttack related configuration.
        /// </summary>
        public float BulletFarwardForce = 500f;

        /// <summary>
        /// GunAttack related configuration.
        /// </summary>
        public float NumBulletPerLoad = 0.5f;

        /// <summary>
        /// GunAttack related configuration.
        /// </summary>
        public float FullNumBullet = 30f;

        /// <summary>
        /// GunAttack related variables.
        /// </summary>
        protected float NumBullet;

        /// <summary>
        /// GunAttack related variables.
        /// </summary>
        protected bool Reloading;

        /// <summary>
        /// If enable attack with sword (SwordAttack).
        /// </summary>
        public bool AllowSwordAttack = false;

        /// <summary>
        /// Reference to the Sword.
        /// </summary>
        public GameObject Sword;

        /// <summary>
        /// If the agent is living.
        /// </summary>
        private bool Living = true;

        /// <summary>
        /// Check if the agent is living.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the agent is living, <c>false</c> otherwise.
        /// </returns>
        public bool
        IsLiving()
        {
            return Living;
        }

        /// <summary>
        /// Set the living status of the agent.
        /// </summary>
        /// <param name="Living_">The living status to be set.</param>
        public void
        SetLiving(bool Living_)
        {
            if ((Living) && (!Living_)) {
                // if transfer from living to dead
                Living = Living_;
                UIPercentageBars["HL"].UpdatePercentage(0f);
            } else if ((!Living) && (Living_)) {
                // if transfer from dead to living
                Living = Living_;
                UIPercentageBars["HL"].UpdatePercentage(1f);
            }
            UpdateCanvas();
        }

        /// <summary>
        /// References to UIPercentageBar.
        /// </summary>
        protected Dictionary<string, UIPercentageBar> UIPercentageBars = new Dictionary<string, UIPercentageBar>();

        /// <summary>
        /// References to UIText.
        /// </summary>
        protected Dictionary<string, UIText> UITexts = new Dictionary<string, UIText>();

        /// <summary>
        /// Agent should override InitializeAgent() and call base.InitializeAgent() before adding
        /// customized code.
        /// </summary>
        public override void
        InitializeAgent()
        {
            base.InitializeAgent();

            /** tag and log **/
            tag = "Agent";
            // Debug.Log(GetLogTag() + " Initialize");

            globalManager = GetComponentInParent<GlobalManager>();
            agentParameters.numberOfActionsBetweenDecisions = globalManager.getNumberOfActionsBetweenDecisions();
            brain = globalManager.getAgentBrain();

            CheckConfig();
            InitializeDisplayID();
            AutoAssignCameraViewPort();
            InitializeRewardFunction();

            // if (globalManager.isTurnBasedGame()) {
            //     InitializeTurnBasedGame();
            // }

            /** initialize reference to UIPercentageBars **/
            foreach (UIPercentageBar UIPercentageBar_ in GetComponentsInChildren<UIPercentageBar>()) {
                UIPercentageBars.Add(UIPercentageBar_.ID, UIPercentageBar_);
            }
            UIPercentageBars["ER"].Enable();
            UIPercentageBars["HL"].Enable(1f);

            /** initialize reference to UITexts **/
            foreach (UIText UIText_ in GetComponentsInChildren<UIText>()) {
                UITexts.Add(UIText_.ID, UIText_);
            }

            if (AllowGunAttack) {
                if (Gun == null) {
                    Debug.LogError("Must have a Gun assigned to AllowGunAttack");
                }
                if (BulletEmitter == null) {
                    Debug.LogError("Must have a BulletEmitter assigned to AllowGunAttack");
                }
                UIPercentageBars["AM"].Enable();
            } else {
                if (Gun != null) {
                    Gun.gameObject.SetActive(false);
                }
                if (BulletEmitter != null) {
                    BulletEmitter.gameObject.SetActive(false);
                }
                UIPercentageBars["AM"].Disable();
            }

            if (AllowSwordAttack) {
                if (Sword == null) {
                    Debug.LogError("Must have a Sword assigned to AllowSwordAttack");
                }
            } else {
                if (Sword != null) {
                    Sword.gameObject.SetActive(false);
                }
            }
        } // InitializeAgent

        /// <summary>
        /// Default Step function for disceret action space.
        /// Called at each step, environment transit by Action_ from this method.
        /// Agent should override DiscreteStep() and call base.DiscreteStep() before adding
        /// customized code.
        /// </summary>
        virtual protected void
        DiscreteStep(int Action_)
        {
            if (GetActionSpaceType() != SpaceType.discrete) {
                Debug.LogError("ActionSpaceType is not Discrete, DiscreteStep() should not be called.");
            }

            if (AllowGunAttack) {
                switch (Action_) {
                    case Attack:
                        if ((NumBullet > 0) && !Reloading) {
                            GameObject Temp_Bullet_Handeler;
                            Temp_Bullet_Handeler = Instantiate(Bullet, BulletEmitter.transform.position,
                                BulletEmitter.transform.rotation) as GameObject;
                            Temp_Bullet_Handeler.transform.SetParent(transform, true);
                            Temp_Bullet_Handeler.GetComponent<Rigidbody>().AddForce(
                                BulletEmitter.transform.up * BulletFarwardForce);
                            Destroy(Temp_Bullet_Handeler, 3.0f);
                            NumBullet -= 1.0f;
                            UIPercentageBars["AM"].UpdatePercentage(GetBulletPercentage());
                            if (NumBullet < 1.0f) {
                                Reloading = true;
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (Reloading) {
                    NumBullet += NumBulletPerLoad;
                    UIPercentageBars["AM"].UpdatePercentage(GetBulletPercentage());
                    if (NumBullet >= FullNumBullet) {
                        Reloading = false;
                    }
                }
            }
        } // DiscreteStep

        /// <summary>
        /// Default Step function for disceret action space.
        /// Called at each step, environment transit by Action_ from this method.
        /// Agent should override ContinuousStep() and call base.ContinuousStep() before adding
        /// customized code.
        /// </summary>
        virtual protected void
        ContinuousStep(float[] Action_)
        {
            if (GetActionSpaceType() != SpaceType.continuous) {
                Debug.LogError("ActionSpaceType is not Continuous, ContinuousStep() should not be called.");
            }
        }

        /// <summary>
        /// Default Step function called for both discrete and continous action space.
        /// Agent should override DiscreteContinuousStep() and call base.DiscreteContinuousStep() before adding
        /// customized code.
        /// This function is normally used for reward functions.
        /// </summary>
        virtual protected void
        DiscreteContinuousStep()
        { }

        /// <summary>
        /// Called at each step. Use this method the deal with LastDiscreteAction_ and Action_.
        /// For example, accumulate force with this method.
        /// Agent should override CheckHistoryAction() and call base.CheckHistoryAction() before adding
        /// customized code.
        /// </summary>
        virtual protected int
        CheckHistoryAction(int LastDiscreteAction_, int Action_)
        {
            if (GetActionSpaceType() != SpaceType.discrete) {
                Debug.LogError("ActionSpaceType is not Discrete, CheckHistoryAction() should not be called.");
            }
            return Action_;
        }

        /// <summary>
        /// Apply TeamMaterial of the agent to a GameObject.
        /// </summary>
        /// <param name="GameObject_">GameObject to be applied with TeamMaterial.</param>
        protected void
        ApplyTeamMaterial(GameObject GameObject_)
        {
            globalManager.ApplyTeamMaterial(
                getTeamID(), GameObject_);
        }

        /// <summary>
        /// Pre-defined action space: NoAction.
        /// </summary>
        protected const int NoAction = 0;

        /// <summary>
        /// Pre-defined action space: Left (MoveLeft).
        /// </summary>
        protected const int Left = 1;

        /// <summary>
        /// Pre-defined action space: Right (MoveRight).
        /// </summary>
        protected const int Right = 2;

        /// <summary>
        /// Pre-defined action axis for continous action space: move left or right.
        /// </summary>
        protected const int AxisLeftRight = 0;

        /// <summary>
        /// Pre-defined action space: Forward.
        /// </summary>
        protected const int Forward = 3;

        /// <summary>
        /// Pre-defined action space: Backward.
        /// </summary>
        protected const int Backward = 4;

        /// <summary>
        /// Pre-defined action axis for continous action space: move forward or backward.
        /// </summary>
        protected const int AxisForwardBackward = 1;

        /// <summary>
        /// Pre-defined action space: TurnLeft.
        /// </summary>
        protected const int TurnLeft = 5;

        /// <summary>
        /// Pre-defined action space: TurnRight.
        /// </summary>
        protected const int TurnRight = 6;

        /// <summary>
        /// Pre-defined action axis for continous action space: turn left or right.
        /// </summary>
        protected const int AxisTurnLeftRight = 2;

        /// <summary>
        /// Pre-defined action space: Jump.
        /// </summary>
        protected const int Jump = 7;

        /// <summary>
        /// Pre-defined action axis for continous action space: turn left or right.
        /// </summary>
        protected const int AxisJump = 3;

        /// <summary>
        /// Pre-defined action space: Attack.
        /// </summary>
        protected const int Attack = 8;

        /// <summary>
        /// Pre-defined action axis for continous action space: Attack.
        /// </summary>
        protected const int AxisAttack = 4;

        /// <summary>
        /// If you only use action in pre-defined action space,
        /// you are perfectly fine to use the GeneralPlayer Brain and GeneralLearner Brain.
        /// If you want to add customize action, you should start with:
        /// protected const int YourCustomizeAction0 = CustomizeActionStartAt;
        /// protected const int YourCustomizeAction1 = CustomizeActionStartAt + 1;
        /// protected const int YourCustomizeAction2 = CustomizeActionStartAt + 2;
        /// </summary>
        protected const int CustomizeActionStartAt = 9;

        protected const int CustomizeActionAxisStartAt = 5;

        /// <summary>
        /// Get the log tag of the object.
        /// </summary>
        /// <returns>LogTag.</returns>
        public string
        GetLogTag()
        {
            return gameObject.GetComponent<ArenaNode>().GetLogTag() + "-" + tag;
        }

        /// <summary>
        /// Reference to the GlobalManager.
        /// </summary>
        protected GlobalManager globalManager;

        /// <summary>
        /// Get reference to the GlobalManager.
        /// </summary>
        /// <returns>The reference to the GlobalManager.</returns>
        protected GlobalManager
        GetGlobalManager()
        {
            return globalManager;
        }

        /// <summary>
        /// The action taken at the last step.
        /// </summary>
        private int LastDiscreteAction;

        /// <summary>
        /// Called at agent reset. This is an interface override from MLAgents.
        /// </summary>
        public override void
        AgentReset()
        {
            base.AgentReset();
            SetLiving(true);

            Debug.Log(
                GetLogTag() + " Reset, CumulativeReward: "
                + GetCumulativeReward());

            UIPercentageBars["ER"].UpdateValue(GetCumulativeReward());

            // if (globalManager.isTurnBasedGame()) {
            //     ResetTurnBasedGame();
            // }

            if (AllowGunAttack) {
                NumBullet = Random.Range(0, FullNumBullet);
                UIPercentageBars["AM"].UpdatePercentage(GetBulletPercentage());
            }
        }

        private float
        GetBulletPercentage()
        {
            return Mathf.Clamp(((float) NumBullet / (float) FullNumBullet), 0f, 1f);
        }

        /// <summary>
        /// Called at agent step. This is an interface override from MLAgents.
        /// </summary>
        public override void
        AgentAction(float[] vectorAction, string textAction)
        {
            if (IsLiving()) {
                // // call turn based game step
                // if (globalManager.isTurnBasedGame()) {
                //     TurnState = globalManager.getTurnState(getTeamID(), getAgentID());
                //     if ((TurnState == TurnStates.NotTurn) || (TurnState == TurnStates.Switching)) {
                //         StepSwitchingOrNotTurn();
                //     } else if (TurnState == TurnStates.Switching) {
                //         StepSwitching();
                //     } else if (TurnState == TurnStates.NotTurn) {
                //         StepNotTurn();
                //     } else if (TurnState == TurnStates.Turn) {
                //         StepTurn();
                //     }
                // }

                bool IsActionTakingEffect = true;

                // // if turn base game and at (NotTurn or Switching), no act
                // if (globalManager.isTurnBasedGame()) {
                //     if ((TurnState == TurnStates.NotTurn) || (TurnState == TurnStates.Switching)) {
                //         IsActionTakingEffect = false;
                //     }
                // }

                if (IsActionTakingEffect) {
                    if (GetActionSpaceType() == SpaceType.discrete) {
                        // for disceret action space
                        int DiscreteAction_ = Mathf.FloorToInt(vectorAction[0]);
                        DiscreteAction_ = CheckHistoryAction(LastDiscreteAction, DiscreteAction_);
                        DiscreteStep(DiscreteAction_);
                        LastDiscreteAction = DiscreteAction_;
                    } else if (GetActionSpaceType() == SpaceType.continuous) {
                        // for continuous action space
                        ContinuousStep(vectorAction);
                    }
                    DiscreteContinuousStep();
                }

                UIPercentageBars["ER"].UpdateValue(GetCumulativeReward());
            } else {
                StepDead();
            }
            if (globalManager.isDebugging()) {
                if ((getTeamID() == 0) && (getAgentID() == 0)) {
                    print(GetLogTag() + " GetCumulativeReward " + GetCumulativeReward());
                }
            }
        } // AgentAction

        /// <summary>
        /// Get action space type.
        /// </summary>
        /// <returns>The the action space type.</returns>
        protected SpaceType
        GetActionSpaceType()
        {
            return globalManager.GetActionSpaceType();
        }

        /// <summary>
        /// Please use the prefab AgentCamera for agent camera.
        /// This AgentCamera contains a canvas, a text and a mask at the top of the field of view.
        /// The text will display the agent's TeamID, AgentID and state (living or dead).
        /// The mask will turn to black if the agent is dead.
        /// </summary>
        protected void
        UpdateCanvas()
        {
            UpdateMask();
            UpdateText();
        }

        /// <summary>
        /// Update mask in AgentCamera.
        /// </summary>
        private void
        UpdateMask()
        {
            Color MaskColor_ = GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().color;

            if (IsLiving()) {
                MaskColor_.a = 0f;
            } else {
                MaskColor_.a = 1f;
            }
            GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().color = MaskColor_;
        }

        /// <summary>
        /// Update text in AgentCamera.
        /// Depreciated
        /// </summary>
        private void
        UpdateText()
        {
            string ToDisplay_ = GetLogTag();

            if (IsLiving()) {
                ToDisplay_ += "; Living";
            } else {
                ToDisplay_ += "; Dead";
            }

            // if (globalManager.isTurnBasedGame()) {
            //     if (TurnState == TurnStates.NotTurn) {
            //         ToDisplay_ += "; NotTurn";
            //     } else if (TurnState == TurnStates.Switching) {
            //         ToDisplay_ += "; Switching";
            //     } else if (TurnState == TurnStates.Turn) {
            //         ToDisplay_ += "; Turn (" + globalManager.getTurnPercentage().ToString("F2") + ")";
            //     }
            // }

            UITexts["Status"].setColor(globalManager.getStateTextColor(IsLiving()));
            UITexts["Status"].setText(ToDisplay_);
        }

        /// <summary>
        /// Initialize the display of ID, including:
        /// 1, remove Collider
        /// 2, apply TeamMaterial, and make transparent
        /// 3, apply text
        /// </summary>
        private void
        InitializeDisplayID()
        {
            if (ID != null) {
                DestroyCollider(ID);
                ApplyTeamMaterial(ID);
                Utils.TransparentObject(ID);
                Utils.TextAllTextMeshesInChild(ID, gameObject.GetComponent<ArenaNode>().GetLogTag());
            } else {
                Debug.Log(
                    "No ID in this agent, this may cause the agent teammates hard to identidy each other, add the ID prefab in your agent.");
            }
        }

        public int
        getTeamID()
        {
            return gameObject.GetComponent<ArenaNode>().GetParentNode().GetNodeID();
        }

        public int
        getAgentID()
        {
            return gameObject.GetComponent<ArenaNode>().GetNodeID();
        }

        /// <summary>
        /// Destroy collider of the GameObject_.
        /// </summary>
        /// <param name="GameObject_">The GameObject of which the collider will be destroied.</param>
        private void
        DestroyCollider(GameObject GameObject_)
        {
            if (GameObject_.GetComponent<Collider>() != null) {
                Destroy(GameObject_.GetComponent<Collider>());
            }
        }

        /// <summary>
        /// Automatically assign the view port of the camera that is the chlid of the agent.
        /// </summary>
        private void
        AutoAssignCameraViewPort()
        {
            GetComponentInChildren<Camera>().rect = globalManager.getAgentViewPortRect(getTeamID(), getAgentID());
        }

        // /// <summary>
        // /// Collect Ram obs.
        // /// Depreciated
        // /// </summary>
        // public override void
        // CollectObservations()
        // {
        //     AddVectorObs(getTeamID());
        //     AddVectorObs(getAgentID());
        //     // AddVectorObs(globalManager.getRam());
        // }

        /// <summary>
        /// Check if various configurations are valid or not.
        /// </summary>
        private void
        CheckConfig()
        {
            // TODO: check if DiscreteStep() or DiscreteStep() has been overridden
            // if (GetActionSpaceType() == SpaceType.discrete) {
            //     if (typeof(ArenaAgent).GetMethod("DiscreteStep").DeclaringType == typeof(ArenaAgent)) {
            //         Debug.LogError("ActionSpaceType is Discrete, but you did not override DiscreteStep().");
            //     }
            // } else if (GetActionSpaceType() == SpaceType.continuous) {
            //     if (typeof(ArenaAgent).GetMethod("ContinuousStep").DeclaringType == typeof(ArenaAgent)) {
            //         Debug.LogError("ActionSpaceType is Continuous, but you did not override ContinuousStep().");
            //     }
            // } else {
            //     Debug.LogError("ActionSpaceType is invalid.");
            // }
        }

        /// <summary>
        /// Reset reward function.
        /// Warning: this function will not called automatically, since it is necessary to
        ///   call this after some customized AgentReset.
        /// </summary>
        protected virtual void
        ResetRewardFunction(){ }

        /// <summary>
        /// Initlize reward function.
        /// 1, check if RewardFunction is valid;
        /// 2, initalize according to RewardFunction;
        /// Customize InitializeRewardFunction should override InitializeRewardFunction() and call base.InitializeRewardFunction() before adding customized code.
        /// </summary>
        protected virtual void
        InitializeRewardFunction()
        { }

        /// <summary>
        /// Called at each step when IsLiving()==false.
        /// Agent should override StepDead() and call base.StepDead() before adding
        /// customized code.
        /// </summary>
        virtual protected void
        StepDead(){ }

        // /// <summary>
        // /// State in turn-based game.
        // /// </summary>
        // private TurnStates TurnState = TurnStates.NotTurn;
        //
        // /// <summary>
        // /// Initialize for turn-based game.
        // /// </summary>
        // private void
        // InitializeTurnBasedGame(){ }
        //
        // /// <summary>
        // /// Reset for turn-based game.
        // /// </summary>
        // private void
        // ResetTurnBasedGame()
        // {
        //     TurnState = TurnStates.NotTurn;
        // }
        //
        // /// <summary>
        // /// Called at each step when TurnState == TurnStates.Turn in turn-based game.
        // /// Agent should override StepTurn() and call base.StepTurn() before adding
        // /// customized code.
        // /// </summary>
        // virtual protected void
        // StepTurn(){ }
        //
        // /// <summary>
        // /// Called at each step when TurnState == TurnStates.NotTurn in turn-based game.
        // /// Agent should override StepNotTurn() and call base.StepNotTurn() before adding
        // /// customized code.
        // /// </summary>
        // virtual protected void
        // StepNotTurn(){ }
        //
        // /// <summary>
        // /// Called at each step when TurnState == TurnStates.Switching in turn-based game.
        // /// Agent should override StepSwitching() and call base.StepSwitching() before adding
        // /// customized code.
        // /// </summary>
        // virtual protected void
        // StepSwitching(){ }
        //
        // /// <summary>
        // /// Called at each step when TurnState == TurnStates.Switching or TurnState == TurnStates.NotTurn in turn-based game.
        // /// Agent should override StepSwitchingOrNotTurn() and call base.StepSwitchingOrNotTurn() before adding
        // /// customized code.
        // /// </summary>
        // virtual protected void
        // StepSwitchingOrNotTurn(){ }
    }
}
