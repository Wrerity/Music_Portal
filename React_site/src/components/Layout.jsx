import { Link, useNavigate } from 'react-router-dom'
import { getToken, setToken } from '../api/client.js'

export default function Layout({ children }) {
  const nav = useNavigate()
  const authed = !!getToken()
  const logout = () => { setToken(null); nav('/login') }
  return (
    <>
      <header style={{background:'#1a1a2e', color:'#fff', padding:'12px 20px', display:'flex', gap:'16px', alignItems:'center', flexWrap:'wrap'}}>
        <Link to="/" style={{color:'#fff', fontWeight:700, fontSize:'18px', textDecoration:'none'}}>Музыкальный портал (React)</Link>
        <nav style={{display:'flex', gap:'12px', flexWrap:'wrap'}}>
          <Link to="/" style={linkStyle}>Каталог</Link>
          <Link to="/admin" style={linkStyle}>Админ SPA</Link>
        </nav>
        <div style={{marginLeft:'auto', display:'flex', gap:'8px', alignItems:'center'}}>
          {authed ? <button onClick={logout} style={btnStyle}>Выход</button> : <Link to="/login" style={btnStyleLink}>Вход</Link>}
          <span style={{opacity:0.7, fontSize:'14px'}}>API: https://localhost:7090</span>
        </div>
      </header>
      <main style={{maxWidth:'1200px', margin:'0 auto', padding:'20px'}}>{children}</main>
    </>
  )
}
const linkStyle = {color:'#eee', textDecoration:'none', fontSize:'14px', padding:'6px 10px', border:'1px solid #444', borderRadius:'6px'}
const btnStyle = {background:'#e94560', color:'#fff', border:'none', padding:'6px 12px', borderRadius:'6px', fontSize:'14px', cursor:'pointer'}
const btnStyleLink = {...btnStyle, textDecoration:'none', display:'inline-block'}
