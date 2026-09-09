import { useEffect, useState, useRef } from 'react'
import * as signalR from '@microsoft/signalr'
import { getToken } from '../api/client.js'

export default function Chat(){
  const [conn, setConn] = useState(null)
  const [username, setUsername] = useState(localStorage.getItem('chatUsername') || 'Guest')
  const [users, setUsers] = useState([])
  const [messages, setMessages] = useState([])
  const [priv, setPriv] = useState([])
  const [text, setText] = useState('')
  const [target, setTarget] = useState('') // connectionId для личного
  const [tab, setTab] = useState('all')
  const endRef = useRef(null)

  useEffect(()=>{ endRef.current?.scrollIntoView({behavior:'smooth'}) },[messages,priv])

  const connect = async () => {
    localStorage.setItem('chatUsername', username)
    const c = new signalR.HubConnectionBuilder()
      .withUrl(`http://localhost:5090/hubs/chat?username=${encodeURIComponent(username)}`, {
        accessTokenFactory: () => getToken() || ''
      })
      .withAutomaticReconnect()
      .build()
    c.on('LoadHistory', msgs => setMessages(msgs))
    c.on('ReceiveMessage', msg => setMessages(m => [...m, msg]))
    c.on('ReceivePrivateMessage', msg => setPriv(m => [...m, msg]))
    c.on('UpdateUsers', list => setUsers(list))
    await c.start()
    setConn(c)
  }

  const sendAll = async () => {
    if(!text.trim()||!conn) return
    if(target) await conn.invoke('SendPrivateMessage', target, text)
    else await conn.invoke('SendMessage', text)
    setText('')
  }

  const disconnect = async () => { await conn?.stop(); setConn(null) }

  return (
    <div style={{maxWidth:'900px', margin:'0 auto'}}>
      <h2 style={{fontSize:'20px'}}>Чат — общая + личные (SignalR + БД)</h2>
      {!conn ? (
        <div style={box}>
          <label style={label}>Имя</label><input value={username} onChange={e=>setUsername(e.target.value)} style={input} />
          <button onClick={connect} style={btn}>Подключиться (OnConnectedAsync → LoadHistory)</button>
          <p style={{fontSize:'14px', opacity:0.7}}>Сохранение в БД: ChatUsers/ChatMessages, ConnectionId обновляется</p>
        </div>
      ) : (
        <>
          <div style={{display:'flex', gap:'8px', marginBottom:'8px'}}>
            <button onClick={disconnect} style={btn2}>Отключиться</button>
            <span style={{fontSize:'14px'}}>Вы: {username} ({conn.connectionId?.slice(0,5)}) | Онлайн: {users.length}</span>
          </div>
          <div style={{display:'flex', gap:'12px'}}>
            <div style={{flex:1, border:'1px solid #ddd', borderRadius:'8px', padding:'8px', height:'400px', display:'flex', flexDirection:'column'}}>
              <div style={{display:'flex', gap:'6px', marginBottom:'8px'}}>
                <button onClick={()=>setTab('all')} style={{...tabBtn, background:tab==='all'?'#e94560':'#fff', color:tab==='all'?'#fff':'#333'}}>Общий чат</button>
                <button onClick={()=>setTab('priv')} style={{...tabBtn, background:tab==='priv'?'#e94560':'#fff', color:tab==='priv'?'#fff':'#333'}}>Личные</button>
              </div>
              <div style={{flex:1, overflowY:'auto', border:'1px solid #eee', padding:'8px', borderRadius:'6px'}}>
                {(tab==='all'?messages:priv).map(m=>(
                  <div key={m.id} style={{marginBottom:'6px', background:m.isPrivate?'#fff3cd':'#f5f5f5', padding:'6px', borderRadius:'6px'}}>
                    <b style={{fontSize:'14px'}}>{m.senderName}</b>{m.isPrivate && <span style={{fontSize:'12px', color:'#c00'}}> → {m.receiverName} (Личное)</span>}: <span style={{fontSize:'14px'}}>{m.content}</span>
                    <span style={{fontSize:'11px', opacity:0.6, marginLeft:'6px'}}>{new Date(m.timestamp).toLocaleTimeString()}</span>
                  </div>
                ))}
                <div ref={endRef} />
              </div>
              <div style={{display:'flex', gap:'6px', marginTop:'8px'}}>
                <input value={text} onChange={e=>setText(e.target.value)} onKeyDown={e=>e.key==='Enter'&&sendAll()} placeholder={target?'Личное сообщение...':'Общее сообщение...'} style={{...input, flex:1}} />
                <button onClick={sendAll} style={btn}>Отправить</button>
              </div>
            </div>
            <div style={{width:'200px', border:'1px solid #ddd', borderRadius:'8px', padding:'8px'}}>
              <div style={{fontSize:'14px', fontWeight:700, marginBottom:'6px'}}>Активные ({users.length}) — клик для лички</div>
              {users.map(u=>(
                <div key={u.id} onClick={()=>setTarget(u.connectionId)} style={{padding:'6px', fontSize:'14px', cursor:'pointer', background:target===u.connectionId?'#e94560':'#fff', color:target===u.connectionId?'#fff':'#333', borderRadius:'4px', marginBottom:'4px', border:'1px solid #eee'}}>
                  {u.username} {u.isOnline?'●':''}
                </div>
              ))}
              {target && <button onClick={()=>setTarget('')} style={{...btn2, width:'100%', marginTop:'6px'}}>Сбросить личку</button>}
              <p style={{fontSize:'12px', opacity:0.6, marginTop:'8px'}}>Выбран ConnectionId: {target || '— общий чат'}</p>
            </div>
          </div>
        </>
      )}
    </div>
  )
}
const box={background:'#f5f5f5', padding:'16px', borderRadius:'8px', display:'flex', flexDirection:'column', gap:'8px', maxWidth:'400px'}
const label={fontSize:'14px', fontWeight:600}
const input={padding:'8px', fontSize:'14px', border:'1px solid #ccc', borderRadius:'6px'}
const btn={background:'#e94560', color:'#fff', border:'none', padding:'8px 12px', borderRadius:'6px', fontSize:'14px', cursor:'pointer'}
const btn2={background:'#fff', border:'1px solid #ccc', padding:'8px 12px', borderRadius:'6px', fontSize:'14px', cursor:'pointer'}
const tabBtn={padding:'6px 10px', fontSize:'14px', border:'1px solid #ccc', borderRadius:'6px', cursor:'pointer'}
