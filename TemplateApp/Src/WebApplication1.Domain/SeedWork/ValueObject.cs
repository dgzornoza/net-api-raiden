using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebApplication1.Domain.SeedWork
{
    /// <summary>
    /// Domain value object base class
    /// </summary>
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        private List<PropertyInfo>? properties;
        private List<FieldInfo>? fields;

        public static bool operator ==(ValueObject obj1, ValueObject obj2)
        {
            if (Equals(obj1, null))
            {
                return Equals(obj2, null);
            }

            return obj1.Equals(obj2);
        }

        public static bool operator !=(ValueObject obj1, ValueObject obj2)
        {
            return !(obj1 == obj2);
        }

        public bool Equals(ValueObject? other)
        {
            return this.Equals(other as object);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            return
                this.GetProperties().All(p => this.PropertiesAreEqual(obj, p)) &&
                this.GetFields().All(f => this.FieldsAreEqual(obj, f));
        }

        public override int GetHashCode()
        {
            // allow overflow
            unchecked
            {
                var hash = 17;
                foreach (var prop in this.GetProperties())
                {
                    var value = prop.GetValue(this, null);
                    hash = HashValue(hash, value ?? 0);
                }

                foreach (var field in this.GetFields())
                {
                    var value = field.GetValue(this);
                    hash = HashValue(hash, value ?? 0);
                }

                return hash;
            }
        }

        protected static void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken())
            {
                throw new BusinessRuleValidationException(rule);
            }
        }

        private static int HashValue(int seed, object value)
        {
            var currentHash = value?.GetHashCode() ?? 0;

            return (seed * 23) + currentHash;
        }

        private bool PropertiesAreEqual(object obj, PropertyInfo p)
        {
            return Equals(p.GetValue(this, null), p.GetValue(obj, null));
        }

        private bool FieldsAreEqual(object obj, FieldInfo f)
        {
            return Equals(f.GetValue(this), f.GetValue(obj));
        }

        private IEnumerable<PropertyInfo> GetProperties()
        {
            if (this.properties == null)
            {
                this.properties = this.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => !Attribute.IsDefined(p, typeof(IgnoreMemberAttribute)))
                    .ToList();
            }

            return this.properties;
        }

        private IEnumerable<FieldInfo> GetFields()
        {
            if (this.fields == null)
            {
                this.fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(p => !Attribute.IsDefined(p, typeof(IgnoreMemberAttribute)))
                    .ToList();
            }

            return this.fields;
        }
    }
}
