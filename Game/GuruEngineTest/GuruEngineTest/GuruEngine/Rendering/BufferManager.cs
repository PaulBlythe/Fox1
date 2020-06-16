using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using GuruEngine.Maths;
using GuruEngine.DebugHelpers;

namespace GuruEngine.Rendering
{
    public class BufferManager
    {
        public static BufferManager Instance;

        Dictionary<int, VertexBuffer> vertexBuffers = new Dictionary<int, VertexBuffer>();
        Dictionary<int, IndexBuffer> indexBuffers = new Dictionary<int, IndexBuffer>();
        Dictionary<int, int> vertexReferences = new Dictionary<int, int>();
        Dictionary<int, int> indexReferences = new Dictionary<int, int>();


        public BufferManager()
        {
            Instance = this;
        }

        public int AddVertexBuffer(String name, VertexBuffer buffer)
        {
            int hash = name.GetHashCode();
            if (vertexBuffers.ContainsKey(hash))
            {
                vertexReferences[hash]++;
            }
            else
            {
                vertexReferences.Add(hash, 1);
                vertexBuffers.Add(hash, buffer);
            }
            return hash;
        }

        public int AddIndexBuffer(String name, IndexBuffer buffer)
        {
            int hash = name.GetHashCode();
            if (indexBuffers.ContainsKey(hash))
            {
                indexReferences[hash]++;
            }
            else
            {
                indexBuffers.Add(hash, buffer);
                indexReferences.Add(hash, 1);
            }
            return hash;
        }

        public VertexBuffer GetVertexBuffer(int hash)
        {
            if (vertexBuffers.ContainsKey(hash))
                return vertexBuffers[hash];
            LogHelper.Instance.Fatal("Request for unknown VertexBuffer with hash" + hash.ToString());
            return null;
        }

        public IndexBuffer GetIndexBuffer(int hash)
        {
            if (indexBuffers.ContainsKey(hash))
                return indexBuffers[hash];
            LogHelper.Instance.Fatal("Request for unknown IndexBuffer with hash" + hash.ToString());
            return null;
        }

        public void RemoveVertexBuffer(int hash)
        {
            vertexReferences[hash]--;
            if (vertexReferences[hash] == 0)
            {
                vertexReferences.Remove(hash);
                vertexBuffers.Remove(hash);
            }
        }
        public void RemoveIndexBuffer(int hash)
        {
            indexReferences[hash]--;
            if (indexReferences[hash] == 0)
            {
                indexReferences.Remove(hash);
                indexBuffers.Remove(hash);
            }
        }

        #region Static methods
        public static int AddNamedVertexBuffer(String name, VertexBuffer buffer)
        {
            return Instance.AddVertexBuffer(name, buffer);
        }
        public static int AddNamedIndexBuffer(String name, IndexBuffer buffer)
        {
            return Instance.AddIndexBuffer(name, buffer);
        }
        public static VertexBuffer GetNamedVertexBuffer(int hash)
        {
            return Instance.GetVertexBuffer(hash);
        }
        public static IndexBuffer GetNamedIndexBuffer(int hash)
        {
            return Instance.GetIndexBuffer(hash);
        }
        public static VertexBuffer GetNamedVertexBuffer(String name)
        {
            int hash = name.GetHashCode(); 
            return Instance.GetVertexBuffer(hash);
        }
        public static IndexBuffer GetNamedIndexBuffer(String name)
        {
            int hash = name.GetHashCode();
            return Instance.GetIndexBuffer(hash);
        }
        public static void RemoveNamedVertexBuffer(int hash)
        {
            Instance.RemoveVertexBuffer(hash);
        }
        public static void RemoveNamedIndexBuffer(int hash)
        {
            Instance.RemoveIndexBuffer(hash);
        }
        public static void RemoveNamedVertexBuffer(String name)
        {
            int hash = name.GetHashCode();
            Instance.RemoveVertexBuffer(hash);
        }
        public static void RemoveNamedIndexBuffer(String name)
        {
            int hash = name.GetHashCode();
            Instance.RemoveIndexBuffer(hash);
        }
        #endregion

    }
}
