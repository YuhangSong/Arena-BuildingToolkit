using UnityEngine;

namespace Arena
{
    /// <summary>
    /// ArenaTeam should attach to the object that is the parent of a batch of agent.
    /// </summary>
    public class ArenaTeam : ArenaBase
    {
        [Header("Team Settings")][Space(10)]

        /// <summary>
        /// Set differnt TeamID in the inspector for different teams.
        /// </summary>
        public int TeamID;

        /// <summary>
        /// Get the TeamID.
        /// </summary>
        /// <returns>The TeamID.</returns>
        public int
        getTeamID()
        {
            return TeamID;
        }

        /// <summary>
        /// Condition at which the team is considerred to be living.
        ///   AlwaysLiving: This team is always living.
        ///   AtLeastOneLiving: This team is living when there is at least one agent living in this team.
        ///   AllLiving: This team is living when all agents are living in this team.
        /// </summary>
        public enum LivingConditions {
            AlwaysLiving,
            AtLeastOneLiving,
            AllLiving
        }

        /// <summary>
        /// Condition at which the team is considerred to be living.
        /// </summary>
        public LivingConditions LivingCondition = LivingConditions.AtLeastOneLiving;

        [Header("Reward Scheme")][Space(10)]

        /// <summary>
        /// RewardScheme at this level
        /// </summary>
        public RewardSchemes RewardScheme = RewardSchemes.NL;

        /// <summary>
        /// Scale of RewardScheme at this level
        /// </summary>
        public float RewardSchemeScale = 10.0f;

        [Header("Reward Functions")][Space(10)]

        /// <summary>
        /// Reward function based on the distance between DistanceBase1 and DistanceBase2.
        /// </summary>
        public bool IsRewardDistance = false;

        /// <summary>
        /// See IsRewardDistance.
        /// </summary>
        public GameObject DistanceBase1;

        /// <summary>
        /// See IsRewardDistance.
        /// </summary>
        public GameObject DistanceBase2;

        /// <summary>
        /// See IsRewardDistance.
        /// </summary>
        private RewardFunctionGeneratorDistanceToTarget RewardFunctionDistance;

        [Header("Reward Function Properties")][Space(10)]

        /// <summary>
        /// Properrties of reward function.
        /// Coeffient for integrating different reward functions
        /// </summary>
        public float RewardDistanceCoefficient  = 1.0f;
        public float RewardDirectionCoefficient = 0.01f;
        public float RewardTimeCoefficient      = 0.001f;

        [Header("Turn-based Game")][Space(10)]

        /// <summary>
        /// How to switch between agents:
        /// Sequence: according to the sequence of AgentID.
        /// None: used in GlobalManager to indicate the setting of
        ///       AgentSwitchType is determined by each team, instead of the GlobalManager,
        ///       by default, it is determined by GlobalManager.
        /// </summary>
        public AgentSwitchTypes AgentSwitchType;

        /// <summary>
        /// Get the log tag of the team.
        /// </summary>
        /// <returns>LogTag.</returns>
        protected new string
        getLogTag()
        {
            return tag + " T" + getTeamID();
        }

        /// <summary>
        /// Get the number of all agents in this team.
        /// </summary>
        /// <returns>The number of all agents in this team.</returns>
        public int
        getNumAgents()
        {
            if (Agents == null) {
                int NumAgents = 0;
                foreach (ArenaAgent each in GetComponentsInChildren<ArenaAgent>()) {
                    NumAgents += 1;
                }
                return NumAgents;
            } else {
                return Agents.Length;
            }
        }

        /// <summary>
        /// Get the reference to an agent in this team by AgentID.
        /// </summary>
        /// <param name="AgentID_">The AgentID of the agent you want to get.</param>
        /// <returns>The reference to the agent in this team.</returns>
        public ArenaAgent
        getAgent(int AgentID_)
        {
            if (CheckValidAgnetID(AgentID_)) {
                return Agents[AgentID_];
            } else {
                Debug.LogWarning("No AgentID " + AgentID_ + " in TeamID " + getTeamID());
                return null;
            }
        }

