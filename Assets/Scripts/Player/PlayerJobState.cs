using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using System;
using UniRx;

public enum Job
{
    Simmilar,//村人
    Dual,//人狼
    Madman,//狂人
}
public static class PlayerJobSelecter
{
    public static Dictionary<Job, int> DJobs = new Dictionary<Job, int>() {
        /*
        {Job.Dual, 2},
        {Job.Madman, 1},
        {Job.Simmilar, 6}
        */
        {Job.Dual, 1},
        {Job.Madman, 0},
        {Job.Simmilar, 2}
    };
    public static List<Job> jobs = new List<Job>() { };
    public static void SetJobList()
    {
        jobs.Clear();
        foreach(var job in DJobs)
        {
            for(int j = 0; j < job.Value; j++)
            {
                jobs.Add(job.Key);
            }
        }
    }

    public static Job GetPlayerJob()
    {
        //リストにあるジョブをランダムに並び替える
        jobs = jobs.OrderBy(a => Guid.NewGuid()).ToList();
        //リストの最初の役職を返す
        Job job = jobs[0];
        jobs.RemoveAt(0);
        return job;
    }

}
[System.Serializable]
public class JobStateReactiveProperty : ReactiveProperty<Job>
{
    public JobStateReactiveProperty() { }

    public JobStateReactiveProperty(Job initialValue) : base(initialValue) { }

}
public class PlayerJobState : NetworkBehaviour
{
    //public Job playerjob;
    public JobStateReactiveProperty playerjob = new JobStateReactiveProperty();
    // Start is called before the first frame update
    void Start()
    {
        if (IsServer)
        {
            PlayerJobSelecter.SetJobList();
            playerjob.Value = PlayerJobSelecter.GetPlayerJob();
            SetPlayerJobClientRpc(playerjob.Value);
            Debug.Log(playerjob.Value.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    [ClientRpc]
    private void SetPlayerJobClientRpc(Job job)
    {
        playerjob.Value = job;
    }
}
