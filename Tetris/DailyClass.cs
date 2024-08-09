using System;
using System.Collections.Generic;
using System.Text;

namespace Tetris
{
    internal class Daily
    {
        public string description;
        public int required;
        public int current;
        public int reward;

        public Daily(string description, int required, int reward)
        {
            this.description = description;
            this.required = required;
            this.current = 0;
            this.reward = reward;
        }
    }

    internal class Avatar
    {
        public int required;
        public string path;

        public Avatar(int required, string path)
        {
            this.required = required;
            this.path = path;
        }
    }
}
