﻿namespace Tiver.Fowl.Core.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class StorageKeyNotFoundException : Exception, ISerializable
    {
        public StorageKeyNotFoundException()
        {
        }

        public StorageKeyNotFoundException(string message)
            : base(message)
        {
        }

        public StorageKeyNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected StorageKeyNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}