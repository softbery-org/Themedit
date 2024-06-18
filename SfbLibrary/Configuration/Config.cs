// Copyright (c) 2024 Softbery by Paweł Tobis
using System;

namespace SfbLibrary.Configuration
{
    public class Config
    {
        private const string VERSION = "3.10.1210";
        private string _name;
        private object _value;
        private string _section;
        private Type _type;

        public string Section { get => _section; }
        public string Name { get => _name; }
        public object Value { get => _value; private set => _value = value; }
        public Type ValueType { get; private set; }

        public Config(string section, string name, string value, Type typo)
        {
            _section = section;
            _name = name;
            _value = value;
            _type = typo;
        }
    }
}
