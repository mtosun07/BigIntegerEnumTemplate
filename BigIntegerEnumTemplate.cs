using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace TSN.Templates
{
    [Serializable]
    public struct BigIntegerEnumTemplate : IComparable, IComparable<BigIntegerEnumTemplate>, IEquatable<BigIntegerEnumTemplate>, ISerializable
    {
        static BigIntegerEnumTemplate()
        {
            _allValues = new Lazy<IReadOnlyDictionary<string, Grants>>(() => new ReadOnlyDictionary<string, Grants>(typeof(Grants).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.DeclaringType.Equals(typeof(Grants))).Select(x => new { x.Name, Value = (Grants)x.GetValue(null) }).ToDictionary(x => x.Name, x => x.Value)));
        }
        public BigIntegerEnumTemplate(BigInteger value)
        {
            _value = value;
        }
        private BigIntegerEnumTemplate(SerializationInfo info, StreamingContext context)
        {
            _value = (BigInteger)(info?.GetValue(nameof(_value), typeof(BigInteger)) ?? throw new ArgumentNullException(nameof(info)));
        }


        private const string _seperator = ", ";
        
        private static readonly Lazy<IReadOnlyDictionary<string, BigIntegerEnumTemplate>> _allValues;
        public static readonly BigIntegerEnumTemplate None = BigInteger.Pow(2, 0);
        public static readonly BigIntegerEnumTemplate EnumValue1 = BigInteger.Pow(2, 1);
        public static readonly BigIntegerEnumTemplate EnumValue2 = BigInteger.Pow(2, 2);
        public static readonly BigIntegerEnumTemplate EnumValue3 = EnumValue1 | EnumValue2;


        private readonly BigInteger _value;

        public static IReadOnlyDictionary<string, BigIntegerEnumTemplate> AllValues => _allValues.Value;



        public static BigIntegerEnumTemplate FromByteArray(byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream(bytes))
                return (BigIntegerEnumTemplate)formatter.Deserialize(ms);
        }
        public static bool TryParse(string s, out BigIntegerEnumTemplate ug)
        {
            ug = BigInteger.Zero;
            if (s == null)
                return false;
            foreach (var name in s.Split(new[] { _seperator }, StringSplitOptions.None))
                if (_allValues.Value.TryGetValue(s, out var x))
                    ug |= x;
                else
                {
                    ug = BigInteger.Zero;
                    return false;
                }
            return true;
        }
        public static BigIntegerEnumTemplate Parse(string s) => TryParse(s, out var ug) ? ug : throw new FormatException();

        public byte[] ToByteArray()
        {
            var formatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, this);
                return ms.ToArray();
            }
        }
        public bool IsFlagDefined() => !BigInteger.TryParse(ToString(), out _);

        public override string ToString()
        {
            if (_allValues.Value.Count == 0)
                return _value.ToString();
            var allValues = _allValues.Value.Select(x => new { Name = x.Key, Value = x.Value._value }).ToArray();
            var sep = _seperator ?? string.Empty;
            var num = _value;
            int num2 = allValues.Length - 1;
            var stringBuilder = new StringBuilder();
            bool flag = true;
            var num3 = num;
            while (num2 >= 0 && (num2 != 0 || allValues[num2].Value != BigInteger.Zero))
            {
                if ((num & allValues[num2].Value) == allValues[num2].Value)
                {
                    num -= allValues[num2].Value;
                    if (!flag)
                        stringBuilder.Insert(0, sep);
                    stringBuilder.Insert(0, allValues[num2].Name);
                    flag = false;
                }
                num2--;
            }
            if (num != BigInteger.Zero)
                return _value.ToString();
            if (num3 == BigInteger.Zero)
            {
                if (allValues.Length != 0 && allValues[0].Value == BigInteger.Zero)
                    return allValues[0].Name;
                return "0";
            }
            return stringBuilder.ToString();
        }
        public override int GetHashCode() => _value.GetHashCode();
        public override bool Equals(object obj) => obj is BigIntegerEnumTemplate ug && Equals(ug);

        public int CompareTo(object obj) => _value.CompareTo(obj);
        public int CompareTo(BigIntegerEnumTemplate other) => _value.CompareTo(other._value);
        public bool Equals(BigIntegerEnumTemplate other) => _value.Equals(other._value);
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            (info ?? throw new ArgumentNullException(nameof(info))).AddValue(nameof(_value), _value, typeof(BigInteger));
        }

        public static implicit operator BigIntegerEnumTemplate(BigInteger value) => new BigIntegerEnumTemplate(value);
        public static implicit operator BigInteger(BigIntegerEnumTemplate value) => value._value;

        public static BigIntegerEnumTemplate operator &(BigIntegerEnumTemplate left, BigIntegerEnumTemplate right) => left._value & right._value;
        public static BigIntegerEnumTemplate operator |(BigIntegerEnumTemplate left, BigIntegerEnumTemplate right) => left._value | right._value;
        public static BigIntegerEnumTemplate operator ^(BigIntegerEnumTemplate left, BigIntegerEnumTemplate right) => left._value ^ right._value;
        public static BigIntegerEnumTemplate operator <<(BigIntegerEnumTemplate value, int shift) => value._value << shift;
        public static BigIntegerEnumTemplate operator >>(BigIntegerEnumTemplate value, int shift) => value._value >> shift;
        public static BigIntegerEnumTemplate operator ~(BigIntegerEnumTemplate value) => ~value._value;
        public static BigIntegerEnumTemplate operator -(BigIntegerEnumTemplate value) => -value._value;
        public static BigIntegerEnumTemplate operator +(BigIntegerEnumTemplate value) => +value._value;
        public static BigIntegerEnumTemplate operator ++(BigIntegerEnumTemplate value) => value._value + BigInteger.One;
        public static BigIntegerEnumTemplate operator --(BigIntegerEnumTemplate value) => value._value - BigInteger.One;
        public static BigIntegerEnumTemplate operator +(BigIntegerEnumTemplate left, BigIntegerEnumTemplate right) => left._value + right._value;
        public static BigIntegerEnumTemplate operator -(BigIntegerEnumTemplate left, BigIntegerEnumTemplate right) => left._value - right._value;
        public static BigIntegerEnumTemplate operator *(BigIntegerEnumTemplate left, BigIntegerEnumTemplate right) => left._value * right._value;
        public static BigIntegerEnumTemplate operator %(BigIntegerEnumTemplate dividend, BigIntegerEnumTemplate divisor) => dividend._value % divisor._value;
        public static bool operator <(BigIntegerEnumTemplate left, BigIntegerEnumTemplate right) => left._value < right._value;
        public static bool operator <=(BigIntegerEnumTemplate left, BigIntegerEnumTemplate right) => left._value <= right._value;
        public static bool operator >(BigIntegerEnumTemplate left, BigIntegerEnumTemplate right) => left._value > right._value;
        public static bool operator >=(BigIntegerEnumTemplate left, BigIntegerEnumTemplate right) => left._value >= right._value;
        public static bool operator ==(BigIntegerEnumTemplate left, BigIntegerEnumTemplate right) => left._value == right._value;
        public static bool operator !=(BigIntegerEnumTemplate left, BigIntegerEnumTemplate right) => left._value != right._value;
        public static bool operator <(BigIntegerEnumTemplate left, long right) => left._value < right;
        public static bool operator <=(BigIntegerEnumTemplate left, long right) => left._value <= right;
        public static bool operator >(BigIntegerEnumTemplate left, long right) => left._value > right;
        public static bool operator >=(BigIntegerEnumTemplate left, long right) => left._value >= right;
        public static bool operator ==(BigIntegerEnumTemplate left, long right) => left._value == right;
        public static bool operator !=(BigIntegerEnumTemplate left, long right) => left._value != right;
        public static bool operator <(long left, BigIntegerEnumTemplate right) => left < right._value;
        public static bool operator <=(long left, BigIntegerEnumTemplate right) => left <= right._value;
        public static bool operator >(long left, BigIntegerEnumTemplate right) => left > right._value;
        public static bool operator >=(long left, BigIntegerEnumTemplate right) => left >= right._value;
        public static bool operator ==(long left, BigIntegerEnumTemplate right) => left == right._value;
        public static bool operator !=(long left, BigIntegerEnumTemplate right) => left != right._value;
        public static bool operator <(BigIntegerEnumTemplate left, ulong right) => left._value < right;
        public static bool operator <=(BigIntegerEnumTemplate left, ulong right) => left._value <= right;
        public static bool operator >(BigIntegerEnumTemplate left, ulong right) => left._value > right;
        public static bool operator >=(BigIntegerEnumTemplate left, ulong right) => left._value >= right;
        public static bool operator ==(BigIntegerEnumTemplate left, ulong right) => left._value == right;
        public static bool operator !=(BigIntegerEnumTemplate left, ulong right) => left._value != right;
        public static bool operator <(ulong left, BigIntegerEnumTemplate right) => left < right._value;
        public static bool operator <=(ulong left, BigIntegerEnumTemplate right) => left <= right._value;
        public static bool operator >(ulong left, BigIntegerEnumTemplate right) => left > right._value;
        public static bool operator >=(ulong left, BigIntegerEnumTemplate right) => left >= right._value;
        public static bool operator ==(ulong left, BigIntegerEnumTemplate right) => left == right._value;
        public static bool operator !=(ulong left, BigIntegerEnumTemplate right) => left != right._value;
    }
}
