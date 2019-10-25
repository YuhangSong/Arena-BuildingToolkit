using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class GobangAgent : Agent
{
    // necessary public reference
    public GobangAgent Competitor;
    public GobangGlobalManager globalManager;
    // customize public reference
    public GameObject chesspiece1;
    public GameObject chesspiece2;
    public GameObject selectrow;
    public int size_num;

    private int[,] memory;
    private int[] action_memory;
    private int flag_revert;
    
    //every chesspiece needs two steps.
    private int step_flag;
    private float center_x;
    private float center_y;
    private float center_z;
    private int last_action;
    private int loss_count;

    // Start is called before the first frame update
    void Start()
    {
        memory = new int[15,15];
        action_memory = new int[2];
        center_x = 5.0f;
        center_y = 0.5f;
        center_z = 5.0f;
        last_action = 0;
    }

    private void shared_trig_win_loss()
    {
        // send done signal to me, competitor and global
        Done();
        Competitor.Done();
        globalManager.Reset();
    }

    public void trig_tie()
    {
        Debug.Log(this.GetComponentInParent<GobangAgent>().tag + " trig_tie");
        this.AddReward(0.0f);
        Competitor.AddReward(0.0f);
        this.shared_trig_win_loss();
    }

    public void trig_win()
    {
        Debug.Log(this.GetComponentInParent<GobangAgent>().tag + " trig_win");
        this.AddReward(1.0f);
        Competitor.AddReward(0.0f);
        this.shared_trig_win_loss();
    }

    public void trig_loss()
    {
        Debug.Log(this.GetComponentInParent<GobangAgent>().tag + " trig_loss");
        this.AddReward(0.0f);
        Competitor.AddReward(1.0f);
        this.shared_trig_win_loss();
    }

    public int[,] getmemory()
    {
        return memory;
    }

    private int[,] addmemory(int[,] memory, int x, int y)
    {
        memory[x,y] = flag_revert; 
        return memory;
    }

    private void testwin(int size_num, int[,] memory){
        int cur_value = this.flag_revert;
        int max_num = 5;
        for (int i = 0; i < size_num; i++)
        {
            for (int j = 0; j < size_num; j++)
            {
                if (memory[i,j]!=0)
                {
                    //纵向判断
                    if (j<(size_num+1-max_num))
                    {
                        if (memory[i, j] == cur_value && memory[i, j + 1] == cur_value && memory[i, j + 2] == cur_value && memory[i, j + 3] == cur_value && memory[i, j + 4] == cur_value)
                        {
                           this.trig_win();
                        }
                    }
                    //横向判断
                    if (i<(size_num+1-max_num))
                    {
                        if (memory[i, j] == cur_value && memory[i + 1, j] == cur_value && memory[i + 2, j] == cur_value && memory[i + 3, j] == cur_value && memory[i + 4, j] == cur_value)
                        {
                            this.trig_win();
                        }
                    }
                    //斜向右下判断
                    if (i<(size_num+1-max_num)&&j<(size_num+1-max_num))
                    {
                        if (memory[i, j] == cur_value && memory[i + 1, j + 1] == cur_value && memory[i + 2, j + 2] == cur_value && memory[i + 3, j + 3] == cur_value && memory[i + 4, j + 4] == cur_value)
                        {
                            this.trig_win();
                        }
                    }
                    //斜向左下判断
                    if (i>=(max_num-1)&&j<(size_num+1-max_num))
                    {
                        if (memory[i, j] == cur_value && memory[i - 1, j + 1] == cur_value && memory[i - 2, j + 2] == cur_value && memory[i - 3, j + 3] == cur_value && memory[i - 4, j + 4] == cur_value)
                        {
                            this.trig_win();
                        }
                    }
                }
            }
        }
    }

    public override void AgentReset()
    {
        Debug.Log(this.GetComponentInParent<GobangAgent>().tag + " reset with reward " + this.GetReward());
        memory = new int[15,15];
        action_memory = new int[2];
        loss_count = 0;
        last_action = 0;
        globalManager.Reset();
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int action = Mathf.FloorToInt(vectorAction[0]);

        if (this.globalManager.get_turn() == this.tag){
            if (last_action>0 && action==0){
                step_flag++;
                memory = Competitor.getmemory();
                
                if (this.tag == "AgentA")
                {
                    flag_revert = this.globalManager.get_color();
                }
                else if (this.tag == "AgentB")
                {
                    flag_revert = this.globalManager.get_color()*-1;
                }

                if (step_flag % 2 == 1){
                    GameObject Temp_selectarea;
                    Temp_selectarea = Instantiate(selectrow, new Vector3(center_x*(last_action-1),center_y,35.0f), selectrow.transform.rotation) as GameObject;
                    action_memory[0] = last_action-1;
                }

                if (step_flag % 2 == 0){
                    step_flag = 0;
                    action_memory[1] = last_action-1;
                    foreach (GameObject each in GameObject.FindGameObjectsWithTag("selectarea")){
                        Destroy(each.gameObject);
                    }
                    if (memory[action_memory[0],action_memory[1]]==0){
                        GameObject cur_chesspiece;
                        if (flag_revert>0){
                            cur_chesspiece = Instantiate(chesspiece1, new Vector3(center_x*action_memory[0],center_y,center_z*action_memory[1]), chesspiece1.transform.rotation) as GameObject;
                        }
                        else{
                            cur_chesspiece = Instantiate(chesspiece2, new Vector3(center_x*action_memory[0],center_y,center_z*action_memory[1]), chesspiece2.transform.rotation) as GameObject;
                        }
                        loss_count = 0;
                        memory = this.addmemory(memory, action_memory[0],action_memory[1]);
                        this.globalManager.return_turn();
                        last_action = 0;
                    }
                    else{
                        loss_count++;
                        if(loss_count>2){
                            Competitor.trig_win();
                        }
                    }
                    //see if win
                    this.testwin(this.size_num, memory);
                    bool is_finish = false;
                    bool sweep_done = true;
                    for(int i_row=0;i_row<size_num;i_row++){
                        for(int i_coll=0;i_coll<size_num;i_coll++){
                            if(memory[i_row,i_coll]==0){
                                is_finish = true;
                                sweep_done = false;
                                break;
                            }
                        }
                        if(is_finish){
                            break;
                        }
                    }
                    if (sweep_done){
                        this.trig_tie();
                    }
                }
            }
            last_action = action;
        }     
    }

}
