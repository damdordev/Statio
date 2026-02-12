using System;
using System.Collections.Generic;
using UnityEngine;

namespace Damdor.VisualStates
{
    [Serializable]
    public class VisualStatesList
    {
        public List<string> States => states;
        
        [SerializeField] private List<string> states;
    }
}