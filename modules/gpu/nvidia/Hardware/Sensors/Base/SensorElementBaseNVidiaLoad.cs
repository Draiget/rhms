using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.Hardware;

namespace gpu_nvidia.Hardware.Sensors.Base
{
    public abstract class SensorElementBaseNVidiaLoad<T> : ISensorElement
    {
        private bool _isActive;
        protected double Value;

        private double _min;
        private double _max;

        protected SensorElementBaseNVidiaLoad() {
            _isActive = true;
        }

        public abstract void Update(T info);

        public void AfterUpdate() {
            CalculateMinMax(Value);
        }

        protected void CalculateMinMax(double value) {
            _min = Math.Min(_min, value);
            _max = Math.Max(_max, value);
        }

        public void SetActive(bool state) {
            _isActive = state;
        }

        public double GetMax() {
            return _max;
        }

        public double GetMin() {
            return _min;
        }

        public double GetValue() {
            return Value;
        }

        public abstract string GetMeasurement();

        public abstract string GetSystemTag();

        public bool IsActive() {
            return _isActive;
        }
    }
}
