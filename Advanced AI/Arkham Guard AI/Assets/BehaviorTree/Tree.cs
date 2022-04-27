using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// BehaviorTree classes are taken from this tutorial https://youtu.be/aR6wt5BlE-E

namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node root = null;
        protected void Start()
        {
            root = SetupTree();
        }

        private void Update()
        {
            if (root != null)
            {
                root.Evaluate();
            }
        }
        protected abstract Node SetupTree();

    }

}
