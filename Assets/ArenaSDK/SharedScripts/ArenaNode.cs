using UnityEngine;
using System.Collections.Generic;

namespace Arena
{
    /// <summary>
    /// ArenaTeam should attach to the object that is the parent of a batch of ArenaTeam.
    /// </summary>
    public class ArenaNode : ArenaBase
    {
        /// <summary>
        /// Called by the globalManager at initalize.
        /// Call base at the start of override code
        /// </summary>
        public override void
        Initialize()
        {
            base.Initialize();

            // Debug.Log(GetLogTag() + " Initialize");

            // if (globalManager.isTurnBasedGame()) {
            //     InitTurnBasedGame();
            // }

            InitializeRewardFunction();
        }

        /// <summary>
        /// Get the log tag of the node.
        /// </summary>
        /// <returns>LogTag.</returns>
        public override string
        GetLogTag()
        {
            if (GetParentNode() != null) {
                return GetParentNode().GetLogTag() + GetNodeID();
            } else {
                return "";
            }
        }

        [Header("Node Settings")][Space(10)]

        /// <summary>
        /// Set differnt NodeID in the inspector for different nodes.
        /// </summary>
        public int NodeID;

        /// <summary>
        /// Get the NodeID.
        /// </summary>
        /// <returns>The NodeID.</returns>
        public int
        GetNodeID()
        {
            return NodeID;
        }

        /// <summary>
        /// Get the number of all nodes in this node.
        /// </summary>
        /// <returns>The number of all nodes in this node.</returns>
        public int
        GetNumChildNodes()
        {
            int NumChildNodes_ = 0;

            foreach (ArenaNode ChildNode_ in Utils.GetTopLevelArenaNodesInChildren(gameObject)) {
                NumChildNodes_ += 1;
            }
            return NumChildNodes_;
        }

        /// <summary>
        /// </summary>
        private List<ArenaNode> ChildNodes = new List<ArenaNode>();
        public List<ArenaNode>
        GetChildNodes()
        {
            if (ChildNodes.Count != GetNumChildNodes()) {
                ChildNodes.Clear();
                foreach (ArenaNode ChildNode_ in Utils.GetTopLevelArenaNodesInChildren(gameObject)) {
                    ChildNodes.Add(ChildNode_);
                }
            }
            return ChildNodes;
        }

        /// <summary>
        /// </summary>
        private ArenaNode ParentNode;
        public ArenaNode
        GetParentNode()
        {
            if (ParentNode == null) {
                ParentNode = Utils.GetBottomLevelArenaNodeInGameObject(gameObject);
            }
            return ParentNode;
        }

        [Header("Living Condition Based On Child Nodes")][Space(10)]

        /// <summary>
        /// Condition at which the node is considerred to be living.
        /// </summary>
        public LivingConditions LivingCondition = LivingConditions.AtLeastOneLiving;

        /// <summary>
        /// Number of AtLeastSpecificNumberLiving in LivingCondition.
        /// </summary>
        public int AtLeastSpecificNumberLiving = 1;

        /// <summary>
        /// Portion of AtLeastSpecificPortionLiving in LivingCondition.
        /// </summary>
        public float AtLeastSpecificPortion = 0.5f;

        [Header("Reward Scheme")][Space(10)]

        /// <summary>
        /// Scale of RewardScheme at this level
        /// </summary>
        public float RewardSchemeScale = 10.0f;

        [Header("Reward Functions (Competitive)")][Space(10)]

        public bool IsRewardRanking = false;

        public RankingWinTypes RankingWinType;

        /// <summary>
        /// Storage the current ranking of the killing (How many teams have been killed till now.).
        /// </summary>
        private int ChildKilledRanking;

        /// <summary>
        /// Reset ChildKilledRankings and ChildKilledRanking.
        /// </summary>
        private void
        ResetChildKilledRanking()
        {
            ChildKilledRanking = 0;
        }

        public void
        IncrementChildKilledRanking()
        {
            ChildKilledRanking++;
        }

        public int
        GetChildKilledRanking()
        {
            return ChildKilledRanking;
        }

        /// <summary>
        /// </summary>
        private int KilledRanking;

        public void
        SetKilledRanking(int KilledRanking_)
        {
            KilledRanking = KilledRanking_;
        }

        public int
        GetKilledRanking()
        {
            return KilledRanking;
        }

        [Header("Reward Functions (Collaborative)")][Space(10)]

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

