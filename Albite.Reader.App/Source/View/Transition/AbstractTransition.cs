﻿using System;
using System.Windows.Media.Animation;

namespace Albite.Reader.App.View.Transition
{
    public class AbstractTransition : ITransition
    {
        public event EventHandler Completed;

        protected Storyboard Storyboard { get; private set; }
        private bool stopRequested = false;

        public AbstractTransition()
        {
            // Set up storyboard
            Storyboard = new Storyboard();
            Storyboard.Completed += storyboard_Completed;
        }

        public void Begin()
        {
            stopRequested = false;
            Storyboard.Begin();
        }

        public void Stop()
        {
            stopRequested = true;
            Storyboard.SkipToFill();
        }

        void storyboard_Completed(object sender, EventArgs e)
        {
            if (stopRequested)
            {
                return;
            }

            if (Completed != null)
            {
                Completed(this, EventArgs.Empty);
            }
        }
    }
}
