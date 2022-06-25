using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Quaternion _rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
    }

    public static void PlayerShoot(int _fromClient, Packet _packet){
        Vector3 _shootDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.Shoot(_shootDirection);
    }

    public static void WeaponDamage(int _fromClient, Packet _packet){
        float _damage = _packet.ReadFloat();

        Server.clients[_fromClient].player.damage = _damage;
    }

    public static void MeleeAttack(int _fromClient, Packet _packet){
        Vector3 _pos = _packet.ReadVector3();
        Vector3 _scale = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.MeleeAttack(_pos, _scale, _rotation);
    }

    public static void PlayerThrowItem(int _fromClient, Packet _packet){
        Vector3 _throwDirection = _packet.ReadVector3();
        Server.clients[_fromClient].player.ThrowItem(_throwDirection);
    }

    public static void ClientDisconnect(int _fromClient, Packet _packet){
        int _idCase = _packet.ReadInt();

        Server.clients[_fromClient].Disconnect();
    }
}
