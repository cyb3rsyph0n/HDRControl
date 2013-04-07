using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDRControl
{
    public class ListItem<p,t>
    {
        private p _Text;
        private t _Value;

        public ListItem(p text, t value)
        {
            _Text = text;
            _Value = value;
        }

        public p Text
        {
            get { return _Text; }
        }

        public t Value
        {
            get { return _Value; }
        }

        public override string ToString()
        {
            return Text.ToString();
        }
    }
}