        public bool IsRewardTime = false;

        public TimeWinTypes TimeWinType = TimeWinTypes.Looger;

        /// <summary>
        /// If this node is living.
        /// </summary>
        private bool Living = true;

        /// <summary>
        /// Check if the node is living.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the node is living, <c>false</c> otherwise.
        /// </returns>
        public bool
        IsLiving()
        {
            return Living;
        }

        /// <summary>
        /// Called when this node is reset (top-down).
        /// </summary>
        public virtual void
        Reset()
        {
            Living = true;
            ResetRewardFunction();
            ResetChildKilledRanking();

            if (GetNumChildNodes() > 0) {
                foreach (ArenaNode ChildNode_ in GetChildNodes()) {
                    ChildNode_.Reset();
                }
            } else {
                if (gameObject.GetComponent<ArenaAgent>() != null) {
                    return;
                } else {
                    Debug.LogError("The very bottom ArenaNode should be attached with the ArenaAgent");
                }
            }
        }

        /// <summary>
        /// Called when this node is stepped (top-down).
        /// </summary>
        public virtual void
        Step()
        {
            StepRewardFunction();

            if (GetNumChildNodes() > 0) {
                foreach (ArenaNode ChildNode_ in GetChildNodes()) {
                    ChildNode_.Step();
                }
            } else {
                if (gameObject.GetComponent<ArenaAgent>() != null) {
                    return;
                } else {
                    Debug.LogError("The very bottom ArenaNode should be attached with the ArenaAgent");
                }
            }
        }

        /// <summary>
        /// Add reward for this node (top-down)
        ///   add the same reward for all child node
        /// </summary>
        /// <param name="Reward_">The reward to be added.</param>
        public void
        AddReward(float Reward_)
        {
            if (GetNumChildNodes() > 0) {
                foreach (ArenaNode ChildNode_ in GetChildNodes()) {
                    ChildNode_.AddReward(Reward_);
                }
            } else {
                if (gameObject.GetComponent<ArenaAgent>() != null) {
                    gameObject.GetComponent<ArenaAgent>().AddReward(Reward_);
                } else {
                    Debug.LogError("The very bottom ArenaNode should be attached with the ArenaAgent");
                }
            }
        }

        /// <summary>
        /// Get how many nodes are still living.
        /// </summary>
        /// <returns>The number of nodes that are still living.</returns>
        public int
        GetNumLivingChildNodes()
        {
            if (GetNumChildNodes() > 0) {
                int NumLivingNodes_ = 0;
                foreach (ArenaNode ChildNode_ in GetChildNodes()) {
                    if (ChildNode_.IsLiving()) {
                        NumLivingNodes_++;
                    }
                }
                return NumLivingNodes_;
            } else {
                if (gameObject.GetComponent<ArenaAgent>() != null) {
                    Debug.LogError(
                        "GetNumLivingChildNodes() should not be called at bottom level, i.e., the same level as ArenaAgent.");
                    return -1;
                } else {
                    Debug.LogError("The very bottom ArenaNode should be attached with the ArenaAgent");
                    return -1;
                }
            }
        }

        /// <summary>
        /// Called when this node is killed (bottom-up)
        ///   Call OnChildNodeKilled() for all parent nodes recursively
        /// </summary>
        public virtual void
        OnChildNodeKilled()
        {
            // Debug.Log(string.Format("{0} - OnChildNodeKilled()", GetLogTag()));

            if (IsLiving()) {
                // Debug.Log(string.Format("{0} - OnChildNodeKilled() - NumLivingNodes = {1}", GetLogTag(), GetNumLivingChildNodes()));

                bool IsLiving_ = true;
                if (LivingCondition == LivingConditions.AtLeastOneLiving) {
                    IsLiving_ = (GetNumLivingChildNodes() >= 1);
                } else if (LivingCondition == LivingConditions.AllLiving) {
                    IsLiving_ = ((float) GetNumLivingChildNodes() == GetNumChildNodes());
                } else if (LivingCondition == LivingConditions.AtLeastSpecificNumberLiving) {
                    IsLiving_ = ((GetNumLivingChildNodes() >= AtLeastSpecificNumberLiving));
                } else if (LivingCondition == LivingConditions.AtLeastSpecificPortionLiving) {
                    IsLiving_ = ((((float) GetNumLivingChildNodes() / GetNumChildNodes()) >= AtLeastSpecificPortion));
                } else {
                    Debug.LogError("Invalid LivingCondition.");
                }

                if (!IsLiving_) {
                    Kill();
                }
            }
        }

