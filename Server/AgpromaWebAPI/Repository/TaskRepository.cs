using AgpromaWebAPI.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgpromaWebAPI.Repository
{
    //interface
    public interface ITaskRepository
    {
        List<SignalRMaster> JoinGroup(int projectId);
        void SetConnectionId(string connectionId, int memberId);
        List<TaskBacklog> GetAll(int storyId);
        void Add(TaskBacklog bklog);
        void Update(int id, TaskBacklog bklog);
        int GetProjectId(int userId);
        int GetStoryPlannedSize(int storyid);
        void Update_RemainingTime(ChecklistBacklog checklist);
    }

    public class TaskRepository : ITaskRepository
    {

        private AgpromaDbContext _context;
        public TaskRepository(AgpromaDbContext context)
        {
            _context = context;

        }

        //this method will add new task to a user story
        public void Add(TaskBacklog bklog)
        {
            bklog.ActualSize = bklog.PlannedSize;
            _context.Tasks.Add(bklog);
            _context.SaveChanges();
        }

        //this method will return tasks of a particular user story
        public List<TaskBacklog> GetAll(int storyId)
        {
            return _context.Tasks.Where(m => m.StoryId == storyId).ToList();
        }

        //this method is for updating details of a task
        public void Update(int id, TaskBacklog bklog)
        {
            TaskBacklog data = _context.Tasks.FirstOrDefault(m => m.TaskId == id);
            data.Title = bklog.Title;
            data.Description = bklog.Description;
            //  data. = bklog.Comments;
            _context.SaveChanges();
        }

        //updates the connection fields for the member
        public void SetConnectionId(string connectionId, int memberId)
        {
            SignalRMaster signalMaster = _context.SignalRDb.FirstOrDefault(m => m.MemberId == memberId);
            signalMaster.ConnectionId = connectionId;
            signalMaster.HubCode = HubCode.task;
            _context.SaveChanges();
        }

        //get the online member list and returned it
        public List<SignalRMaster> JoinGroup(int projectId)
        {
            List<Projectmembers> Projectmembers = _context.Projectmembers.Where(m => m.ProjectId == projectId).ToList();
            var memberIds = Projectmembers.Select(m => m.MemberId);
            List<SignalRMaster> onlineMembers = _context.SignalRDb.Where(m => m.Online == true && m.HubCode == HubCode.task).ToList();
            return onlineMembers.Where(n => memberIds.Contains(n.MemberId)).ToList();
        }

        //this method will return project id of userstory
        public int GetProjectId(int userStoryId)
        {
            var sprintid = _context.Sprint_UserStory.Where(s => s.StoryId == userStoryId).Select(u => u.SprintId).SingleOrDefault();

            return _context.Sprints.Where(s => s.SprintId == sprintid).Select(s => s.ProjectId).SingleOrDefault();
        }

        //this method will return planned size of user story 
        public int GetStoryPlannedSize(int storyid)
        {
            return _context.Userstories.Where(u => u.StoryId == storyid).Select(u => u.PlannedSize).SingleOrDefault();
        }

        //this method will update remaining time in checklist
        public void Update_RemainingTime(ChecklistBacklog checklist)
        {
            TaskBacklog task = _context.Tasks.FirstOrDefault(m => m.TaskId == checklist.TaskId);
            task.Remaining = task.Remaining - checklist.RemainingSize;
            _context.SaveChanges();
        }
    }

}