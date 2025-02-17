﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Tagging;

namespace InlineLocals
{
    class LocalInfo
    {
        internal LocalInfo(string value, string type) {
            Value = value;
            Type = type;
        }

        internal string Value;
        internal string Type;
    }
    class WatchTag : ITag
    {
        /// <summary>
        /// Data tag indicating that the tagged text is currently a local in the stack frame.
        /// </summary>
        /// <remarks>
        /// Note that this tag has nothing directly to do with adornments or other UI.
        /// This project's adornments will be produced based on the data provided in these tags.
        /// In this case, we will display the value of variables in the stack frame inside the code.
        /// This separation provides the potential for other extensions to consume watch tags
        /// and provide alternative UI or other derived functionality over this data.
        /// </remarks>
        internal WatchTag(Dictionary<string, LocalInfo> locals, int offset) {
            Locals = locals;
            Offset = offset;
        }

        internal readonly Dictionary<string, LocalInfo> Locals;
        internal readonly int Offset;
    }
}
