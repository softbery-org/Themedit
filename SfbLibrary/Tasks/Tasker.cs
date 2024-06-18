// Copyright (c) 2024 Softbery by Paweł Tobis
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SfbLibrary.Tasks
{
    public class Tasker : Task
    {
        private Action _action;
        private List<Action> _actions;
        private Task _task;
        private List<Task> _tasks;

        /*public event EventHandler<Task> OnCompleted;
        public event EventHandler<Task> OnFailed;
        public event EventHandler<Task> OnCancelled;
        public event EventHandler<Task> OnPaused;
        public event EventHandler<Task> OnStopped;*/
        
        public List<Task> List { get; set; }

        public List<Action> this[Action action]
        {
            get
            {
                return _actions;
            }
            private set
            {
                _actions = value;
            }
        }

        public Tasker(Action action) : base(action)
        {
            _action = action;
            _actions = new List<Action>();
            _task = new Task(_action);
            _tasks = new List<Task>();
            _task.RunSynchronously();
            _tasks.Add(_task);
        }

        /*public Tasker(Action action, string[] args) : base(action)
        {
            _action = action;
            _actions = new List<Action>();
            _task = new Task(_action);
            _tasks = new List<Task>();
            _task.RunSynchronously();
            _tasks.Add(_task);
        }*/

        public static class Console 
        { 
            public static void WriteLine(System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public MethodInvoker Invoke(MethodInvoker method)
        {
            return new((MethodInvoker)delegate {
                try {
                
                } catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                method.Invoke();
            });
        }

        public async Task Add(Action action)
        {
            try
            {
                _action = action;
                _actions = new List<Action>();
                _actions.Add(_action);
                _task = new Task(_action);
                _tasks = new List<Task>();

                await Task.Run(()=> {
                    this.Invoke((MethodInvoker)async delegate
                    {
                        var task_run = Task.Run(_task.Start);
                        _tasks.Add(task_run);
                        await task_run;
                    });
                    
                    _task.Start(); 

                });
                

            } 
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task GetTask(Task task)
        {
            try
            {
                if (_tasks.Contains(task))
                {
                    await this.List[_tasks.IndexOf(task)];
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /*public async void GetListItem(Action action)
        {
            
        }*/

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    public struct TTask
    {
          
    }

    public class TTaskResult : IAsyncResult
    {
        public bool IsCompleted => throw new NotImplementedException();

        public WaitHandle AsyncWaitHandle => throw new NotImplementedException();

        public object AsyncState => throw new NotImplementedException();

        public bool CompletedSynchronously => throw new NotImplementedException();
    }
}
