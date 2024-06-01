using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runedoku_Solver.Models
{
    public class MouseEventArgs : EventArgs
    {
        public readonly float MouseX;
        public readonly float MouseY;
        public MouseEventArgs(float mouseX, float mouseY)
        {
            MouseX = mouseX;
            MouseY = mouseY;
        }
    }
}
