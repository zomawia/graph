using System;
using System.Collections.Generic;
using System.Text;

namespace GraphProject
{
    class Solver<T>
    {
        class Meta
        {
            public enum VisitState { undiscovered, frontier, explored };
            public VisitState state;

            public uint depth;
            public float gScore;
            public float heur;
            public float fScore
            {
                get
                {
                    return gScore + heur;
                }
            }

            // When we add a node to the frontier
            // We will set it's previous to whatever the current node is.
            public Graph<T>.Node prev;
            public Meta() {
                gScore = 0;
                depth = 0;
                heur = 0;
                state = VisitState.undiscovered;
            }
        };

        public Graph<T> graph;

        private Meta[] metadata;
        private List<Graph<T>.Node> frontier;

        private T start, goal;
        private float threshold;
        private Graph<T>.FindDelegate searcher;
        private Graph<T>.Node goalNode;

        public List<T> solution
        {
            get
            {
                List<T> retval = new List<T>();
                retval.Add(goal);
                var n = goalNode;
                while (n != null)
                {
                    retval.Add(n.data);
                    n = metadata[n.uid].prev;
                }
                retval.Add(start);
                retval.Reverse();
                return retval;
            }
        }

        // Cleanup, setup and start our search.
        public void init(T a_start, T a_goal, Graph<T>.FindDelegate a_searcher, float a_search_threshold = 0.0001f)
        {
            start = a_start;
            goal = a_goal;
            searcher = a_searcher;
            threshold = a_search_threshold;
            goalNode = graph.FindNode(goal, searcher, threshold);

            metadata = new Meta[graph.nodes.Count];
            frontier = new List<Graph<T>.Node>();

            var snode = graph.FindNode(start, searcher, threshold);
            for (int i = 0; i < metadata.Length; ++i)
                metadata[i] = new Meta();

            metadata[snode.uid].state = Meta.VisitState.frontier;
            frontier.Add(snode);
        }

        private int dijkstra(Graph<T>.Node a, Graph<T>.Node b)
        {
            int res = 0;
            if (metadata[a.uid].gScore < metadata[b.uid].gScore)
                res = -1;
            if (metadata[a.uid].gScore > metadata[b.uid].gScore)
                res = 1;
            return res;
        }

        private int aStar(Graph<T>.Node a, Graph<T>.Node b)
        {
            int res = 0;
            if (metadata[a.uid].fScore < metadata[b.uid].fScore)
                res = -1;
            if (metadata[a.uid].fScore > metadata[b.uid].fScore)
                res = 1;
            return res;
        }


        public bool step()
        {
            frontier.Sort(aStar);
            var current = frontier[0];
            frontier.RemoveAt(0);

            metadata[current.uid].state = Meta.VisitState.explored;

            // stop if we've reached the goal.
            if (current.uid == goalNode.uid)
            {
                return false;
            }

            foreach (var e in current.edges)
            {
                float g = e.weight + metadata[current.uid].gScore;
                uint d = 1 + metadata[current.uid].depth;

                if (metadata[e.end.uid].state == Meta.VisitState.undiscovered)
                {                    
                    frontier.Add(e.end);
                    metadata[e.end.uid].state = Meta.VisitState.frontier;
                    // determine the h score
                    metadata[e.end.uid].heur = searcher(e.end.data, goalNode.data);
                }

                if (metadata[e.end.uid].state == Meta.VisitState.frontier)
                {
                    if (g < metadata[e.end.uid].gScore ||
                    metadata[e.end.uid].prev == null)
                    {
                        metadata[e.end.uid].prev = current;
                        metadata[e.end.uid].gScore = g;
                        metadata[e.end.uid].depth = d;
                    }
                }
            }
            return frontier.Count != 0;
        }
    }
}