        /// <summary>
        /// Check if an AgentID is valid or not.
        /// </summary>
        /// <param name="AgentID_">The AgentID you want to check.</param>
        /// <returns>
        /// <c>true</c>, if the AgentID_ is valid, <c>false</c> otherwise.
        /// </returns>
        public bool
        CheckValidAgnetID(int AgentID_)
        {
            if ((AgentID_ >= 0) && (AgentID_ < getNumAgents())) {
                return true;
            } else {
                Debug.LogWarning("No AgentID " + AgentID_ + " in TeamID " + getTeamID());
                return false;
            }
        }

        /// <summary>
        /// Kill all agents in this team except one agent.
        /// </summary>
        /// <param name="AgentID_">The AgentID of the agent you want it to survive.</param>
        public void
        KillAllAgentsExcept(int AgentID_)
        {
            for (int i = 0; i < getNumAgents(); i++) {
                if (i != AgentID_) {
                    getAgent(i).ReceiveSignal(AgentNextStates.Die, 0f);
                }
            }
        }

        /// <summary>
        /// Kill all agents in this team.
        /// </summary>
        public void
        KillAllAgents()
        {
            SignalToAllAgents(AgentNextStates.Die, 0f);
        }

        /// <summary>
        /// Send reward to all agents in the team.
        /// </summary>
        /// <param name="NextReward_">The reward to be sent.</param>
        public void
        RewardAllAgents(float NextReward_)
        {
            SignalToAllAgents(AgentNextStates.None, NextReward_);
        }

        /// <summary>
        /// Signal to all agents in this team.
        /// Example:
        ///   // send reward signal NextReward to all agents in a team of TeamID
        ///   GetComponentInParent<GlobalManager>().getTeam(TeamID).SignalToAllAgents(AgentNextStates.None, NextReward);
        /// </summary>
        /// <param name="NextState_">
        /// NextState_ can be
        ///    AgentNextStates.None: normal step
        ///    AgentNextStates.Die: change Living to false, ignore all Actions till receiving signal AgentNextStates.Done,
        ///           which will change Living back to true
        ///    AgentNextStates.Done: change Living back to true, and call Done
        /// </param>
        /// <param name="NextReward_">The reward the agent will reveive at next step.</param>
        public void
        SignalToAllAgents(AgentNextStates NextState_, float NextReward_)
        {
            for (int i = 0; i < getNumAgents(); i++) {
                getAgent(i).ReceiveSignal(NextState_, NextReward_);
            }
        }

        /// <summary>
        /// If this team is living.
        /// </summary>
        public bool Living = true;

        /// <summary>
        /// Check if the team is living.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the team is living, <c>false</c> otherwise.
        /// </returns>
        public bool
        isLiving()
        {
            return Living;
        }

        /// <summary>
        /// Set the living status of the team.
        /// </summary>
        /// <param name="Living_">The living status to be set.</param>
        protected void
        setLiving(bool Living_)
        {
            if ((Living) && (!Living_)) {
                // if transfer from living to dead
                Living = Living_;
                getGlobalManager().AtAnTeamDie();
            } else if ((!Living) && (Living_)) {
                // if transfer from dead to living
                Living = Living_;
                getGlobalManager().AtAnTeamRespawn();
            }
        }

        /// <summary>
        /// Update living status of the team according to
        ///   1, LivingCondition
        ///   2, living status of all children agents.
        /// </summary>
        private void
        UpdateLiving()
        {
            if (LivingCondition == LivingConditions.AlwaysLiving) {
                setLiving(true);
                return;
            } else if (LivingCondition == LivingConditions.AtLeastOneLiving) {
                for (int i = 0; i < getNumAgents(); i++) {
                    if (getAgent(i).isLiving()) {
                        setLiving(true);
                        return;
                    }
                }
                setLiving(false);
                return;
            } else if (LivingCondition == LivingConditions.AllLiving) {
                for (int i = 0; i < getNumAgents(); i++) {
                    if (!getAgent(i).isLiving()) {
                        setLiving(false);
                    }
                }
                setLiving(true);
                return;
            } else {
                Debug.LogError("Invalid LivingCondition.");
                setLiving(true);
                return;
            }
        }

