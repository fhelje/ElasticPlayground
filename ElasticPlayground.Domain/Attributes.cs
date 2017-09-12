using System;
using System.Collections.Generic;
using System.Reflection;

namespace ElasticPlayground
{
    public class Attributes<T> : IEquatable<Attributes<T>>
    {
        public Attributes(string name, T value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; }
        public T Value { get; }

        public bool Equals(Attributes<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && EqualityComparer<T>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Attributes<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ EqualityComparer<T>.Default.GetHashCode(Value);
            }
        }

        public static bool operator ==(Attributes<T> left, Attributes<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Attributes<T> left, Attributes<T> right)
        {
            return !Equals(left, right);
        }

    }
}
