using SpiderDomain.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderCore.Config
{
    public class SpiderRunningTask
    {
        public GatherTypeEnum GatherType { get; set; } = GatherTypeEnum.GatherUser;

        /// <summary>
        /// 待处理用户集合
        /// </summary>
        public ConcurrentQueue<SinaUser> DoUsers { get; set; } = new ConcurrentQueue<SinaUser>();

        public ConcurrentDictionary<int, ThreadState> DoTasks { get; set; } = new ConcurrentDictionary<int, ThreadState>();

        public ConcurrentQueue<string> CancelUsers { get; set; } = new ConcurrentQueue<string>();


        List<string> _runUserIds { get; set; } = new List<string>();
        public void Reset()
        {
            this._runUserIds.Clear();
            this.DoUsers = new ConcurrentQueue<SinaUser>();
            this.DoTasks.Clear();
        }

        public bool AddUser(SinaUser user, bool append = true)
        {
            if (user != null && !this.DoUsers.Any(c => c.uid == user.uid))
            {
                if (!this._runUserIds.Contains(user.uid))
                {
                    this.DoUsers.Enqueue(user);
                    this._runUserIds.Add(user.uid);
                    return true;
                }
            }
            return false;
        }

        public SinaUser PeekUser()
        {
            SinaUser user = null;
            if (this.DoUsers.TryDequeue(out user)) return user;
            return null;
        }

        public bool CheckTaskRunning()
        {
            return this.DoTasks.Any(c => c.Value == ThreadState.Running);
        }

        public void NewTask(int taskId)
        {
            this.DoTasks.TryAdd(taskId, ThreadState.Running);
        }

        public void UpdateTask(int taskId, ThreadState newState)
        {
            var oldState = ThreadState.Suspended;
            if (newState == ThreadState.Suspended) oldState = ThreadState.Running;

            this.DoTasks.TryUpdate(taskId, newState, oldState);
        }

        public void CancelUser(string userId)
        {
            if (!this.CancelUsers.Contains(userId))
            {
                this.CancelUsers.Enqueue(userId);
            }
        }

        public bool CheckUserCanceled(string userId)
        {
            return this.CancelUsers.Contains(userId);
        }
    }
}
