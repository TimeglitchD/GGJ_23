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
            connectionObject.NodeMonoBehaviour = a;

            // Some visual elements
            SpriteRenderer sprite = connectionObject.Renderer;
            TextMesh text = connectionObject.Text;
            sprite.material = _materials[a.Node.GroupId % _materials.Length];

            Vector3 c = (b.transform.position - a.transform.position).normalized;
            Vector3 center = (b.transform.position + a.transform.position) / 2;
            Vector3 diff = b.transform.position - a.transform.position;

            int count = 1;
            foreach (var connection in a.Node.Connections)
                if (connection.Node == b.Node) count++;


            Vector3 offset = (b.Node.GetConnectionIndex(a.Node) == 1) ? new Vector3(0.0f, .5f, 0.0f) : new Vector3(0.0f, -.5f, 0.0f);

            connectionObject.transform.position = center;
            text.transform.position = center - .1f * count * diff + new Vector3(0.0f, .0f, -2.0f);

            float dist = Vector3.Distance(b.transform.position, a.transform.position);
            sprite.transform.localScale = new Vector3(dist * .5f, .1f, 1.0f);
            sprite.gameObject.transform.rotation = Quaternion.LookRotation(c, Vector3.forward) * Quaternion.Euler(-90, 0, 90);
            sprite.transform.localPosition += new Vector3(0.0f, .5f, -1.0f);

            text.text = connectionObject.Connection.DecayValue.ToString();
            _connections.Add(connectionObject);

            foreach (var item in a.Node.Connections)
                item.Node.CanInteract = true;
            b.ConnectToNetwork();
        }

        public void DestroyConnection(ConnectionMonobehaviour connectionBehaviour)
        {
            Node other = connectionBehaviour.Connection.Node;

            // Remove it from this side
            if (connectionBehaviour.NodeMonoBehaviour.Node.RemoveConnection(connectionBehaviour.Connection) == 1)
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

        public void UndoDecayToAll()
        {

        }

        public void Restart()
        {
            for (int i = _connections.Count - 1; i >= 0; i--)
            {
                _connections[i].NodeMonoBehaviour.Restart();
                Destroy(_connections[i].gameObject);
                _connections.RemoveAt(i);
            }
        }
    }
}
