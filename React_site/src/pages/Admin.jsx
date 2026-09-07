import { useEffect, useState } from 'react'
import api from '../api/client.js'

export default function Admin(){
  const [tab,setTab]=useState('users')
  const [users,setUsers]=useState([])
  const [pending,setPending]=useState([])
  const [genres,setGenres]=useState([])
  const [authors,setAuthors]=useState([])
  const [songs,setSongs]=useState([])
  const [form,setForm]=useState({name:'',description:'',country:''})
  const [editId,setEditId]=useState(null)

  const loadUsers=async()=>{ const {data}=await api.get('/api/users?page=1&pageSize=20'); setUsers(data.users||data.Users||[]) }
  const loadPending=async()=>{ const {data}=await api.get('/api/users/pending'); setPending(data) }
  const loadGenres=async()=>{ const {data}=await api.get('/api/genres'); setGenres(data) }
  const loadAuthors=async()=>{ const {data}=await api.get('/api/authors'); setAuthors(data) }
  const loadSongs=async()=>{ const {data}=await api.get('/api/admin/songs?page=1&pageSize=20'); setSongs(data.songs||data.Songs||[]) }

  useEffect(()=>{ loadUsers(); loadPending(); loadGenres(); loadAuthors(); loadSongs() },[tab])

  const createGenre=async()=>{ await api.post('/api/genres', {name:form.name, description:form.description}); setForm({name:'',description:''}); loadGenres() }
  const updateGenre=async()=>{ if(!editId) return; await api.put(`/api/genres/${editId}`, {id:editId, name:form.name, description:form.description}); setEditId(null); setForm({name:'',description:''}); loadGenres() }
  const deleteGenre=async(id)=>{ if(!confirm('DELETE genre '+id)) return; await api.delete(`/api/genres/${id}`); loadGenres() }

  const createAuthor=async()=>{ await api.post('/api/authors', {name:form.name, country:form.country, description:form.description}); setForm({name:'',description:'',country:''}); loadAuthors() }
  const deleteAuthor=async(id)=>{ await api.delete(`/api/authors/${id}`); loadAuthors() }

  const approve=async(id)=>{ await api.post(`/api/users/${id}/activate`); loadPending(); loadUsers() }
  const reject=async(id)=>{ await api.post(`/api/users/${id}/reject`); loadPending() }
  const deleteUser=async(id)=>{ await api.delete(`/api/users/${id}`); loadUsers() }

  const deleteSong=async(id)=>{ await api.delete(`/api/admin/songs/${id}`); loadSongs() }

  return (
    <div>
      <h2 style={{fontSize:'20px'}}>Админ SPA — AJAX GET/POST/PUT/DELETE к Web API (разные домены)</h2>
      <div style={{display:'flex', gap:'8px', marginBottom:'12px', flexWrap:'wrap'}}>
        {['users','pending','genres','authors','songs'].map(t=>(
          <button key={t} onClick={()=>setTab(t)} style={{padding:'8px 12px', fontSize:'14px', border:'1px solid #ccc', borderRadius:'6px', background:tab===t?'#e94560':'#fff', color:tab===t?'#fff':'#333', cursor:'pointer'}}>{t}</button>
        ))}
        <span style={{fontSize:'14px', marginLeft:'auto'}}>CORS: {window.location.origin} → https://localhost:7090</span>
      </div>

      {tab==='users' && <div><h3 style={h3}>Users GET</h3><button onClick={loadUsers} style={btn}>Обновить GET</button><table style={table}><thead><tr><th style={th}>ID</th><th style={th}>Username</th><th style={th}>Role</th><th style={th}>Действия (PUT/DELETE)</th></tr></thead><tbody>{users.map(u=><tr key={u.id||u.Id}><td style={td}>{u.id||u.Id}</td><td style={td}>{u.username||u.Username}</td><td style={td}>{u.role||u.Role}</td><td style={td}><button onClick={()=>deleteUser(u.id||u.Id)} style={btnDel}>DELETE</button></td></tr>)}</tbody></table></div>}

      {tab==='pending' && <div><h3 style={h3}>Pending GET</h3><table style={table}><tbody>{pending.map(u=><tr key={u.id||u.Id}><td style={td}>{u.username||u.Username}</td><td style={td}><button onClick={()=>approve(u.id||u.Id)} style={btn}>POST activate</button> <button onClick={()=>reject(u.id||u.Id)} style={btnDel}>POST reject</button></td></tr>)}</tbody></table></div>}

      {tab==='genres' && <div><h3 style={h3}>Genres GET/POST/PUT/DELETE</h3>
        <div style={formRow}><input placeholder="Name" value={form.name} onChange={e=>setForm({...form,name:e.target.value})} style={input} /><input placeholder="Desc" value={form.description} onChange={e=>setForm({...form,description:e.target.value})} style={input} /><button onClick={editId?updateGenre:createGenre} style={btn}>{editId?'PUT':'POST'}</button></div>
        <table style={table}><tbody>{genres.map(g=><tr key={g.id||g.Id}><td style={td}>{g.id||g.Id}</td><td style={td}>{g.name||g.Name}</td><td style={td}><button onClick={()=>{setEditId(g.id||g.Id); setForm({name:g.name||g.Name, description:g.description||g.Description})}} style={btn}>Edit</button> <button onClick={()=>deleteGenre(g.id||g.Id)} style={btnDel}>DELETE</button></td></tr>)}</tbody></table></div>}

      {tab==='authors' && <div><h3 style={h3}>Authors GET/POST/PUT/DELETE</h3>
        <div style={formRow}><input placeholder="Name" value={form.name} onChange={e=>setForm({...form,name:e.target.value})} style={input} /><input placeholder="Country" value={form.country} onChange={e=>setForm({...form,country:e.target.value})} style={input} /><button onClick={createAuthor} style={btn}>POST</button></div>
        <table style={table}><tbody>{authors.map(a=><tr key={a.id||a.Id}><td style={td}>{a.name||a.Name}</td><td style={td}>{a.country||a.Country}</td><td style={td}><button onClick={()=>deleteAuthor(a.id||a.Id)} style={btnDel}>DELETE</button></td></tr>)}</tbody></table></div>}

      {tab==='songs' && <div><h3 style={h3}>Songs GET/DELETE (admin)</h3><table style={table}><tbody>{songs.map(s=><tr key={s.id||s.Id}><td style={td}>{s.title||s.Title}</td><td style={td}>{s.authors||s.Authors}</td><td style={td}><button onClick={()=>deleteSong(s.id||s.Id)} style={btnDel}>DELETE</button></td></tr>)}</tbody></table></div>}
    </div>
  )
}
const h3={fontSize:'16px', margin:'12px 0 8px'}
const btn={background:'#e94560', color:'#fff', border:'none', padding:'6px 10px', borderRadius:'6px', fontSize:'14px', cursor:'pointer'}
const btnDel={background:'#fff', border:'1px solid #c00', color:'#c00', padding:'6px 10px', borderRadius:'6px', fontSize:'14px', cursor:'pointer'}
const table={width:'100%', borderCollapse:'collapse', marginTop:'8px'}
const th={border:'1px solid #ddd', padding:'8px', fontSize:'14px', background:'#f5f5f5', textAlign:'left'}
const td={border:'1px solid #ddd', padding:'8px', fontSize:'14px'}
const input={padding:'8px', fontSize:'14px', border:'1px solid #ccc', borderRadius:'6px', flex:1}
const formRow={display:'flex', gap:'8px', marginBottom:'8px', flexWrap:'wrap'}
