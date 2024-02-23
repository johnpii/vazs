import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      "/api/login": {
        target: "https://localhost:7251",
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path.replace(/^\/api\/login/, "/Account/Login"),
      },
    },
  },
});
