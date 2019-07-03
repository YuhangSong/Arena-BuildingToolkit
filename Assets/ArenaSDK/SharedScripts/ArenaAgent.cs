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
        /// Set differnt AgentID in inspector for each agent in a team.
        /// Different agent in the same team should have different AgentID, strictly follow 0, 1, 2, ...
        /// Agents in differnt teams can have same AgentID.
        /// </summary>
        public int AgentID;

        /// <summary>
        /// Get AgentID.
        /// </summary>
        /// <returns>AgentID.</returns>
        public int
        getAgentID()
        {
            return AgentID;
        }

        /// <summary>
        /// Setup display ID: the ID's color is the team color, and the ID's displayed number is the AgentID.
        /// This is usefull for agents to identify their teammates and collaborate with each other.
        /// Use the prefab ID as a child of the agent, or customize a object with 3D text on it.
        /// </summary>
        public GameObject ID;

        [Header("Reward Scheme")][Space(10)]

        /// <summary>
        /// RewardScheme at this level
        /// </summary>
        public RewardSchemes RewardScheme = RewardSchemes.NL;

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
        /// Reference to the Gun.
        /// </summary>
        public GameObject Gun;
        public GameObject BulletEmitter;
        public GameObject Bullet;

        public float BulletFarwardForce = 500f;
        public float NumBulletPerLoad   = 0.5f;
        public float FullNumBullet      = 30f;

        protected float NumBullet;
        protected bool Reloading;

        /// <summary>
        /// If enable attack with sword (SwordAttack).
        /// </summary>
        public bool AllowSwordAttack = false;

        /// <summary>
        /// Reference to the Sword.
        /// </summary>
        public GameObject Sword;

        /// <remarks>
        /// Get TeamID from the ArenaTeam object who should be the parent of ArenaAgent.
        /// </remarks>
        /// <returns>TeamID.</returns>
        public int
        getTeamID()
        {
            return GetComponentInParent<ArenaTeam>().getTeamID();
        }

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
        isLiving()
        {
            return Living;
        }

        /// <summary>
        /// Set the living status of the agent.
        /// </summary>
        /// <param name="Living_">The living status to be set.</param>
        private void
        setLiving(bool Living_)
        {
            if ((Living) && (!Living_)) {
                // if transfer from living to dead
                Living = Living_;
                getTeam().AtAnAgentDie();
            } else if ((!Living) && (Living_)) {
                // if transfer from dead to living
                Living = Living_;
                getTeam().AtAnAgentRespawn();
            }
            UpdateCanvas();
        }

        /// <summary>
        /// Reference to the team that is the agent's parent.
        /// </summary>
        private ArenaTeam Team;

        /// <summary>
        /// Get the reference to the team that is the agent's parent.
        /// </summary>
        /// <returns>The reference to the team that is the agent's parent.</returns>
        private ArenaTeam
        getTeam()
        {
            if (Team == null) {
                Team = GetComponentInParent<ArenaTeam>();
            }
            return Team;
        }

        /// <summary>
        /// Receive signal from outside
        /// </summary>
        /// <remarks>
        /// This method should only be called by GlobalManager.
        /// </remarks>
        /// <param name="NextState_">
        /// NextState_ can be
        ///    AgentNextStates.None: normal step
        ///    AgentNextStates.Die: change Living to false, ignore all Actions till receiving signal AgentNextStates.Done,
        ///           which will change Living back to true
        ///    AgentNextStates.Done: call Done(), Done() will call set Living back to true
        /// </param>
        /// <param name="NextReward_">The reward the agent will reveive at next step.</param>
        public void
        ReceiveSignal(AgentNextStates NextState_, float NextReward_)
        {
            AddReward(NextReward_);
            if (NextState_ == AgentNextStates.Die) {
                setLiving(false);
            }
            // Done signal has been processed by the base of GlobalManager, Academy
            // else if (NextState_ == AgentNextStates.Done) {
            //     Done();
            // }
        }

        protected Dictionary<string, UIPercentageBar> UIPercentageBars = new Dictionary<string, UIPercentageBar>();

        /// <summary>
        /// Agent should override InitializeAgent() and call base.InitializeAgent() before adding
        /// customized code.
        /// </summary>
        public override void
        InitializeAgent()
        {
            // base
            base.InitializeAgent();

            // tag and log
            tag = "Agent";
            Debug.Log(getLogTag() + " Initialize");

            // initialize reference to globalManager
            globalManager = GetComponentInParent<GlobalManager>();
            // set global agent settings: numberOfActionsBetweenDecisions
            agentParameters.numberOfActionsBetweenDecisions = globalManager.getNumberOfActionsBetweenDecisions();
            // set global agent settings: brain
            brain = globalManager.getAgentBrain();

            // initialize
            CheckConfig();
            InitializeDisplayID();
            AutoAssignCameraViewPort();
            if (globalManager.isTurnBasedGame()) {
                InitializeTurnBasedGame();
            }
            InitializeRewardFunction();

            // initialize reference to UIPercentageBars
            foreach (UIPercentageBar UIPercentageBar_ in GetComponentsInChildren<UIPercentageBar>()) {
                UIPercentageBars.Add(UIPercentageBar_.ID, UIPercentageBar_);
            }

            UIPercentageBars["ER"].Enable();
            UIPercentageBars["SR"].Enable();

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
        /// Called at each step when isLiving()==false.
        /// Agent should override StepDead() and call base.StepDead() before adding
        /// customized code.
        /// </summary>
        virtual protected void
        StepDead(){ }

        /// <summary>
        /// Called at each step when TurnState == TurnStates.Turn in turn-based game.
        /// Agent should override StepTurn() and call base.StepTurn() before adding
        /// customized code.
        /// </summary>
        virtual protected void
        StepTurn(){ }

        /// <summary>
        /// Called at each step when TurnState == TurnStates.NotTurn in turn-based game.
        /// Agent should override StepNotTurn() and call base.StepNotTurn() before adding
        /// customized code.
        /// </summary>
        virtual protected void
        StepNotTurn(){ }

        /// <summary>
        /// Called at each step when TurnState == TurnStates.Switching in turn-based game.
        /// Agent should override StepSwitching() and call base.StepSwitching() before adding
        /// customized code.
        /// </summary>
        virtual protected void
        StepSwitching(){ }

        /// <summary>
        /// Called at each step when TurnState == TurnStates.Switching or TurnState == TurnStates.NotTurn in turn-based game.
        /// Agent should override StepSwitchingOrNotTurn() and call base.StepSwitchingOrNotTurn() before adding
        /// customized code.
        /// </summary>
        virtual protected void
        StepSwitchingOrNotTurn(){ }

        /// <summary>
        /// Apply TeamMaterial of the agent to a GameObject.
        /// </summary>
        /// <param name="GameObject_">GameObject to be applied with TeamMaterial.</param>
        protected void
        ApplyTeamMaterial(GameObject GameObject_)
        {
            globalManager.ApplyTeamMaterial(getTeamID(), GameObject_);
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
        /// Get the log tag of the agent.
        /// </summary>
        /// <returns>LogTag.</returns>
        protected string
        getLogTag()
        {
            return tag + " T" + getTeamID() + " A" + getAgentID();
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
        getGlobalManager()
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
            setLiving(true);

            Debug.Log(
                getLogTag() + " Reset, CumulativeReward: "
                + GetCumulativeReward());

            UIPercentageBars["ER"].UpdateValue(GetCumulativeReward());
            UIPercentageBars["SR"].UpdateValue(GetReward());

            if (globalManager.isTurnBasedGame()) {
                ResetTurnBasedGame();
            }

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
            if (isLiving()) {
                // call turn based game step
                if (globalManager.isTurnBasedGame()) {
                    TurnState = globalManager.getTurnState(getTeamID(), getAgentID());
                    if ((TurnState == TurnStates.NotTurn) || (TurnState == TurnStates.Switching)) {
                        StepSwitchingOrNotTurn();
                    } else if (TurnState == TurnStates.Switching) {
                        StepSwitching();
                    } else if (TurnState == TurnStates.NotTurn) {
                        StepNotTurn();
                    } else if (TurnState == TurnStates.Turn) {
                        StepTurn();
                    }
                }

                bool IsActionTakingEffect = true;
                // if turn base game and at (NotTurn or Switching), no act
                if (globalManager.isTurnBasedGame()) {
                    if ((TurnState == TurnStates.NotTurn) || (TurnState == TurnStates.Switching)) {
                        IsActionTakingEffect = false;
                    }
                }

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
                UIPercentageBars["SR"].UpdateValue(GetReward());
            } else {
                StepDead();
            }
            if (globalManager.isDebugging()) {
                if ((getTeamID() == 0) && (getAgentID() == 0)) {
                    print(getLogTag() + " GetCumulativeReward " + GetCumulativeReward());
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
        /// State in turn-based game.
        /// </summary>
        private TurnStates TurnState = TurnStates.NotTurn;

        /// <summary>
        /// Initialize for turn-based game.
        /// </summary>
        private void
        InitializeTurnBasedGame(){ }

        /// <summary>
        /// Reset for turn-based game.
        /// </summary>
        private void
        ResetTurnBasedGame()
        {
            TurnState = TurnStates.NotTurn;
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
            // // TODO: text font size is differnt on differnt platform, depreciated for now, waiting for solutions
            // UpdateText();
        }

        /// <summary>
        /// Update mask in AgentCamera.
        /// </summary>
        private void
        UpdateMask()
        {
            Color MaskColor_ = GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().color;

            if (isLiving()) {
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
            string ToDisplay_ = getLogTag();
            Text DisplayText_ = GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();

            DisplayText_.color = globalManager.getStateTextColor(isLiving());

            if (isLiving()) {
                ToDisplay_ += "; Living";
            } else {
                ToDisplay_ += "; Dead";
            }

            if (globalManager.isTurnBasedGame()) {
                if (TurnState == TurnStates.NotTurn) {
                    ToDisplay_ += "; NotTurn";
                } else if (TurnState == TurnStates.Switching) {
                    ToDisplay_ += "; Switching";
                } else if (TurnState == TurnStates.Turn) {
                    ToDisplay_ += "; Turn (" + globalManager.getTurnPercentage().ToString("F2") + ")";
                }
            }

            DisplayText_.text     = ToDisplay_;
            DisplayText_.fontSize = (int) (47f / 0.5f * globalManager.getViewPortSize(ViewAxis.X));
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
                Utils.TextAllTextMeshesInChild(ID, getAgentID().ToString());
            } else {
                Debug.Log(
                    "No ID in this agent, this may cause the agent teammates hard to identidy each other, add the ID prefab in your agent.");
            }
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

        /// <summary>
        /// Collect Ram obs.
        /// Depreciated
        /// </summary>
        public override void
        CollectObservations()
        {
            AddVectorObs(getTeamID());
            AddVectorObs(getAgentID());
            AddVectorObs(globalManager.getRam());
        }

        /// <summary>
        /// Check if various configurations are valid or not.
        /// </summary>
        private void
        CheckConfig()
        {
            if ((RewardScheme != RewardSchemes.NL) && (RewardScheme != RewardSchemes.IS)) {
                Debug.LogError("RewardScheme at ArenaAgent level only support NL and IS.");
            }

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
        {
            if (RewardScheme == RewardSchemes.IS) {
                //
            } else if (RewardScheme == RewardSchemes.NL) {
                //
            } else {
                Debug.LogError("RewardFunction not valid.");
            }
        }
    }
}
