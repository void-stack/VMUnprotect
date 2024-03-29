﻿using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VMUnprotect.Runtime.Helpers
{
    public class StringCounts
    {
        private readonly Dictionary<string, int> _strings = new(StringComparer.Ordinal);

        public IEnumerable<string> Strings => _strings.Keys;
        public int NumStrings => _strings.Count;

        protected void Add(string s) {
            _strings.TryGetValue(s, out var count);
            _strings[s] = count + 1;
        }

        private bool Exists(string s) {
            return _strings.ContainsKey(s);
        }

        public bool All(IEnumerable<string> list) {
            return list.All(Exists);
        }

        public bool Exactly(ICollection<string> list) {
            return list.Count == _strings.Count && All(list);
        }

        public int Count(string s) {
            _strings.TryGetValue(s, out var count);
            return count;
        }
    }


    public class LocalTypes : StringCounts
    {
        public LocalTypes(MethodDef method) {
            if (method is {Body: { }}) Initialize(method.Body.Variables);
        }

        public LocalTypes(IEnumerable<Local> locals) {
            Initialize(locals);
        }

        private void Initialize(IEnumerable<Local> locals) {
            foreach (var local in locals) Add(local.Type.FullName);
        }
    }
}