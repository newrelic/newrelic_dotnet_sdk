using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewRelic.Platform.Sdk.Utils;

namespace NewRelic.Platform.Sdk.Binding
{
    internal class MetricData
    {
        #region Properties
        private string _name;
        private string _units;
        private int _count;
        private float _value;
        private float _minValue;
        private float _maxValue;
        private float _sumOfSquares;

        internal int Count { get { return _count; } }
        internal float Value { get { return _value; } }
        internal float MinValue { get { return _minValue; } }
        internal float MaxValue { get { return _maxValue; } }
        internal float SumOfSquares { get { return _sumOfSquares; } }
        internal string FullName { get { return string.Format("Component/{0}[{1}]", _name, _units); } }
        #endregion

        internal MetricData(string name, string units, float value) : this(name, units, 1, value, value, value, value * value)
        {
        }

        internal MetricData(string name, string units, int count, float value, float minValue, float maxValue, float sumOfSquares) 
        {
            this._name = name;
            this._units = units;
            this._count = count;
            this._value = value;
            this._minValue = minValue;
            this._maxValue = maxValue;
            this._sumOfSquares = sumOfSquares;
        }

        internal void AggregateWith(MetricData other)
        {
            this._count += other.Count;
            this._value += other.Value;
            this._minValue = Math.Min(_minValue, other.MinValue);
            this._maxValue = Math.Max(_maxValue, other.MaxValue);
            this._sumOfSquares += other.SumOfSquares;
        }

        internal Array Serialize()
        {
            return new float[] { _value, _count, _minValue, _maxValue, _sumOfSquares };
        }
    }
}