        /// <summary>
        /// Set AgentSwitchType.
        /// </summary>
        /// <param name="AgentSwitchType_">The AgentSwitchType to be set.</param>
        public void
        setAgentSwitchType(AgentSwitchTypes AgentSwitchType_)
        {
            AgentSwitchType = AgentSwitchType_;
        }

        /// <summary>
        /// Get AgentSwitchType.
        /// </summary>
        /// <returns>The AgentSwitchType.</returns>
        public AgentSwitchTypes
        getAgentSwitchType()
        {
            return AgentSwitchType;
        }

        /// <summary>
        /// Get the AgentID of the agent that is current on turn.
        /// </summary>
        /// <returns>The AgentID of the agent that is current on turn.</returns>
        public int
        getCurrentTurnAgentID()
        {
            return CurrentTurnAgentID;
        }

        /// <summary>
        /// Check an agent in this team is at turn by AgentID.
        /// </summary>
        /// <param name="AgentID_">The AgentID of which the agent you want to check.</param>
        /// <returns>
        /// <c>true</c>, if the agent is at turn, <c>false</c> otherwise.
        /// </returns>
        public bool
        isCurrentTurnAgentID(int AgentID_)
        {
            return (getCurrentTurnAgentID() == AgentID_);
        }

        /// <summary>
        /// Randomlized the agent that is currently at turn.
        /// </summary>
        public void
        RandomCurrentAgentTurn()
        {
            setCurrentTurnAgentID(getRandomAgentID());
        }

        /// <summary>
        /// Push the agent that is currently at turn to the next one (according to the order of AgentID),
        /// </summary>
        /// <returns>
        /// <c>true</c>, if all agents has been turned, <c>false</c> otherwise.
        /// </returns>
        public bool
        NextAgentTurn()
        {
            if (getAgentSwitchType() == AgentSwitchTypes.Sequence) {
                PushCurrentTurnAgentIDForward();
                AddTurnedAgent(CurrentTurnAgentID);
                if (isAllAgentsTurned()) {
                    ClearTurnedAgent();
                    return true;
                } else {
                    return false;
                }
            } else {
                Debug.LogWarning("AgentSwitchType not supported. Use the ones within AgentSwitchTypes.");
                return true;
            }
        }

        /// <summary>
        /// Referece to all agents in this team.
        /// </summary>
        private ArenaAgent[] Agents;

        /// <summary>
        /// Initialize referece to all agents in this team.
        /// </summary>
        private void
        InitReferenceToAgents()
        {
            Agents = new ArenaAgent[getNumAgents()];
            foreach (ArenaAgent each in GetComponentsInChildren<ArenaAgent>()) {
                Agents[each.getAgentID()] = each;
            }
        }

        /// <summary>
        /// Get the AgentID of an agent randomly chosen from all agents in this team.
        /// </summary>
        private int
        getRandomAgentID()
        {
            return Random.Range(0, getNumAgents());
        }

        /// <summary>
        /// Set the agent that is currently at turn by AgentID.
        /// This will clear out the record of the agents that has been turned in this team.
        /// Thus, if your intention is to turn to another agent, do not use this,
        /// call NextTurn instead.
        /// </summary>
        private void
        setCurrentTurnAgentID(int AgentID_)
        {
            CurrentTurnAgentID = AgentID_;
            ClearTurnedAgent();
            AddTurnedAgent(CurrentTurnAgentID);
        }

        /// <summary>
        /// A list of int indicating if each agent in this team has been turned (1) or not (0).
        /// </summary>
        private int[] TurnedAgents;

        /// <summary>
        /// Mark a agent as turned by AgentID.
        /// </summary>
        /// <param name="AgentID_">The AgentID of which the agent will be marked as turned.</param>
        private void
        AddTurnedAgent(int AgentID_)
        {
            TurnedAgents[AgentID_] = 1;
        }

