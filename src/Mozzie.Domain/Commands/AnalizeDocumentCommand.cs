using System;
using System.Collections.Generic;
using System.Text;

namespace Mozzie.Domain.Commands
{
    public class AnalizeDocumentCommand
    {
        public byte[] Self { get; set; }
        public byte[] Front { get; set; }
        public byte[] Back { get; set; }
    }
}
