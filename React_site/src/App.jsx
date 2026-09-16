import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Layout from './components/Layout.jsx'
import Catalog from './pages/Catalog.jsx'
import Login from './pages/Login.jsx'
import Admin from './pages/Admin.jsx'

export default function App(){
  return (
    <BrowserRouter>
      <Layout>
        <Routes>
          <Route path="/" element={<Catalog/>} />
          <Route path="/login" element={<Login/>} />
          <Route path="/admin" element={<Admin/>} />
          <Route path="*" element={<div style={{fontSize:'14px'}}>404 — страница не найдена</div>} />
        </Routes>
      </Layout>
    </BrowserRouter>
  )
}
