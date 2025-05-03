using System.Collections.Generic;
using UnityEngine;

namespace SKC.Trie
{
    public class Trie
    {
        private readonly TrieNode _root = new TrieNode();

        public void Insert(string word)
        {
            TrieNode current = _root;
            foreach (char c in word)
            {
                if (!current.Children.ContainsKey(c))
                {
                    current.Children[c] = new TrieNode();
                }
                current = current.Children[c];
            }
            current.IsEndOfWord = true;
        }

        public bool IsWord(string word)
        {
            TrieNode current = _root;
            foreach (char c in word)
            {
                if (!current.Children.ContainsKey(c))
                {
                    return false;
                }
                current = current.Children[c];
            }
            return current != null && current.IsEndOfWord;
        }

        public bool IsPrefix(string prefix)
        {
            TrieNode current = _root;
            foreach (char c in prefix)
            {
                if (!current.Children.ContainsKey(c))
                {
                    return false;
                }
                current = current.Children[c];
            }
            return current != null; // Any node means it's a prefix
        }
    }
}
