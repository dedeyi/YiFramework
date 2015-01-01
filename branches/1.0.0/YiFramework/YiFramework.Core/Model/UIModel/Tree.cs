using System.Collections.Generic;

namespace YiFramework.Core
{
    public class Tree
    {
        public string id { get; set; }
        public string text { get; set; }
        public IEnumerable<Tree> children { get; set; }
    }
}
