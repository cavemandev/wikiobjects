using System;
using System.Collections.Generic;
using System.Text;

namespace WikiObjects.Data.Model
{
    public class Attachment : ACLObject
    {
        public Attachment(User owner) : base (owner) { }

        public string name { get; set; }

        public string parentId { get; set; }
    }
}
