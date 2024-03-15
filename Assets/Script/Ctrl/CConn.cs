using System;
using System.Collections.Generic;
using Cherry;
using Cherry.Ctrl;
using Cherry.Misc;
using Google.Protobuf;
using Pb;
using UnityEngine;
using UnityWebSocket;

namespace Script.Ctrl
{
    public class CConn : CtrlBase
    {
        private class Req
        {
            public Action<ushort> ResFail;
            public Action<IMessage> ResOk;
            public ushort Code;
            public float ReqTime;
        }
        private WebSocket _socket;
        private uint _seqId;
        private readonly Dictionary<uint, Req> _seqIdToReq = new();
        private readonly Dictionary<ushort, Action<IMessage>> _ntcListener = new();
        private readonly Dictionary<object, Action<IMessage>> _handlerToListener = new();
        public override void Initialize(Action onComplete = null)
        {
            
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _socket?.CloseAsync();
        }

        public void Conn()
        {
            if (_socket != null) return;
            // var addr = "ws://1.117.224.225:7737";
            var addr = "ws://192.168.3.48:7737";
            _socket = new WebSocket(addr);
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            _socket.OnError += OnError;
            _socket.ConnectAsync();
        }

        public void Request<T>(IMessage message, Action<ushort>resFail, Action<T> resOk) where T : IMessage
        {
            if (_socket == null) return;
            Game.Log.Debug($"request {message.GetType()}: {message}");
            var msgCode = Code.MsgTypeToCode[message.GetType()];
            var payload = message.ToByteArray();
            var buff = new Bytes(6 + payload.Length);
            buff.Write(msgCode);
            _seqId++;
            _seqIdToReq[_seqId] = new Req
            {
                ResFail = resFail,
                ResOk = msg=>resOk((T)msg),
                Code = msgCode,
                ReqTime = Time.time,
            };
            buff.Write(_seqId);
            buff.Write(payload);
            _socket.SendAsync(buff.All());
        }

        private void OnOpen(object sender, OpenEventArgs e)
        {
            Game.Log.Debug("on websocket open");
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            Game.Log.Debug("on websocket close");
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            var bytes = new Bytes(e.RawData);
            var type = bytes.RByte();
            switch (type)
            {
                case 0:
                {
                    var seqId = bytes.RUInt32();
                    if (_seqIdToReq.TryGetValue(seqId, out var req))
                    {
                        var payload = bytes.RAvailable();
                        var resCode = Code.ReqToRes[req.Code];
                        var msg = Code.CodeToMsg[resCode](payload);
                        var dur = Mathf.RoundToInt((Time.time - req.ReqTime) * 1000);
                        Game.Log.Debug($"{dur}ms res ok {msg.GetType()}: {msg}");
                        req.ResOk?.Invoke(msg);
                        _seqIdToReq.Remove(seqId);
                    }
                    break;
                }
                case 1:
                {
                    var seqId = bytes.RUInt32();
                    var resCode = bytes.RUInt16();
                    if (_seqIdToReq.TryGetValue(seqId, out var req))
                    {
                        Game.Log.Debug($"res fail: {resCode}");
                        req.ResFail?.Invoke(resCode);
                        _seqIdToReq.Remove(seqId);
                    }
                    break;
                }
                case 2:
                {
                    var code = bytes.RUInt16();
                    if (_ntcListener.TryGetValue(code, out var action))
                    {
                        var payload = bytes.RAvailable();
                        var msg = Code.CodeToMsg[code](payload);
                        if (!msg.GetType().Name.EndsWith("SceneEventNtc"))
                        {
                            Game.Log.Debug($"ntc {msg.GetType()}: {msg}");
                        }
                        action(msg);
                    }
                    else
                    {
                        Game.Log.Warn($"not listen notice: {code}");
                    }
                    break;
                }
            }
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Game.Log.Debug($"on websocket error: {e.Message}");
        }

        public void BindPus<T>(Action<T> handler) where T : IMessage
        {
            var ntc = typeof(T);
            if (!Code.MsgTypeToCode.TryGetValue(ntc, out var code))
            {
                throw new ArgumentException($"not exist type: {ntc}");
            }

            Action<IMessage> listener = msg =>
            {
                handler((T)msg);
            };
            _handlerToListener.Add(handler, listener);
            _ntcListener.Add(code, listener);
        }

        public void UnbindNtc<T>() where T : IMessage
        {
            var ntc = typeof(T);
            if (!Code.MsgTypeToCode.TryGetValue(ntc, out var code))
            {
                throw new ArgumentException($"not exist type: {ntc}");
            }
            _ntcListener.Remove(code);
        }
    }
}