        /// <summary>
        /// </summary>
        public void
        Kill()
        {
            if (IsLiving()) {
                Living = false;

                // kill child nodes
                if (GetNumChildNodes() > 0) {
                    foreach (ArenaNode ChildNode_ in GetChildNodes()) {
                        ChildNode_.Kill();
                    }
                } else {
                    if (gameObject.GetComponent<ArenaAgent>() != null) {
                        gameObject.GetComponent<ArenaAgent>().SetLiving(false);
                    } else {
                        Debug.LogError("The very bottom ArenaNode should be attached with the ArenaAgent");
                    }
                }

                // record my ranking of being killed, for parent node to compute reward based on ranking
                if (GetParentNode() != null) {
                    if (GetParentNode().IsRewardRanking) {
                        SetKilledRanking(GetParentNode().GetChildKilledRanking());
                    }
                }

                // compute reward of ranking for child nodes
                if (IsRewardRanking) {
                    foreach (ArenaNode ChildNode_ in GetChildNodes()) {
                        float StepReward_ = 0f;
                        if (RankingWinType == RankingWinTypes.Survive) {
                            // Survive: dead ranking at 1 means reward 0, the higher the dead ranking, the better
                            StepReward_ += (float) ChildNode_.GetKilledRanking()
                              * globalManager.RewardRankingCoefficient;
                        } else if (RankingWinType == RankingWinTypes.Depart) {
                            // Depart: dead last (ranking getNumTeams()) means reward 0, the lower the dead ranking, the better
                            StepReward_ += ((float) GetNumChildNodes() - (float) ChildNode_.GetKilledRanking() - 1f)
                              * globalManager.RewardRankingCoefficient;
                        } else {
                            Debug.LogError("RankingWinType is invalid.");
                        }

                        ChildNode_.AddReward(StepReward_ * RewardSchemeScale);
                    }
                }

                // notice parent node that a child has been killed
                if (GetParentNode() != null) {
                    if (GetParentNode().IsRewardRanking) {
                        GetParentNode().IncrementChildKilledRanking();
                    }
                    GetParentNode().OnChildNodeKilled();
                } else {
                    gameObject.GetComponent<GlobalManager>().Done();
                }
            }
        } // Kill

        /// <summary>
        /// Initlize reward function.
        /// 1, check if RewardFunction is valid;
        /// 2, initalize according to RewardFunction;
        /// Customize InitializeRewardFunction should override InitializeRewardFunction() and call base.InitializeRewardFunction() before adding customized code.
        /// </summary>
        protected virtual void
        InitializeRewardFunction()
        {
            if (IsRewardDistance) {
                RewardFunctionDistance = new RewardFunctionGeneratorDistanceToTarget(
                    DistanceBase1,
                    DistanceBase2
                );
            }
        }

        /// <summary>
        /// Reset reward function.
        /// </summary>
        protected virtual void
        ResetRewardFunction()
        {
            if (IsRewardDistance) {
                RewardFunctionDistance.Reset();
            }
        }

        /// <summary>
        /// Step reward function.
        /// </summary>
        protected virtual void
        StepRewardFunction()
        {
            float StepReward_ = 0f;

            if (IsRewardDistance) {
                StepReward_ += (RewardFunctionDistance.StepGetReward() * globalManager.RewardDistanceCoefficient
                  * RewardSchemeScale);
            }

            if (IsRewardTime) {
                if (TimeWinType == TimeWinTypes.Looger) {
                    StepReward_ += globalManager.RewardTimeCoefficient;
                } else if (TimeWinType == TimeWinTypes.Shorter) {
                    StepReward_ -= globalManager.RewardTimeCoefficient;
                } else {
                    Debug.LogError("TimeWinType is invalid.");
                }
            }

            AddReward(StepReward_);
        }

