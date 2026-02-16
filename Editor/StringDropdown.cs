using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace Damdor.VisualStates.Editor
{
    internal class StringDropdown : AdvancedDropdown
    {
        private readonly string[] choices;
        private readonly Action<string> onChoice;

        private readonly Dictionary<string, AdvancedDropdownItem> parents = new();
        private readonly Dictionary<AdvancedDropdownItem, string> itemToChoice = new();
            
        public StringDropdown(string[] choices, Action<string> onChoice) : base(new AdvancedDropdownState())
        {
            this.choices = choices;
            this.onChoice = onChoice;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("");
            parents[""] = root;

            foreach (var path in choices)
            {
                var parent = GetParent(path);
                var item = new AdvancedDropdownItem(GetRawName(path));
                itemToChoice[item] = path;
                parent.AddChild(item);
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            onChoice?.Invoke(itemToChoice[item]);
        }

        private AdvancedDropdownItem GetParent(string path)
        {
            if (!path.Contains('/')) return parents[""];
            var parentPath = path.Substring(0, path.LastIndexOf('/'));
            if (parents.TryGetValue(parentPath, out var parent)) return parent;

            var parentName = GetRawName(parentPath);
            var grandparent = GetParent(parentPath);
            parent = new AdvancedDropdownItem(parentName);
            parents[parentPath] = parent;
            grandparent.AddChild(parent);
            return parent;
        }

        private string GetRawName(string path) => path.Contains('/')
            ? path.Substring(path.LastIndexOf('/') + 1)
            : path;
        
    }
}