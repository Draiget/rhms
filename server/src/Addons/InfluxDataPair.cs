using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;

namespace server.Addons
{
    public class InfluxDataTag : InfluxDataPair
    {
        public InfluxDataTag(string key, string value) 
            : base(key, value, true) {
        }
    }

    public class InfluxDataPair
    {
        public string Key;
        public string Value;
        public bool IsTag;

        public InfluxDataPair(string key, string value, bool isTag = false) {
            Key = key;
            Value = value;
            IsTag = isTag;
        }
    }

    public class InfluxWriteData
    {
        private readonly string _tableTag;

        private Dictionary<string, string> _tags;
        private Dictionary<string, string> _values;

        public InfluxWriteData(string table, params InfluxDataPair[] values) {
            _tableTag = table;
            _tags = new Dictionary<string, string>();
            _values = new Dictionary<string, string>();

            foreach (var value in values) {
                if (value.IsTag) {
                    AddTags(value);
                    continue;
                }

                AddValues(value);
            }
        }

        public void AddTags(params InfluxDataPair[] values) {
            foreach (var value in values) {
                _tags.Add(value.Key, value.Value);
            }
        }

        public void AddValues(params InfluxDataPair[] values) {
            foreach (var value in values) {
                _values.Add(value.Key, value.Value);
            }
        }

        public bool IsValidQuery(){
            return _tags.Count > 0 && _values.Count > 0;
        }

        public string QueryRequest {
            get {
                if (!IsValidQuery()) {
                    return null;
                }

                var req = $"{_tableTag}";
                foreach (var tag in _tags) {
                    req += $",{tag.Key}={tag.Value}";
                }

                req += " ";
                var isFirst = false;
                foreach (var value in _values) {
                    req += (isFirst ? ":" : "") + $"{value.Key}={value.Value}";
                    isFirst = true;
                }

                return req;
            }
        }
    }
}
