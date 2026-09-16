import axios from 'axios'

const api = axios.create({
  baseURL: import.meta.env.DEV ? '' : 'http://localhost:5090',
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' },
})

// Bearer JWT из localStorage
api.interceptors.request.use(cfg => {
  const t = localStorage.getItem('token')
  if (t) cfg.headers.Authorization = `Bearer ${t}`
  return cfg
})

api.interceptors.response.use(
  r => r,
  err => {
    if (!err.response) {
      return Promise.reject({ status: 0, data: null, message: `Network Error: API ${err.config?.baseURL}${err.config?.url} недоступен.` })
    }
    const data = err.response.data
    let msg = data?.detail || data?.title || err.message
    // Валидация 400 — показать поля
    if (data?.errors) {
      const fields = Object.entries(data.errors).map(([k,v])=> `${k}: ${Array.isArray(v)?v.join(', '):v}`).join('; ')
      msg = fields || msg
    }
    if (err.response.status === 401) localStorage.removeItem('token')
    return Promise.reject({ status: err.response.status, data, message: msg })
  }
)

export default api
export const setToken = t => t ? localStorage.setItem('token', t) : localStorage.removeItem('token')
export const getToken = () => localStorage.getItem('token')
