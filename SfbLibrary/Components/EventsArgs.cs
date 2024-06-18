// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SfbLibrary.Components
{
    public class PanelLabelEventArgs : EventArgs
    {
        private string _label;
        private EventArgs _args;
        private MouseEventArgs _mouse;
        public MouseEventArgs Mouse
        {
            get => _mouse;
            set => _mouse = value;
        }
        public string PanelLabelName
        {
            get => _label;
            set => _label = value;
        }
        public EventArgs Args
        {
            get => _args;
            set => _args = value;
        }
        public PanelLabelEventArgs()
        {
            _label = string.Empty;
            _args = EventArgs.Empty;
            _mouse = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
        }
    }
}
