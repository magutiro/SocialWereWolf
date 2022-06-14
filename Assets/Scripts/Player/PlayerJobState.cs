using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using System;

public enum Job
{
    Simmilar,//���l
    Dual,//�l�T
    Madman,//���l
}
public static class PlayerJobSelecter
{
    public static Dictionary<Job, int> DJobs = new Dictionary<Job, int>() {
        {Job.Dual, 2},
        {Job.Madman, 1},
        {Job.Simmilar, 6}
    };
    public static List<Job> jobs = new List<Job>() { };
    public static void SetJobList()
    {
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
        //���X�g�ɂ���W���u�������_���ɕ��ёւ���
        jobs = jobs.OrderBy(a => Guid.NewGuid()).ToList();
        //���X�g�̍ŏ��̖�E��Ԃ�
        Job job = jobs[0];
        jobs.RemoveAt(0);
        return job;
    }

}
public class PlayerJobState : NetworkBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
