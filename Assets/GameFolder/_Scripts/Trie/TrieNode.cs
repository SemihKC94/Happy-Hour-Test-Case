using System.Collections.Generic;

namespace SKC.Trie
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; private set; } = new Dictionary<char, TrieNode>();
        public bool IsEndOfWord { get; set; }
    }
}
