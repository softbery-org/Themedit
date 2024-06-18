// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfbLibrary.Hooks
{
    public class HookManager
    {
        private List<IHookDevices> _devices;
        public List<IHookDevices> Devices { get => _devices; }

        public HookManager()
        {
            _devices = new List<IHookDevices>();

        }

        public HookManager(object control)
        {
            _devices = new List<IHookDevices>();
        }
    }

    public class Hook
    {
        public string Name { get; set; }
        public Type DeviceType { get; set; }
    }
}
