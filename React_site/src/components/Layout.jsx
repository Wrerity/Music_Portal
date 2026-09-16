import { Link, useNavigate } from 'react-router-dom'
import { getToken, setToken } from '../api/client.js'

export default function Layout({ children }) {
  const nav = useNavigate()
  const authed = !!getToken()
  const logout = () => { setToken(null); nav('/login') }
  return (
    <>
      <header className="navbar-custom" style={{padding:'12px 20px', display:'flex', gap:'16px', alignItems:'center', flexWrap:'wrap'}}>
        <Link to="/" className="navbar-brand" style={{fontWeight:700, fontSize:'18px', textDecoration:'none'}}>Музыкальный портал (React)</Link>
        <nav style={{display:'flex', gap:'12px', flexWrap:'wrap'}}>
          <Link to="/" className="nav-link" style={{...linkStyle, background:'transparent', border:'none'}}>Каталог</Link>
          <Link to="/admin" className="nav-link" style={{...linkStyle, background:'transparent', border:'none'}}>Админ SPA</Link>
        </nav>
        <div style={{marginLeft:'auto', display:'flex', gap:'8px', alignItems:'center'}}>
          {authed ? <button onClick={logout} style={btnStyle}>Выход</button> : <Link to="/login" style={btnStyleLink}>Вход</Link>}
        </div>
      </header>
      <main style={{maxWidth:'1200px', margin:'0 auto', padding:'20px'}}>{children}</main>
    </>
  )
}
const linkStyle = {color:'#000', textDecoration:'none', fontSize:'14px', padding:'6px 10px', border:'1px solid var(--accent)', borderRadius:'6px', background:'transparent'}
const btnStyle = {background:'var(--accent)', color:'#fff', border:'1px solid var(--accent)', padding:'6px 12px', borderRadius:'6px', fontSize:'14px', cursor:'pointer'}
const btnStyleLink = {...btnStyle, textDecoration:'none', display:'inline-block'}
