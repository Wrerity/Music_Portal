import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import api from '../api/client.js'

export default function Catalog() {
  const [search, setSearch] = useState('')
  const [genreIds, setGenreIds] = useState('')
  const [authorIds, setAuthorIds] = useState('')
  const [sortBy, setSortBy] = useState('date')
  const [page, setPage] = useState(1)
  const [data, setData] = useState({ songs:[], totalPages:1, totalCount:0 })
  const [genres, setGenres] = useState([])
  const [authors, setAuthors] = useState([])
  const [loading, setLoading] = useState(false)

  useEffect(()=>{ api.get('/api/genres').then(r=>setGenres(r.data)).catch(()=>{}); api.get('/api/authors').then(r=>setAuthors(r.data)).catch(()=>{}) },[])

  const load = async (p=page) => {
    setLoading(true)
    try {
      const params = new URLSearchParams()
      if(search) params.append('search', search)
      if(genreIds) params.append('genreIds', genreIds)
      if(authorIds) params.append('authorIds', authorIds)
      if(sortBy) params.append('sortBy', sortBy)
      params.append('page', p)
      const { data } = await api.get('/api/songs?' + params.toString())
      setData({ songs: data.songs || data.Songs || [], totalPages: data.totalPages ?? data.TotalPages ?? 1, totalCount: data.totalCount ?? data.TotalCount ?? 0 })
      setPage(p)
    } catch(e){ console.error(e) } finally{ setLoading(false) }
  }
  useEffect(()=>{ load(1) },[])

  return (
    <div>
      <h2 style={h2}>Каталог — GET /api/songs (EF: сортировка/фильтрация/пагинация)</h2>
      <div style={filters}>
        <div><label style={label}>Поиск</label><input value={search} onChange={e=>setSearch(e.target.value)} placeholder="Название..." style={input} /></div>
        <div><label style={label}>Жанр</label><select value={genreIds} onChange={e=>setGenreIds(e.target.value)} style={input}><option value="">Все жанры</option>{genres.map(g=><option key={g.id||g.Id} value={g.id||g.Id}>{g.name||g.Name}</option>)}</select></div>
        <div><label style={label}>Исполнитель</label><select value={authorIds} onChange={e=>setAuthorIds(e.target.value)} style={input}><option value="">Все исполнители</option>{authors.map(a=><option key={a.id||a.Id} value={a.id||a.Id}>{a.name||a.Name}</option>)}</select></div>
        <div><label style={label}>Сортировка</label><select value={sortBy} onChange={e=>setSortBy(e.target.value)} style={input}><option value="date">Новые</option><option value="popularity">Популярные</option><option value="title">По названию</option></select></div>
        <button onClick={()=>load(1)} style={btn}>Применить (GET)</button>
        <button onClick={()=>{setSearch('');setGenreIds('');setAuthorIds('');setSortBy('date');load(1)}} style={btn2}>Сброс</button>
      </div>

      {loading ? <p style={{fontSize:'14px'}}>Загрузка...</p> : (
        <>
          <div style={grid}>
            {data.songs.map(s=>(
              <div key={s.id||s.Id} style={card}>
                <div style={{fontWeight:700, fontSize:'15px'}}>{s.title||s.Title}</div>
                <div style={{fontSize:'14px', opacity:0.7}}>{s.authors||s.Authors} — {s.genres||s.Genres}</div>
                <div style={{fontSize:'14px'}}>{s.playCount||s.PlayCount} слушателей</div>
                <Link to={`/song/${s.id||s.Id}`} style={{fontSize:'14px'}}>Подробнее</Link>
              </div>
            ))}
          </div>
          <div style={{marginTop:'16px', display:'flex', gap:'6px', flexWrap:'wrap', justifyContent:'center'}}>
            {Array.from({length:data.totalPages},(_,i)=>i+1).map(p=>(
              <button key={p} onClick={()=>load(p)} style={{...pageBtn, ...(p===page?{background:'#e94560',color:'#fff'}:{})}}>{p}</button>
            ))}
          </div>
          <p style={{fontSize:'14px', textAlign:'center', marginTop:'8px'}}>Всего {data.totalCount} — стр. {page}/{data.totalPages}</p>
        </>
      )}
    </div>
  )
}
const h2={fontSize:'20px', marginBottom:'12px'}
const filters={display:'grid', gridTemplateColumns:'1fr 1fr 1fr 1fr auto auto', gap:'8px', alignItems:'end', marginBottom:'16px', background:'#f5f5f5', padding:'12px', borderRadius:'8px'}
const label={fontSize:'14px', fontWeight:600, display:'block', marginBottom:'4px'}
const input={padding:'8px', fontSize:'14px', border:'1px solid #ccc', borderRadius:'6px', width:'100%'}
const btn={background:'#e94560', color:'#fff', border:'none', padding:'8px 12px', borderRadius:'6px', fontSize:'14px', cursor:'pointer'}
const btn2={background:'#fff', border:'1px solid #ccc', padding:'8px 12px', borderRadius:'6px', fontSize:'14px', cursor:'pointer'}
const grid={display:'grid', gridTemplateColumns:'repeat(auto-fill,minmax(240px,1fr))', gap:'12px'}
const card={border:'1px solid #ddd', borderRadius:'8px', padding:'12px', background:'#fff'}
const pageBtn={padding:'6px 10px', border:'1px solid #ccc', background:'#fff', borderRadius:'6px', fontSize:'14px', cursor:'pointer'}