        /// <summary>
        /// Clear the record of if each agent has been turned in TurnedAgents.
        /// </summary>
        private void
        ClearTurnedAgent()
        {
            System.Array.Clear(TurnedAgents, 0, getNumAgents());
        }

        /// <summary>
        /// Initialization for turn-based game.
        /// </summary>
        private void
        InitTurnBasedGame()
        {
            TurnedAgents = new int[getNumAgents()];
        }

        /// <summary>
        /// The AgentID of which the agent is currently at turn.
        /// </summary>
        private int CurrentTurnAgentID;

        /// <summary>
        /// Switch turn to next agent by the order of AgentID.
        /// </summary>
        private void
        PushCurrentTurnAgentIDForward()
        {
            if (CurrentTurnAgentID < (getNumAgents() - 1)) {
                CurrentTurnAgentID += 1;
            } else {
                CurrentTurnAgentID = 0;
            }
        }

        /// <summary>
        /// Check if all agents in this team have been turned.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if all agents has been turned, <c>false</c> otherwise.
        /// </returns>
        private bool
        isAllAgentsTurned()
        {
            if (Utils.SumArray(TurnedAgents) == getNumAgents()) {
                return true;
            } else if (Utils.SumArray(TurnedAgents) < getNumAgents()) {
                return false;
            } else {
                Debug.LogWarning("TurnedAgents Error.");
                return false;
            }
        }

        /// <summary>
        /// Called by the globalManager at initalize.
        /// Call base at the start of override code
        /// </summary>
        public override void
        Initialize()
        {
            base.Initialize();

            tag = "Team";
            Debug.Log(getLogTag() + " Initialize");

            InitReferenceToAgents();
            if (globalManager.isTurnBasedGame()) {
                InitTurnBasedGame();
            }

            InitializeRewardFunction();
        }

        /// <summary>
        /// Called by the globalManager at reset.
        /// Call base at the start of override code
        /// </summary>
        public virtual void
        Reset()
        {
            Debug.Log(getLogTag() + " Reset");
            ResetRewardFunction();
        }

        /// <summary>
        /// Called by the globalManager at step.
        /// Call base at the start of override code
        /// </summary>
        public virtual void
        Step()
        {
            StepRewardFunction();
        }

        /// <summary>
        /// Initlize reward function.
        /// 1, check if RewardFunction is valid;
        /// 2, initalize according to RewardFunction;
        /// Customize InitializeRewardFunction should override InitializeRewardFunction() and call base.InitializeRewardFunction() before adding customized code.
        /// </summary>
        protected virtual void
        InitializeRewardFunction()
        {
            if (RewardScheme == RewardSchemes.CL) {
                if (IsRewardDistance) {
                    RewardFunctionDistance = new RewardFunctionGeneratorDistanceToTarget(
                        DistanceBase1,
                        DistanceBase2
                    );
                }
            } else if (RewardScheme == RewardSchemes.NL) {
                //
            } else {
                Debug.LogError("RewardFunction not valid.");
            }
        }

        /// <summary>
        /// Reset reward function.
        /// </summary>
        protected virtual void
        ResetRewardFunction()
        {
            if (RewardScheme == RewardSchemes.CL) {
                if (IsRewardDistance) {
                    RewardFunctionDistance.Reset();
                }
            }
        }

        /// <summary>
        /// Step reward function.
        /// </summary>
        protected virtual void
        StepRewardFunction()
        {
            float StepReward_ = 0f;

            if (RewardScheme == RewardSchemes.CL) {
                if (IsRewardDistance) {
                    StepReward_ += (RewardFunctionDistance.StepGetReward() * RewardDistanceCoefficient
                      * RewardSchemeScale);
                }
            }

            RewardAllAgents(StepReward_);
        }

        /// <summary>
        /// Called at an agent die.
        /// </summary>
        public virtual void
        AtAnAgentDie()
        {
            UpdateLiving();
        }

        /// <summary>
        /// Called at an agent respawn.
        /// </summary>
        public virtual void
        AtAnAgentRespawn()
        {
            UpdateLiving();
        }
    }
}
