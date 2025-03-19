using UnityEngine;
using System.Collections;

namespace Utilities.Inspector
{
    public class SeparatorAttribute : PropertyAttribute
    {
        public readonly string title;

        public SeparatorAttribute()
        {
            this.title = "";
        }

        public SeparatorAttribute(string _title)
        {
            this.title = _title;
        }
    }
}