        // /// <summary>
        // /// Get the NodeID of the Team that is current on turn.
        // /// </summary>
        // /// <returns>The NodeID of the Team that is current on turn.</returns>
        // public int
        // getCurrentTurnNodeID()
        // {
        //     return CurrentTurnNodeID;
        // }
        //
        // /// <summary>
        // /// Check an Team in this node is at turn by NodeID.
        // /// </summary>
        // /// <param name="NodeID_">The NodeID of which the Team you want to check.</param>
        // /// <returns>
        // /// <c>true</c>, if the Team is at turn, <c>false</c> otherwise.
        // /// </returns>
        // public bool
        // isCurrentTurnNodeID(int NodeID_)
        // {
        //     return (getCurrentTurnNodeID() == NodeID_);
        // }
        //
        // /// <summary>
        // /// Randomlized the Team that is currently at turn.
        // /// </summary>
        // public void
        // RandomCurrentTeamTurn()
        // {
        //     setCurrentTurnNodeID(getRandomNodeID());
        // }
        //
        // /// <summary>
        // /// Push the Team that is currently at turn to the next one (according to the order of NodeID),
        // /// </summary>
        // /// <returns>
        // /// <c>true</c>, if all Teams has been turned, <c>false</c> otherwise.
        // /// </returns>
        // public bool
        // NextTeamTurn(bool NoForwardIfAllTeamsTurned_)
        // {
        //     bool IsAllTeamsTurned = false;
        //
        //     if (isAllTeamsTurned()) {
        //         IsAllTeamsTurned = true;
        //         ClearTurnedTeam();
        //     }
        //
        //     if (!((IsAllTeamsTurned) && (NoForwardIfAllTeamsTurned_))) {
        //         PushCurrentTurnNodeIDForward();
        //         AddTurnedTeam(CurrentTurnNodeID);
        //     }
        //
        //     return IsAllTeamsTurned;
        // }
        //
        // /// <summary>
        // /// Get the NodeID of an Team randomly chosen from all Teams in this node.
        // /// </summary>
        // private int
        // getRandomNodeID()
        // {
        //     return Random.Range(0, getNumTeams());
        // }
        //
        // /// <summary>
        // /// Set the Team that is currently at turn by NodeID.
        // /// This will clear out the record of the Teams that has been turned in this node.
        // /// Thus, if your intention is to turn to another Team, do not use this,
        // /// call NextTurn instead.
        // /// </summary>
        // private void
        // setCurrentTurnNodeID(int NodeID_)
        // {
        //     CurrentTurnNodeID = NodeID_;
        //     ClearTurnedTeam();
        //     AddTurnedTeam(CurrentTurnNodeID);
        // }
        //
        // /// <summary>
        // /// A list of int indicating if each Team in this node has been turned (1) or not (0).
        // /// </summary>
        // private int[] TurnedTeams;
        //
        // /// <summary>
        // /// Mark a Team as turned by NodeID.
        // /// </summary>
        // /// <param name="NodeID_">The NodeID of which the Team will be marked as turned.</param>
        // private void
        // AddTurnedTeam(int NodeID_)
        // {
        //     TurnedTeams[NodeID_] = 1;
        // }
        //
        // /// <summary>
        // /// Clear the record of if each Team has been turned in TurnedTeams.
        // /// </summary>
        // private void
        // ClearTurnedTeam()
        // {
        //     System.Array.Clear(TurnedTeams, 0, getNumTeams());
        // }
        //
        // /// <summary>
        // /// Initialization for turn-based game.
        // /// </summary>
        // private void
        // InitTurnBasedGame()
        // {
        //     TurnedTeams = new int[getNumTeams()];
        // }
        //
        // /// <summary>
        // /// The NodeID of which the Team is currently at turn.
        // /// </summary>
        // private int CurrentTurnNodeID;
        //
        // /// <summary>
        // /// Switch turn to next Team by the order of NodeID.
        // /// </summary>
        // private void
        // PushCurrentTurnNodeIDForward()
        // {
        //     if (CurrentTurnNodeID < (getNumTeams() - 1)) {
        //         CurrentTurnNodeID += 1;
        //     } else {
        //         CurrentTurnNodeID = 0;
        //     }
        // }
        //
        // /// <summary>
        // /// Check if all Teams in this node have been turned.
        // /// </summary>
        // /// <returns>
        // /// <c>true</c>, if all Teams has been turned, <c>false</c> otherwise.
        // /// </returns>
        // private bool
        // isAllTeamsTurned()
        // {
        //     if (Utils.SumArray(TurnedTeams) == getNumTeams()) {
        //         return true;
        //     } else if (Utils.SumArray(TurnedTeams) < getNumTeams()) {
        //         return false;
        //     } else {
        //         Debug.LogWarning("TurnedTeams Error.");
        //         return false;
        //     }
        // }
    }
}
