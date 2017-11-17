using AgpromaWebAPI.model;
using AgpromaWebAPI.Repository;
using AgpromaWebAPI.Viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgpromaWebAPI.Service
{
    //interface
    public interface ITaskServices
    {
        List<SignalRMaster> JoinGroup(int projectId);
        void SetConnectionId(string connectionId, int memberId);
        List<TaskBacklog> GetAll(int storyId);
        string Add(TaskBacklog backlog);
        void Update(int id, TaskBacklog res);
        int GetProjectId(int storyId);
        void Update_RemainingTime(ChecklistBacklog checklist);
    }

    public class TaskService : ITaskServices
    {
        private ITaskRepository _repository;
        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        //this method will add new task to the backlog
        public string Add(TaskBacklog backlog)
        {
            int plannedsize = _repository.GetStoryPlannedSize(backlog.StoryId);
            int sum = 0;
            sum = TotalSize(backlog.StoryId);
            sum += backlog.PlannedSize;
            //here we are comparing sum of task planned size of a user story with user story planned size
            if (sum <= plannedsize)
            {
                _repository.Add(backlog);
                return "matched";
            }
            else
            {
                return "unmatched";
            }

        }

        //this method will return all tasks of a user story
        public List<TaskBacklog> GetAll(int storyId)
        { 
            List<TaskBacklog> taskbacklog = _repository.GetAll(storyId);
            return taskbacklog;
        }

        //this method will update details of task
        public void Update(int id, TaskBacklog res)
        {
            _repository.Update(id, res);

        }

        //this method will set connection
        public void SetConnectionId(string connectionId, int memberId)
        {
            _repository.SetConnectionId(connectionId, memberId);
        }

        //this method will create group
        public List<SignalRMaster> JoinGroup(int projectId)
        {
            return _repository.JoinGroup(projectId);
        }

        //this method will return projectId
        public int GetProjectId(int storyId)
        {
            int sp = _repository.GetProjectId(storyId);
            return sp;
        }
        //this method will calculate planned Size of tasks of a particular storyId
        public int TotalSize(int userStoryId)
        {
            List<TaskBacklog> bck = _repository.GetAll(userStoryId);
            int sum = 0;
            foreach (TaskBacklog tb in bck)
            {
                sum = sum + tb.PlannedSize;
            }

            return sum;
        }

        //this method will update  remaining time in checklist
        public void Update_RemainingTime(ChecklistBacklog checklist)
        {
            _repository.Update_RemainingTime(checklist);
        }
    }
}