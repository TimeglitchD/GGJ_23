using System;
using System.Collections.Generic;
using UnityEngine;

namespace Synapse.Core
{
    public class ConnectionLifetimeManager : MonoBehaviour, IRestartable
    {
        private List<ConnectionMonobehaviour> _connections = new List<ConnectionMonobehaviour>();
        [SerializeField] private ConnectionMonobehaviour prefab;
        [SerializeField] private Material[] _materials;

        [SerializeField] Vector3 offset;

        public void SpawnConnection(NodeMonoBehaviour a, NodeMonoBehaviour b)
        {
            // Setup connection monobehaviour
            ConnectionMonobehaviour connectionObject = Instantiate(prefab);
            connectionObject.Connection = a.Node.Connections[a.Node.Connections.Count - 1];
            connectionObject.Node = a.Node;

            // Some visual elements
            SpriteRenderer sprite = connectionObject.Renderer;
            TextMesh text = connectionObject.Text;
            sprite.material = _materials[a.Node.GroupId % _materials.Length];

            Vector3 c = (b.transform.position - a.transform.position).normalized;
           
            Vector3 center = (a.transform.position + b.transform.position) / 2.0f + offset;
            connectionObject.transform.position = center;

            float dist = Vector3.Distance(b.transform.position, a.transform.position);
            sprite.transform.localScale = new Vector3(dist * .5f, .1f, 1.0f);
            sprite.transform.localPosition += new Vector3(0.0f, .5f, 0.0f);
            sprite.gameObject.transform.rotation = Quaternion.LookRotation(c, Vector3.forward) * Quaternion.Euler(-90, 0, 90);

            text.text = connectionObject.Connection.DecayValue.ToString();
            _connections.Add(connectionObject);

            foreach (var item in a.Node.Connections)
                item.Node.CanInteract = true;

            Debug.Log($"Added: {connectionObject.name}");
        }

        public void DestroyConnection(ConnectionMonobehaviour connectionBehaviour)
        {
            Node other = connectionBehaviour.Connection.Node;

            // Remove it from this side
            if (connectionBehaviour.Node.RemoveConnection(connectionBehaviour.Connection) == 1)
                Debug.Log("Removed.");
           

            Destroy(connectionBehaviour.gameObject);
            _connections.Remove(connectionBehaviour);
        }

        public void UpdateConnection(ConnectionMonobehaviour connectionBehaviour)
        {
            connectionBehaviour.Text.text = connectionBehaviour.Connection.DecayValue.ToString();
        }

        public void ApplyDecayToAll()
        {
            for (int i = _connections.Count - 1; i >= 0; i--)
                if (_connections[i].Connection.Decay())
                    DestroyConnection(_connections[i]);
                else
                    UpdateConnection(_connections[i]);
        }

        public void Restart()
        {
            for (int i = _connections.Count - 1; i > 0; i--)
            {
                Destroy(_connections[i].gameObject);
                _connections.RemoveAt(i);
            }
        }
    }
}
