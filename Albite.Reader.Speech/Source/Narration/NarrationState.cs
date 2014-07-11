using Albite.Reader.Speech.Narration.Commands;

namespace Albite.Reader.Speech.Narration
{
    internal class NarrationState<TExpression> : INarrationState where TExpression : NarrationExpression
    {
        private object myLock = new object();

        private NarrationCommand command_;
        private TExpression expression_;
        private string language_;
        private float speed_;
        private bool stopped = false;

        public NarrationState(NarrationCommand command, string language, float speed)
        {
            set(command);
            language_ = language;
            speed_ = speed;
        }

        private void set(NarrationCommand command)
        {
            command_ = command;
            expression_ = getFirstExpression(command);
        }

        private static TExpression getFirstExpression(NarrationCommand command)
        {
            // Try searching forwards
            for (NarrationCommand c = command; c != null; c = c.Next)
            {
                if (c is TExpression)
                {
                    return (TExpression)c;
                }
            }

            // Try searching backwards
            for (NarrationCommand c = command; c != null; c = c.Previous)
            {
                if (c is TExpression)
                {
                    return (TExpression)c;
                }
            }

            return null;
        }

        public NarrationCommand Command
        {
            get
            {
                lock (myLock)
                {
                    return command_;
                }
            }

            set
            {
                lock (myLock)
                {
                    set(value);
                }
            }
        }

        public TExpression Expression
        {
            get
            {
                lock (myLock)
                {
                    return expression_;
                }
            }
        }

        public NarrationCommand Next()
        {
            lock (myLock)
            {
                // Cache the current command
                NarrationCommand command = command_;

                if (stopped || command == null)
                {
                    // No more commands
                    stopped = true;
                    return null;
                }

                if (command is TExpression)
                {
                    // Update the current expression
                    expression_ = (TExpression)command;
                }

                command_ = command.Next;
                return command;
            }
        }

        public string Language
        {
            get
            {
                lock (myLock)
                {
                    return language_;
                }
            }
            set
            {
                lock (myLock)
                {
                    language_ = value;
                }
            }
        }

        public float Speed
        {
            get
            {
                lock (myLock)
                {
                    return speed_;
                }
            }
            set
            {
                lock (myLock)
                {
                    speed_ = value;
                }
            }
        }
    }
}
