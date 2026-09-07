import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import api, { setToken } from '../api/client.js'

export default function Login() {
  const nav = useNavigate()
  const [u, setU] = useState('admin')
  const [p, setP] = useState('admin123')
  const [err, setErr] = useState('')
  const [ok, setOk] = useState('')

  const submit = async e => {
    e.preventDefault()
    setErr(''); setOk('')
    try {
      const { data } = await api.post('/api/auth/login', { username: u, password: p })
      const token = data.token || data.Token
      setToken(token)
      setOk('Вход OK, токен сохранён')
      setTimeout(()=> nav('/'), 800)
    } catch (e) {
      setErr(e.message || 'Ошибка входа')
    }
  }

  const register = async () => {
    setErr(''); setOk('')
    try {
      await api.post('/api/auth/register', { username: u, password: p })
      setOk('Регистрация OK, дождитесь подтверждения админом')
    } catch (e) { setErr(e.message) }
  }

  return (
    <div style={{maxWidth:'420px', margin:'40px auto'}}>
      <h2 style={h2}>Вход (POST /api/auth/login)</h2>
      <form onSubmit={submit} style={form}>
        <label style={label}>Имя пользователя</label>
        <input value={u} onChange={e=>setU(e.target.value)} style={input} />
        <label style={label}>Пароль</label>
        <input value={p} onChange={e=>setP(e.target.value)} type="password" style={input} />
        {err && <div style={{color:'#c00', fontSize:'14px'}}>{err}</div>}
        {ok && <div style={{color:'#0a0', fontSize:'14px'}}>{ok}</div>}
        <div style={{display:'flex', gap:'8px', marginTop:'12px'}}>
          <button type="submit" style={btn}>Войти (POST)</button>
          <button type="button" onClick={register} style={btn2}>Регистрация (POST)</button>
        </div>
      </form>
      <p style={{fontSize:'14px', marginTop:'12px'}}>Демо: admin/admin123, demo/demo123 — <Link to="/">Каталог</Link></p>
    </div>
  )
}
const h2={fontSize:'22px', marginBottom:'12px'}
const form={display:'flex', flexDirection:'column', gap:'8px', background:'#f5f5f5', padding:'16px', borderRadius:'8px'}
const label={fontSize:'14px', fontWeight:600}
const input={padding:'8px', fontSize:'14px', border:'1px solid #ccc', borderRadius:'6px'}
const btn={background:'#e94560', color:'#fff', border:'none', padding:'8px 14px', borderRadius:'6px', fontSize:'14px', cursor:'pointer', flex:1}
const btn2={background:'#fff', color:'#333', border:'1px solid #ccc', padding:'8px 14px', borderRadius:'6px', fontSize:'14px', cursor:'pointer', flex:1}
