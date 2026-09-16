import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: {
      '/api': { target: 'http://localhost:5090', changeOrigin: true },
      '/hubs': { target: 'http://localhost:5090', changeOrigin: true, ws: true }
    }
  }
})
