import tailwindcss from '@tailwindcss/vite'
import react from '@vitejs/plugin-react'
import { defineConfig } from 'vitest/config'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:7068',
        changeOrigin: true,
        secure: false,
      },
    },
  },
  test: {
    environment: 'jsdom',
    setupFiles: './src/test/setup.ts',
    // 'forks' (procesos de SO separados) en vez de 'threads': varios
    // archivos de test mutan globals compartidos como document.title
    // (PropertyDetailPage, Normativa*, MiObraPage) — con 'threads' se vio
    // interferencia intermitente entre archivos al correr la suite
    // completa (cada archivo pasaba solo, pero no siempre juntos).
    pool: 'forks',
  },
})
