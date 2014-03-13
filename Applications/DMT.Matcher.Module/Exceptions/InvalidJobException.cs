using System;

namespace DMT.Matcher.Module
{
    [Serializable]
    public class InvalidJobException : Exception
    {
        public InvalidJobException() { }
        public InvalidJobException(string message) : base(message) { }
        public InvalidJobException(string message, Exception inner) : base(message, inner) { }
        protected InvalidJobException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}